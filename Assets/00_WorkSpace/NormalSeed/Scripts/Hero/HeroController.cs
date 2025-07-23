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
    // �� Hero���� ��ų �ִϸ��̼� ����

    private void Awake() => Init();

    private void Init()
    {
        model = GetComponent<HeroModel>();
        view = GetComponent<HeroView>();
        mov = GetComponent<HeroMovement>();

        // �ӽ÷� Hero1�� ������ ������ ����
        heroType = 0;
        model.GetInitStats(heroType);
    }

    private void FixedUpdate()
    {
        // ��Ŭ���ϸ� ��Ŭ���� �������� �̵�
        if (Input.GetMouseButton(1))
        {
            mov.GetMoveDestination(model.MoveSpd);
        }

        mov.LookMoveDir();

        // �����̽��ٸ� ������ ī�޶� �÷��̾� ���� ������
        if (Input.GetKey(KeyCode.Space))
        {
            SetCameraOnHero();
        }
    }

    /// <summary>
    /// ī�޶� �÷��̾� ��ġ�� �����ϴ� �޼���
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
