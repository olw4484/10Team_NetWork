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
    // �� Hero���� ��ų �ִϸ��̼� ����

    private void Awake() => Init();

    private void Init()
    {
        model = GetComponent<HeroModel>();
        view = GetComponent<HeroView>();
        mov = GetComponent<HeroMovement>();

        atkDelay = 0f;

        // �ӽ÷� Hero1�� ������ ������ ���� -> Lobby���� HeroType�� �޾ƿ��� ������� ����� ����
        heroType = 0;
        model.GetInitStats(heroType);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (atkDelay <= 0f)
            {
                mov.HeroAttack(model.MoveSpd, (int)model.Atk, model.AtkRange); // ���� damage ������ ������ ���Ŀ� ���� �ٲ��� �ʿ䰡 ����
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
        // ��Ŭ���ϸ� ��Ŭ���� �������� �̵�
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
