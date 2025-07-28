using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class HeroMovement : MonoBehaviour
{
    [SerializeField] public Camera camera;
    private NavMeshAgent agent;
    private PhotonView pv;

    public bool isMove;
    private Vector3 destination;

    private WaitForSeconds distCheck = new WaitForSeconds(2f);

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

    public void HandleRightClick(float moveSpd, int damage, float atkRange)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // �� Ŭ�� �� �̵�
            if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Obstacle"))
            {
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
                    StartCoroutine(HeroAttackRoutine(hit.collider.transform, damagable, atkRange, damage, moveSpd));
                    return;
                }
            }

            // �Ʊ� ���� Ŭ�� (��: ���󰡱� �� Ŀ���͸���¡ ����)
            Debug.Log("��Ŭ���� ����� �Ʊ��Դϴ�. �⺻ �̵� ó�� �Ǵ� ����.");
            SetDestination(hit.point, moveSpd);
        }
    }


    ///// <summary>
    ///// �̵������� Raycast�� �����ؼ� �̵������� ���� �����̰� �ϴ� �޼���
    ///// </summary>
    ///// <param name="moveSpd"></param>
    //public void GetMoveDestination(float moveSpd)
    //{
    //    // ī�޶󿡼� ���� ���� Ground�� �¾��� �� �װ��� ��ǥ�������� ���� 
    //    RaycastHit hit;
    //    if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
    //    {
    //        if (hit.collider.CompareTag("Ground"))
    //        {
    //            SetDestination(hit.point, moveSpd);
    //        }
    //    }
    //}

    ///// <summary>
    ///// ����� �⺻���� �޼���
    ///// </summary>
    ///// <param name="moveSpd"></param>
    ///// <param name="damage"></param>
    ///// <param name="atkRange"></param>
    //public void HeroAttack(float moveSpd, int damage, float atkRange)
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
    //    {
    //        LGH_IDamagable damagable = hit.collider.GetComponent<LGH_IDamagable>();
    //        PhotonView view = hit.collider.GetComponent<PhotonView>();

    //        if (damagable != null && view != null && !view.IsMine)
    //        {
    //            object targetTeam, myTeam;
    //            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out myTeam) &&
    //                view.Owner.CustomProperties.TryGetValue("Team", out targetTeam))
    //            {
    //                if (!targetTeam.Equals(myTeam))
    //                {
    //                    // ���� ��Ÿ� �ȿ� �ִٸ� �⺻���� ����(�ִϸ��̼� ���, TakeDamage�� ������ ��)
    //                    StartCoroutine(HeroAttackRoutine(hit.collider.transform, damagable, atkRange, damage, moveSpd));
    //                    return;
    //                }
    //            }
    //        }
    //    }
    //}

    private IEnumerator HeroAttackRoutine(Transform target, LGH_IDamagable damagable, float atkRange, int damage, float moveSpd)
    {
        while (true)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= atkRange)
            {
                // TakeDamage�� RPC�� ������ ��
                damagable.TakeDamage(damage);
                Debug.Log("Hero1 �⺻ ����");
                yield break;
            }
            else
            {
                SetDestination(target.position, moveSpd);
            }

            yield return distCheck;
        }
    }

    /// <summary>
    /// Hero�� �̵� ������ ������ �� �̵������� �˷��ִ� �޼���
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="moveSpd"></param>
    public void SetDestination(Vector3 dest, float moveSpd)
    {
        if (pv.IsMine)
        {
            pv.RPC("RPC_SetDestination", RpcTarget.All, dest, moveSpd);
        }
    }

    [PunRPC]
    public void RPC_SetDestination(Vector3 dest, float moveSpd)
    {
        agent.speed = moveSpd;
        //agent.SetDestination(dest);
        destination = dest;
        isMove = true;

        //// Lag Compensation
        //float lag = (float)(PhotonNetwork.Time - timestamp);
        //Vector3 direction = (dest - transform.position).normalized;
        //float predictedDistance = moveSpd * lag;
        //Vector3 compensatedPosition = transform.position + direction * predictedDistance;

        //agent.Warp(compensatedPosition);  // ���� ����
        agent.SetDestination(dest);
        //Debug.Log($"[RPC] ������ ����ȭ: {dest}, ���� ���� �Ÿ�: {predictedDistance:F2}");
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
