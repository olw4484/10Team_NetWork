using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class HeroController : MonoBehaviour, IDamageable, IPunInstantiateMagicCallback
{
    public HeroModel model;
    public HeroView view;
    public HeroMovement mov;
    public NavMeshAgent agent;
    public PhotonView pv;

    [SerializeField] private int heroType;
    public bool isUsingSkill = false;
    public bool isDead = false;
    private float atkDelay;
    private float genTime = 1f;

    public int teamId;

    int IDamageable.teamId => this.teamId;
    bool IDamageable.isDead => this.isDead;

    private int currentAnimationHash = -1;
    public readonly int IDLE_HASH = Animator.StringToHash("Idle");
    public readonly int MOVE_HASH = Animator.StringToHash("Move");
    public readonly int ATTACK_HASH = Animator.StringToHash("Attack");
    public readonly int DEAD_HASH = Animator.StringToHash("Dead");

    public readonly int Q_HASH = Animator.StringToHash("QSkill");
    public readonly int W_HASH = Animator.StringToHash("WSkill");
    public readonly int E_HASH = Animator.StringToHash("ESkill");
    public readonly int R_HASH = Animator.StringToHash("RSkill");

    // 각 Hero마다 스킬 애니메이션 존재

    private void Awake() => Init();

    private void Init()
    {
        Debug.Log("HeroController Init");
        model = GetComponent<HeroModel>();
        view = GetComponent<HeroView>();
        mov = GetComponent<HeroMovement>();
        agent = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();

        atkDelay = 0f;

        // 임시로 Hero1을 선택한 것으로 가정 -> 게임이 시작되면 HeroType을 결정하게
        heroType = 0;
        model.GetInitStats(heroType);

        model.CurHP.Value = model.MaxHP;
        model.CurMP.Value = model.MaxMP;
    }

    private void Start()
    {
        StartCoroutine(RegisterRoutine());
        model.CurHP.Subscribe(OnHPChanged);
        model.CurMP.Subscribe(OnMPChanged);
        
        model.Level.Subscribe(OnLevelChanged);

        mov.OnMoveStateChanged += (moving) =>
        {
            view.animator.SetBool("isMove", moving);
        };
        mov.OnAttackStateChanged += (attack) =>
        {
            view.animator.SetBool("isAttack", attack);
        };
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            teamId = (int)data[0];
            Debug.Log($"[HeroController] teamId 동기화: {teamId}");
        }
    }

    private IEnumerator RegisterRoutine()
    {
        while (LGH_TestGameManager.Instance == null)
        {
            yield return null; // GameManager가 생성되기를 기다림
        }
        LGH_TestGameManager.Instance.RegisterPlayer(this.gameObject);
        yield break;
    }

    private void Update()
    {
        if (!pv.IsMine) return;

        if (Input.GetMouseButtonDown(1))
        {
            atkDelay = 1f / model.AtkSpd;
            mov.HandleRightClick(model.MoveSpd, (int)model.Atk, model.AtkRange, atkDelay);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            agent.ResetPath();
        }

        if (genTime <= 0f)
        {
            HandleAutoGen();
        }
        else
        {
            genTime -= Time.deltaTime;
        }

        // Test용 코드들
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (pv.IsMine)
            {
                pv.RPC("TakeDamage", RpcTarget.All, 100, default);
                Debug.Log("현재 HP : " + model.CurHP.Value);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddExp(50);
            Debug.Log("현재 경험치 : " + model.Exp.Value);
        }
    }

    private void FixedUpdate()
    {
        if (!pv.IsMine) return;

        mov.LookMoveDir();
    }

    private void LateUpdate()
    {
        //HandleAnimation();
    }

    /// <summary>
    /// HP와 MP 젠을 관리하는 메서드
    /// </summary>
    void HandleAutoGen()
    {
        AutoHpGen();
        AutoMpGen();
    }

    void AutoHpGen()
    {   
        if (model.CurHP.Value + model.HPGen > model.MaxHP)
        {
            model.CurHP.Value = model.MaxHP;
        }
        else
        {
            model.CurHP.Value += model.HPGen;
            genTime = 1f;
        }
    }

    void AutoMpGen()
    {
        if (model.CurMP.Value + model.MPGen > model.MaxMP)
        {
            model.CurMP.Value = model.MaxMP;
        }
        else
        {
            model.CurMP.Value += model.MPGen;
            genTime = 1f;
        }
    }

    void OnHPChanged(float newHP)
    {
        pv.RPC(nameof(UpdateHeroHP), RpcTarget.All, model.MaxHP, newHP);
        Debug.Log("현재 체력 : " + model.CurHP.Value);
    }

    void OnMPChanged(float newMP)
    {
        pv.RPC(nameof(UpdateHeroMP), RpcTarget.All, model.MaxMP, newMP);
        Debug.Log("현재 마나 : " + model.CurMP.Value);
    }
    /// <summary>
    /// 경험치 증가 시 호출되는 메서드
    /// </summary>
    /// <param name="amount"></param>
    public void AddExp(int amount)
    {
        model.Exp.Value += amount;
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        int currentLevel = model.Level.Value;
        float currentEXP = model.Exp.Value;

        // 누적 경험치 기반으로 여러 레벨업 처리
        while (model.levelExpTable.ContainsKey(currentLevel + 1) &&
               currentEXP >= model.levelExpTable[currentLevel + 1])
        {
            currentLevel++;
            TestSkillManager.Instance.skillPoint.Value++;
            Debug.Log($"레벨업! → {currentLevel}레벨");
        }

        // 최종 레벨 반영
        model.Level.Value = currentLevel;
    }

    void OnLevelChanged(int newLevel)
    {
        float hp = model.MaxHP;
        float mp = model.MaxMP;

        pv.RPC(nameof(UpdateHeroLevel), RpcTarget.All, newLevel);

        float levelUpHp = model.MaxHP - hp;
        float levelUpMp = model.MaxMP - mp;

        model.CurHP.Value += levelUpHp;
        model.CurMP.Value += levelUpMp;

        TestSkillManager.Instance.RefreshSkillButton();
    }

    [PunRPC]
    public void UpdateHeroHP(float maxHP, float curHP)
    {
        view.SetHpBar(maxHP, curHP);
    }

    [PunRPC]
    public void UpdateHeroMP(float maxMP, float curMP)
    {
        view.SetMpBar(maxMP, curMP);
    }

    [PunRPC]
    public void UpdateHeroLevel(int nextLevel)
    {
        // 최대 레벨 제한
        if (nextLevel > 18)
        {
            model.Level.Value = 18;
            Debug.Log("최대 레벨임.");
            return;
        }

        HeroStat nextStat = model.GetStatByLevel(heroType, nextLevel);

        if (nextStat.Equals(default(HeroStat)))
        {
            Debug.LogWarning($"레벨 {nextLevel} 데이터 없음.");
            return;
        }

        // 스탯 갱신
        model.Name = nextStat.Name;
        model.MaxHP = nextStat.MaxHP;
        model.MaxMP = nextStat.MaxMP;
        model.MoveSpd = nextStat.MoveSpd;
        model.Atk = nextStat.Atk;
        model.AtkRange = nextStat.AtkRange;
        model.AtkSpd = nextStat.AtkSpd;
        model.Def = nextStat.Def;
        model.HPGen = nextStat.HPGen;
        model.MPGen = nextStat.MPGen;

        Debug.Log($"{model.Name} 현재 레벨 : {nextLevel}레벨");
    }

    [PunRPC]
    public void GetHeal(int amount)
    {
        
    }

    [PunRPC]
    public void RPC_TakeDamage(int amount, int attackerViewID = -1)
    {
        model.CurHP.Value -= amount;
        Debug.Log($"{amount}의 데미지를 입음. 현재 HP : {model.CurHP.Value}");

        if (model.CurHP.Value <= 0)
        {
            if (pv.IsMine)
            {
                pv.RPC("Dead", RpcTarget.All);
            }
        }
    }

    public void TakeDamage(int amount, GameObject attacker = null)
    {
        model.CurHP.Value -= amount;
    }

    [PunRPC]
    public void Dead()
    {
        StartCoroutine(DeadRoutine());
        gameObject.SetActive(false);

        if (pv.IsMine)
        {
            LGH_TestGameManager.Instance.RequestRespawn(this);
        }
    }

    private IEnumerator DeadRoutine()
    {
        isDead = true;
        view.animator.SetBool("isDead", isDead);
        yield return new WaitForSeconds(0.5f);
    }

    [PunRPC]
    public void RPC_Respawn(Vector3 spawnPos, float hp)
    {
        transform.position = spawnPos;
        gameObject.SetActive(true);
        model.CurHP.Value = hp;
        isDead = false;

        Debug.Log($"리스폰 완료. 위치: {spawnPos}, HP: {hp}");
    }

    private void HandleAnimation()
    {
        if (isUsingSkill) return;

        if (isDead)
        {
            view.animator.SetBool("isDead", isDead);
        }
        else if (mov.isMove)
        {
            
        }
        else if (mov.isAttack)
        {
            
        }
        else
        {
            
        }
    }

    private void OnDisable()
    {
        Debug.Log("비활성화됨");
    }
}
