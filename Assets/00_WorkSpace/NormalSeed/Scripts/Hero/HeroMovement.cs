using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

public class HeroMovement : MonoBehaviour
{
    [SerializeField] public Camera camera;
    private NavMeshAgent agent;
    private PhotonView pv;

    public bool isMove;
    private bool isAttacking = false;
    private Vector3 destination;

    private Coroutine attackCoroutine;
    private WaitForSeconds distCheck = new WaitForSeconds(0.1f);

    private void Awake() => Init();

    private void Init()
    {
        camera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();

        agent.acceleration = 100f;
        agent.angularSpeed = 720f;
        agent.stoppingDistance = 0.5f;
    }

    /// <summary>
    /// GetMoveDestination�� HeroAttack�� ��ģ �޼���
    /// </summary>
    /// <param name="moveSpd"></param>
    /// <param name="damage"></param>
    /// <param name="atkRange"></param>

    public void HandleRightClick(float moveSpd, int damage, float atkRange, float atkDelay)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // �� Ŭ�� �� �̵�
            if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Obstacle"))
            {
                isAttacking = false;
                SetDestination(hit.point, moveSpd);
                return;
            }

            // �� ������Ʈ üũ
            PhotonView targetView = hit.collider.GetComponent<PhotonView>();
            LGH_IDamagable damagable = hit.collider.GetComponent<LGH_IDamagable>();

            if (damagable != null && targetView != null && !targetView.IsMine)
            {
                object myTeam, targetTeam;
                bool hasMyTeam = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out myTeam);
                bool hasTargetTeam = targetView.Owner.CustomProperties.TryGetValue("Team", out targetTeam);

                // �� ������ ����ü�� �Ǿ��ֱ� ������ ����ü�� string���� TryParse �߰�
                if (hasMyTeam && hasTargetTeam &&
                    Enum.TryParse(myTeam.ToString(), out TestTeamSetting myTeamEnum) &&
                    Enum.TryParse(targetTeam.ToString(), out TestTeamSetting targetTeamEnum) &&
                    myTeamEnum != targetTeamEnum)
                {
                    if (attackCoroutine != null)
                    {
                        return;
                    }

                    attackCoroutine = StartCoroutine(HeroAttackRoutine(hit.collider.transform, damagable, atkRange, atkDelay, damage, moveSpd));

                    return;
                }
                // �Ʊ� ���� Ŭ�� (��: ���󰡱� �� Ŀ���͸���¡ ����)
                Debug.Log("��Ŭ���� ����� �Ʊ��Դϴ�. �⺻ �̵� ó�� �Ǵ� ����.");
            }
            SetDestination(hit.point, moveSpd);
        }
    }

    /// <summary>
    /// ���� �⺻���� �ڷ�ƾ
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damagable"></param>
    /// <param name="atkRange"></param>
    /// <param name="atkDelay"></param>
    /// <param name="damage"></param>
    /// <param name="moveSpd"></param>
    /// <returns></returns>
    private IEnumerator HeroAttackRoutine(Transform target, LGH_IDamagable damagable, float atkRange, float atkDelay, int damage, float moveSpd)
    {
        Debug.Log("�⺻���� �ڷ�ƾ ���۵�");
        if (isAttacking) yield break;

        isAttacking = true;
        while (true)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist <= atkRange && atkDelay <= 0f)
            {
                // ���� ����ȭ�� ���� RPC ����
                ExecuteAttack(target, damagable, damage);
                attackCoroutine = null;
                break;
            }
            else
            {
                isAttacking = false;
                agent.isStopped = false;
                SetDestination(target.position, moveSpd);
            }
            yield return distCheck;
        }
        isAttacking = false;
        attackCoroutine = null;
    }

    /// <summary>
    /// ���� ���� ���� �޼���
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damagable"></param>
    /// <param name="damage"></param>
    private void ExecuteAttack(Transform target, LGH_IDamagable damagable, int damage)
    {
        pv.RPC(nameof(RPC_StopAndFace), RpcTarget.All, target.position);
        
        // Ÿ���� ���� �ִ� HeroControlelr ���� TakeDamage RPC ����
        PhotonView targetPv = target.gameObject.GetComponent<PhotonView>();
        if (targetPv != null)
        {
            targetPv.RPC(nameof(HeroController.TakeDamage), RpcTarget.All, damage);
        }
        Debug.Log("Hero1 �⺻ ����");
    }

    /// <summary>
    /// �̵� ������ ��Ʈ��ũ�� ����ȭ�ϱ� ���� RPC �޼���
    /// </summary>
    /// <param name="lookPos"></param>
    [PunRPC]
    public void RPC_StopAndFace(Vector3 lookPos)
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        // ȸ�� ����ȭ
        Vector3 dir = (lookPos - transform.position).normalized;
        if (dir.sqrMagnitude > 0.1f)
            transform.forward = new Vector3(dir.x, 0, dir.z);
    }


    /// <summary>
    /// Hero�� �̵� ������ ������ �� �̵������� �˷��ִ� �޼���
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="moveSpd"></param>
    public void SetDestination(Vector3 dest, float moveSpd)
    {
        if (pv.IsMine && !isAttacking)
        {
            pv.RPC("RPC_SetDestination", RpcTarget.All, dest, moveSpd);
        }
    }

    /// <summary>
    /// �̵� ��ǥ ������ ��Ʈ��ũ�� ����ȭ�ϱ� ���� RPC �޼���
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="moveSpd"></param>
    [PunRPC]
    public void RPC_SetDestination(Vector3 dest, float moveSpd)
    {
        agent.speed = moveSpd;
        destination = dest;
        isMove = true;
        agent.SetDestination(dest);
    }


    /// <summary>
    /// �̵������� �����Ǿ��� �� �̵������� �ٶ󺸵��� �ϴ� �޼���
    /// </summary>
    public void LookMoveDir()
    {
        // �̵� ������ �����Ǿ��� ��
        if (isMove)
        {
            // �̵��� �����ٸ�
            if (agent.velocity.magnitude == 0.0f)
            {
                isMove = false;
                return;
            }
        }

        // �̵����̶�� �̵������� �ٶ󺸵��� ����
        var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.forward = dir;
        }
    }
}
