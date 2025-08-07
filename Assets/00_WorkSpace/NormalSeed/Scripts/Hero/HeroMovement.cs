using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class HeroMovement : MonoBehaviour
{
    [SerializeField] public Camera mainCamera;
    private NavMeshAgent agent;
    private PhotonView pv;

    public bool isMove;
    public bool isAttacking = false;
    public bool isAttack = false;
    [SerializeField] private float atkCooldown;
    private Vector3 destination;
    private float attackLockTime = 0.6f;

    private Coroutine attackCoroutine;
    private WaitForSeconds distCheck = new WaitForSeconds(0.1f);

    private void Awake() => Init();

    private void Init()
    {
        mainCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();

        agent.acceleration = 100f;
        agent.angularSpeed = 720f;
        agent.stoppingDistance = 0.5f;
    }

    private void Update()
    {
        if (atkCooldown > 0f)
        {
            atkCooldown -= Time.deltaTime;
        }
    }

    /// <summary>
    /// GetMoveDestination과 HeroAttack을 합친 메서드
    /// </summary>
    /// <param name="moveSpd"></param>
    /// <param name="damage"></param>
    /// <param name="atkRange"></param>

    public void HandleRightClick(float moveSpd, int damage, float atkRange, float atkDelay)
    {
        if (isAttack) return;

        HeroController controller = this.gameObject.GetComponent<HeroController>();

        if (controller.isUsingSkill) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 땅 클릭 시 이동
            if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Obstacle"))
            {
                CancleAttackRoutine();
                isAttacking = false;
                SetDestination(hit.point, moveSpd);
                return;
            }

            // 적 오브젝트 체크
            PhotonView targetView = hit.collider.GetComponent<PhotonView>();
            IDamageable damagable = hit.collider.GetComponent<IDamageable>();

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
    private IEnumerator HeroAttackRoutine(Transform target, IDamageable damagable, float atkRange, float atkDelay, int damage, float moveSpd)
    {
        Debug.Log("기본공격 코루틴 시작됨");
        if (isAttacking)
        {
            yield break;
        }

        isAttacking = true;

        while (true)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist <= atkRange && atkCooldown <= 0f)
            {
                ExecuteAttack(target, damagable, damage);
                attackCoroutine = null;
                atkCooldown = atkDelay;
                break;
            }
            else if (dist <= atkRange && atkCooldown > 0f)
            {
                pv.RPC(nameof(RPC_StopAndFace), RpcTarget.All, target.position);
                break;
            }
            else if (dist > atkRange)
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

    private void CancleAttackRoutine()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            isAttacking = false;
        }
    }

    private IEnumerator AttackLockRoutine(float lockTime)
    {
        float timer = 0f;
        while (timer < lockTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        isAttack = false;
    }

    /// <summary>
    /// 실제 공격 실행 메서드
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damagable"></param>
    /// <param name="damage"></param>
    public void ExecuteAttack(Transform target, IDamageable damagable, int damage)
    {
        // 멈춤 동기화를 위해 RPC 실행
        pv.RPC(nameof(RPC_StopAndFace), RpcTarget.All, target.position);

        isMove = false;
        isAttack = true;
        
        // 타겟이 갖고 있는 TakeDamage RPC 실행
        PhotonView targetPv = target.gameObject.GetComponent<PhotonView>();
        if (targetPv != null)
        {
            targetPv.RPC("RPC_TakeDamage", RpcTarget.All, damage, pv.ViewID); // TODO 데미지 줄 때 내가 줬다고 전달해줘야 함
        }
        Debug.Log("Hero1 기본 공격");

        StartCoroutine(AttackLockRoutine(attackLockTime));
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
            pv.RPC(nameof(RPC_SetDestination), RpcTarget.All, dest, moveSpd);
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

    [PunRPC]
    public void InterruptMovement()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;
        isAttack = false;
        isMove = false;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();
    }

}
