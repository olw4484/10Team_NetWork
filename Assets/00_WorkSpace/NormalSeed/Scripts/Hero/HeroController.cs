using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour, LGH_IDamagable
{
    protected HeroModel model;
    protected HeroView view;
    protected HeroMovement mov;

    private int heroType;
    private bool isInCombat;

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

        // 임시로 Hero1을 선택한 것으로 가정
        heroType = 0;
        model.GetInitStats(heroType);
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            mov.GetMoveDestination();
        }

        mov.Move(model.MoveSpd);
    }

    public void GetHeal(int amount)
    {
        
    }

    public void TakeDamage(int amount)
    {
        
    }
}
