using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hero1SkillSet : SkillSet
{
    //public SkillSO skill_Q;
    //public SkillSO skill_W;
    //public SkillSO skill_E;
    //public SkillSO skill_R;

    // protected Camera mainCam;
    //protected HeroController hero;

    private WaitForSeconds distCheck = new WaitForSeconds(0.1f);

    #region UseQ
    public override void UseQ()
    {
        isQExecuted = true;
        // 마우스 방향에 부채꼴로 공격하는 스킬
        Vector3 originPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 attackDir;

        // Q스킬의 사거리만큼의 OverlapSphere로 충돌 검색
        Collider[] hits = Physics.OverlapSphere(originPos, skill_Q.skillRange);

        RaycastHit hit;
        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            // 공격 방향은 마우스 위치로 설정
            attackDir = (hit.point - originPos).normalized;
            // 공격 방향을 바라보게 forward를 바꿔줌
            transform.forward = new Vector3(attackDir.x, transform.position.y, attackDir.z);

            foreach (Collider collider in hits)
            {
                IDamageable damagable = collider.GetComponent<IDamageable>();
                PhotonView view = collider.GetComponent<PhotonView>();

                // OverlapSphere 안의 오브젝트가 damagable이 아니거나 photonView를 가지고 있지 않거나 photonView가 내거라면 스킵
                if (damagable == null || view == null || view.IsMine) continue;

                object targetTeam, myTeam;
                // CustomProperty로 로비화면에서 팀이 정해지도록 해야 작동함
                if (view.Owner.CustomProperties.TryGetValue("Team", out targetTeam) && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out myTeam))
                {
                    if (targetTeam == myTeam) continue;
                }

                Vector3 colliderPos = new Vector3(collider.transform.position.x, 0, collider.transform.position.z);
                Vector3 toTarget = colliderPos - originPos;

                float angle = Vector3.Angle(attackDir, toTarget);
                // 60도 범위의 부채꼴 범위 안의 적을 공격할 것이기 때문에 공격 방향 기준 30도 이내의 적만 검색
                if (angle <= 30)
                {
                    // 일단 데미지 계산식 없이 깡 스킬 데미지 부여
                    view.RPC(nameof(HeroController.TakeDamage), RpcTarget.All, skill_Q.curDamage, default);
                }
            }
            Debug.Log("BladeWind");
        }
    }
    #endregion

    public override void UseW()
    {
        isWExecuted = true;
        // 5초동안 방어력을 총 공격력의 0.2배만큼 올려주는 스킬
        StartCoroutine(HardenRoutine());
    }

    private IEnumerator HardenRoutine()
    {
        hero.model.Def = hero.model.Def + hero.model.Atk * 0.2f;
        Debug.Log($"방어력 올라감. 방어력 : {hero.model.Def}");
        yield return new WaitForSeconds(5f);
        hero.model.Def = hero.model.Def - hero.model.Atk * 0.2f;
        Debug.Log($"방어력 돌아감. 방어력 : {hero.model.Def}");
    }

    public override void UseE()
    {
        isEExecuted = true;
        hero.mov.isMove = false;

        Vector3 originPos = new Vector3(transform.position.x, 0, transform.position.z);
        RaycastHit hit;
        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            Vector3 dashDir = (hit.point - originPos).normalized;
            pv.RPC(nameof(RPC_StartBash), RpcTarget.All, dashDir);
            Debug.Log("RPC_StartBash 호출됨, dashDir: " + dashDir);
        }
    }

    private IEnumerator BashRoutine(Vector3 dashDir)
    {
        Debug.Log("BashRoutine 시작!");

        // 마우스 방향으로 돌진하고 경로상에 부딪힌 적에 데미지를 주고 적 Hero나 장애물과 부딪히면 멈추는 스킬

        float dashDuration = 0.5f;
        float dashSpeed = 10f;
        float timer = 0f;

        Collider heroCollider = hero.GetComponent<Collider>();
        heroCollider.isTrigger = true;

        agent.isStopped = true;
        agent.enabled = false;

        transform.forward = new Vector3(dashDir.x, 0, dashDir.z);

        while (timer < dashDuration)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.6f);
            bool hitDetected = false;

            foreach (Collider collider in hits)
            {
                IDamageable damagable = collider.GetComponent<IDamageable>();
                PhotonView view = collider.GetComponent<PhotonView>();
                bool isPlayerOrObstacle = collider.CompareTag("Player") || collider.CompareTag("Obstacle");

                // 자신은 collider에서 제외
                if (collider.gameObject == gameObject) continue;

                // 먼저 데미지를 줄 수 있는 충돌인지 체크
                if (damagable != null && view != null && !view.IsMine)
                {
                    object targetTeam, myTeam;
                    if (view.Owner.CustomProperties.TryGetValue("Team", out targetTeam) &&
                        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out myTeam))
                    {
                        if (targetTeam == myTeam) continue;
                    }

                    view.RPC(nameof(HeroController.TakeDamage), RpcTarget.All, skill_E.curDamage, default);
                    Debug.Log("Bash Hit");
                }

                // 만약 벽 또는 상대 영웅과 충돌했다면 멈춤
                if (isPlayerOrObstacle)
                {
                    Debug.Log("벽 또는 상대 영웅과 충돌 감지");
                    hitDetected = true;
                    break;
                }
            }

            if (hitDetected)
            {
                dashSpeed = 0f;

                agent.enabled = true;
                agent.isStopped = false;
                yield break;
            }

            transform.position += dashDir * dashSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        agent.enabled = true;
        agent.isStopped = false;
        agent.ResetPath();
        heroCollider.isTrigger = false;
    }

    [PunRPC]
    public void RPC_StartBash(Vector3 dashDir)
    {
        StartCoroutine(BashRoutine(dashDir));
    }

    public override void UseR()
    {
        // 사정거리 안의 Hero를 선택해서 공격 가능, 적에게 큰 데미지를 주고 이동속도를 1초동안 감소시키는 스킬
        StartCoroutine(BrutalSmiteRoutine());
        isRExecuted = false;
    }

    private IEnumerator BrutalSmiteRoutine()
    {
        if (!TryGetValidTarget(out Transform target, out IDamageable damagable, out HeroController targetHero))
            yield break;

        while (true)
        {
            if (IsInSkillRange(target.position))
            {
                ExecuteSkill(target, damagable, targetHero);
                isRExecuted = true;
                break;
            }
            else
            {
                FollowTarget(target.position);
            }

            yield return distCheck;
        }
    }

    private bool TryGetValidTarget(out Transform target, out IDamageable damagable, out HeroController targetHero)
    {
        target = null;
        damagable = null;
        targetHero = null;

        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            return false;

        damagable = hit.collider.GetComponent<IDamageable>();
        PhotonView view = hit.collider.GetComponent<PhotonView>();
        targetHero = hit.collider.GetComponent<HeroController>();

        if (damagable == null || view == null || view.IsMine)
            return false;

        if (view.Owner.CustomProperties.TryGetValue("Team", out object targetTeam) &&
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object myTeam) &&
            targetTeam.Equals(myTeam))
            return false;

        target = hit.collider.transform;
        return true;
    }

    private bool IsInSkillRange(Vector3 targetPosition)
    {
        return Vector3.Distance(targetPosition, transform.position) <= skill_R.skillRange;
    }

    private void ExecuteSkill(Transform target, IDamageable damagable, HeroController targetHero)
    {
        hero.mov.ExecuteAttack(target, damagable, skill_R.curDamage);

        if (targetHero != null)
        {
            StartCoroutine(ApplyMovementSlow(targetHero));
        }
    }

    private IEnumerator ApplyMovementSlow(HeroController targetHero)
    {
        float originalSpeed = targetHero.model.MoveSpd;
        targetHero.model.MoveSpd *= 0.5f;
        yield return new WaitForSeconds(1f);
        targetHero.model.MoveSpd = originalSpeed;
    }

    private void FollowTarget(Vector3 destination)
    {
        float stopDistance = 0.5f;
        float distanceToTarget = Vector3.Distance(hero.transform.position, destination);

        if (distanceToTarget > stopDistance)
        {
            hero.agent.isStopped = false;
            hero.mov.SetDestination(destination, hero.model.MoveSpd);
        }
        else
        {
            hero.agent.isStopped = true;
            agent.ResetPath();
        }   
    }
}
