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
    /// GetMoveDestination과 HeroAttack을 합친 메서드
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
            // 땅 클릭 시 이동
            if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Obstacle"))
            {
                isAttacking = false;
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
                    if (attackCoroutine != null)
                    {
                        return;
                    }

                    attackCoroutine = StartCoroutine(HeroAttackRoutine(hit.collider.transform, damagable, atkRange, atkDelay, damage, moveSpd));

                    return;
                }
                // 아군 유닛 클릭 (예: 따라가기 등 커스터마이징 가능)
                Debug.Log("우클릭된 대상은 아군입니다. 기본 이동 처리 또는 무시.");
            }
            SetDestination(hit.point, moveSpd);
        }
    }

    /// <summary>
    /// 영웅 기본공격 코루틴
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
        Debug.Log("기본공격 코루틴 시작됨");
        if (isAttacking) yield break;

        isAttacking = true;
        while (true)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist <= atkRange && atkDelay <= 0f)
            {
                // 멈춤 동기화를 위해 RPC 실행
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
    /// 실제 공격 실행 메서드
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damagable"></param>
    /// <param name="damage"></param>
    private void ExecuteAttack(Transform target, LGH_IDamagable damagable, int damage)
    {
        pv.RPC(nameof(RPC_StopAndFace), RpcTarget.All, target.position);
        
        // 타겟이 갖고 있는 HeroControlelr 안의 TakeDamage RPC 실행
        PhotonView targetPv = target.gameObject.GetComponent<PhotonView>();
        if (targetPv != null)
        {
            targetPv.RPC(nameof(HeroController.TakeDamage), RpcTarget.All, damage);
        }
        Debug.Log("Hero1 기본 공격");
    }

    /// <summary>
    /// 이동 멈춤을 네트워크와 동기화하기 위한 RPC 메서드
    /// </summary>
    /// <param name="lookPos"></param>
    [PunRPC]
    public void RPC_StopAndFace(Vector3 lookPos)
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        // 회전 동기화
        Vector3 dir = (lookPos - transform.position).normalized;
        if (dir.sqrMagnitude > 0.1f)
            transform.forward = new Vector3(dir.x, 0, dir.z);
    }


    /// <summary>
    /// Hero의 이동 지점을 결정한 후 이동중임을 알려주는 메서드
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
    /// 이동 목표 지점을 네트워크와 동기화하기 위한 RPC 메서드
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
