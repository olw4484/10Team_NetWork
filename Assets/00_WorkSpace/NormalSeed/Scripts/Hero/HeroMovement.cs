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
    /// GetMoveDestination과 HeroAttack을 합친 메서드
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
            // 땅 클릭 시 이동
            if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Obstacle"))
            {
                SetDestination(hit.point, moveSpd);
                return;
            }

            // 적 오브젝트 체크
            PhotonView targetView = hit.collider.GetComponent<PhotonView>();
            LGH_IDamagable damagable = hit.collider.GetComponent<LGH_IDamagable>();

            if (damagable != null && targetView != null && !targetView.IsMine)
            {
                object myTeam, targetTeam;
                bool hasMyTeam = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out myTeam);
                bool hasTargetTeam = targetView.Owner.CustomProperties.TryGetValue("Team", out targetTeam);

                // 팀 구분이 구조체로 되어있기 떄문에 구조체를 string으로 TryParse 추가
                if (hasMyTeam && hasTargetTeam &&
                    Enum.TryParse(myTeam.ToString(), out TestTeamSetting myTeamEnum) &&
                    Enum.TryParse(targetTeam.ToString(), out TestTeamSetting targetTeamEnum) &&
                    myTeamEnum != targetTeamEnum)
                {
                    StartCoroutine(HeroAttackRoutine(hit.collider.transform, damagable, atkRange, damage, moveSpd));
                    return;
                }
            }

            // 아군 유닛 클릭 (예: 따라가기 등 커스터마이징 가능)
            Debug.Log("우클릭된 대상은 아군입니다. 기본 이동 처리 또는 무시.");
            SetDestination(hit.point, moveSpd);
        }
    }


    ///// <summary>
    ///// 이동지점을 Raycast로 지정해서 이동지점을 향해 움직이게 하는 메서드
    ///// </summary>
    ///// <param name="moveSpd"></param>
    //public void GetMoveDestination(float moveSpd)
    //{
    //    // 카메라에서 빛을 쏴서 Ground에 맞았을 때 그곳을 목표지점으로 설정 
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
    ///// 히어로 기본공격 메서드
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
    //                    // 공격 사거리 안에 있다면 기본공격 실행(애니메이션 재생, TakeDamage로 데미지 줌)
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
                // TakeDamage를 RPC로 만들어야 함
                damagable.TakeDamage(damage);
                Debug.Log("Hero1 기본 공격");
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
    /// Hero의 이동 지점을 결정한 후 이동중임을 알려주는 메서드
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

        //agent.Warp(compensatedPosition);  // 순간 보정
        agent.SetDestination(dest);
        //Debug.Log($"[RPC] 목적지 동기화: {dest}, 지연 보정 거리: {predictedDistance:F2}");
    }


    /// <summary>
    /// 이동지점이 설정되었을 때 이동방향을 바라보도록 하는 메서드
    /// </summary>
    public void LookMoveDir()
    {
        // 이동 지점이 설정되었을 때
        if (isMove)
        {
            // 이동이 끝난다면
            if (agent.velocity.magnitude == 0.0f)
            {
                isMove = false;
                return;
            }
        }

        // 이동중이라면 이동방향을 바라보도록 설정
        var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.forward = dir;
        }
    }
}
