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

    private Vector3 cameraOffset = new Vector3(5f, 19f, -5f);

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
        // 우클릭하면 우클릭한 지점으로 이동
        if (Input.GetMouseButton(1))
        {
            mov.GetMoveDestination(model.MoveSpd);
        }

        mov.LookMoveDir();

        // 스페이스바를 누르면 카메라를 플레이어 위에 고정함
        if (Input.GetKey(KeyCode.Space))
        {
            SetCameraOnHero();
        }
    }

    /// <summary>
    /// 카메라를 플레이어 위치에 고정하는 메서드
    /// </summary>
    private void SetCameraOnHero()
    {
        mov.camera.transform.position = transform.position + cameraOffset;
    }

    public void GetHeal(int amount)
    {
        
    }

    public void TakeDamage(int amount)
    {
        
    }
}
