using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeroController : MonoBehaviour, LGH_IDamagable
{
    public HeroModel model;
    public HeroView view;
    public HeroMovement mov;
    public NavMeshAgent agent;
    private PhotonView pv;

    [SerializeField] private int heroType;
    private bool isInCombat;

    private Vector3 cameraOffset = new Vector3(5f, 19f, -5f);

    private float atkDelay;

    public readonly int IDLE_HASH = Animator.StringToHash("Idle");
    public readonly int MOVE_HASH = Animator.StringToHash("Move");
    public readonly int ATTACK_HASH = Animator.StringToHash("Attack");
    public readonly int DEAD_HASH = Animator.StringToHash("Dead");
    // �� Hero���� ��ų �ִϸ��̼� ����

    private void Awake() => Init();

    private void Init()
    {
        model = GetComponent<HeroModel>();
        view = GetComponent<HeroView>();
        mov = GetComponent<HeroMovement>();
        agent = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();

        atkDelay = 0f;

        // �ӽ÷� Hero1�� ������ ������ ���� -> ������ ���۵Ǹ� HeroType�� �����ϰ�
        heroType = 0;
        model.GetInitStats(heroType);

        model.CurHP.Value = model.MaxHP;
        model.CurMP.Value = model.MaxMP;
    }

    private void Update()
    {
        if (!pv.IsMine) return;

        if (Input.GetMouseButtonDown(1))
        {
            //if (atkDelay <= 0f)
            //{
            //    mov.HeroAttack(model.MoveSpd, (int)model.Atk, model.AtkRange); // ���� damage ������ ������ ���Ŀ� ���� �ٲ��� �ʿ䰡 ����
            //    atkDelay = 1 / model.AtkSpd;
            //}
            mov.HandleRightClick(model.MoveSpd, (int)model.Atk, model.AtkRange, atkDelay);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            agent.ResetPath();
        }
    }

    private void FixedUpdate()
    {
        if (!pv.IsMine) return;

        mov.LookMoveDir();
    }

    [PunRPC]
    public void GetHeal(int amount)
    {
        
    }

    [PunRPC]
    public void TakeDamage(int amount)
    {
        model.CurHP.Value -= amount;
        Debug.Log($"{amount}�� �������� ����. ���� HP : {model.CurHP.Value}");
    }
}
