using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour, LGH_IDamagable
{
    public HeroModel model;
    public HeroView view;
    public HeroMovement mov;

    [SerializeField] private int heroType;
    private bool isInCombat;

    private Vector3 cameraOffset = new Vector3(5f, 19f, -5f);

    private float atkDelay;

    public readonly int IDLE_HASH = Animator.StringToHash("Idle");
    public readonly int MOVE_HASH = Animator.StringToHash("Move");
    public readonly int ATTACK_HASH = Animator.StringToHash("Attack");
    public readonly int DEAD_HASH = Animator.StringToHash("Dead");
    // 각 Hero마다 스킬 애니메이션 존재

    private void Awake() => Init();

    private void Init()
    {
        model = GetComponent<HeroModel>();
        view = GetComponent<HeroView>();
        mov = GetComponent<HeroMovement>();

        atkDelay = 0f;

        // 임시로 Hero1을 선택한 것으로 가정 -> Lobby에서 HeroType을 받아오는 방식으로 만들고 싶음
        heroType = 0;
        model.GetInitStats(heroType);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (atkDelay <= 0f)
            {
                mov.HeroAttack(model.MoveSpd, (int)model.Atk, model.AtkRange); // 추후 damage 변수는 데미지 공식에 따라 바꿔줄 필요가 있음
                atkDelay = 1 / model.AtkSpd;
            }
        }

        if (atkDelay > 0f)
        {
            atkDelay -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // 우클릭하면 우클릭한 지점으로 이동
        if (Input.GetMouseButton(1))
        {
            mov.GetMoveDestination(model.MoveSpd);
        }

        mov.LookMoveDir();
    }

    public void GetHeal(int amount)
    {
        
    }

    public void TakeDamage(int amount)
    {
        
    }
}
