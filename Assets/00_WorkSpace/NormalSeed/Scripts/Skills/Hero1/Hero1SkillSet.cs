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
        // ���콺 ���⿡ ��ä�÷� �����ϴ� ��ų
        Vector3 originPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 attackDir;

        // Q��ų�� ��Ÿ���ŭ�� OverlapSphere�� �浹 �˻�
        Collider[] hits = Physics.OverlapSphere(originPos, skill_Q.skillRange);

        RaycastHit hit;
        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            // ���� ������ ���콺 ��ġ�� ����
            attackDir = (hit.point - originPos).normalized;
            // ���� ������ �ٶ󺸰� forward�� �ٲ���
            transform.forward = new Vector3(attackDir.x, transform.position.y, attackDir.z);

            foreach (Collider collider in hits)
            {
                LGH_IDamagable damagable = collider.GetComponent<LGH_IDamagable>();
                PhotonView view = collider.GetComponent<PhotonView>();

                // OverlapSphere ���� ������Ʈ�� damagable�� �ƴϰų� photonView�� ������ ���� �ʰų� photonView�� ���Ŷ�� ��ŵ
                if (damagable == null || view == null || view.IsMine) continue;

                object targetTeam, myTeam;
                // CustomProperty�� �κ�ȭ�鿡�� ���� ���������� �ؾ� �۵���
                if (view.Owner.CustomProperties.TryGetValue("Team", out targetTeam) && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out myTeam))
                {
                    if (targetTeam == myTeam) continue;
                }

                Vector3 colliderPos = new Vector3(collider.transform.position.x, 0, collider.transform.position.z);
                Vector3 toTarget = colliderPos - originPos;

                float angle = Vector3.Angle(attackDir, toTarget);
                // 60�� ������ ��ä�� ���� ���� ���� ������ ���̱� ������ ���� ���� ���� 30�� �̳��� ���� �˻�
                if (angle <= 30)
                {
                    // �ϴ� ������ ���� ���� �� ��ų ������ �ο�
                    view.RPC(nameof(HeroController.TakeDamage), RpcTarget.All, skill_Q.damage);
                }
            }
            Debug.Log("BladeWind");
        }
    }
    #endregion

    public override void UseW()
    {
        // 5�ʵ��� ������ �� ���ݷ��� 0.2�踸ŭ �÷��ִ� ��ų
        StartCoroutine(HardenRoutine());
    }

    private IEnumerator HardenRoutine()
    {
        
        hero.model.Def = hero.model.Def + hero.model.Atk * 0.2f;
        Debug.Log($"���� �ö�. ���� : {hero.model.Def}");
        yield return new WaitForSeconds(5f);
        hero.model.Def = hero.model.Def - hero.model.Atk * 0.2f;
        Debug.Log($"���� ���ư�. ���� : {hero.model.Def}");
    }

    public override void UseE()
    {
        Vector3 originPos = new Vector3(transform.position.x, 0, transform.position.z);
        RaycastHit hit;
        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            Vector3 dashDir = (hit.point - originPos).normalized;
            pv.RPC(nameof(RPC_StartBash), RpcTarget.All, dashDir);
            Debug.Log("RPC_StartBash ȣ���, dashDir: " + dashDir);
        }
    }

    private IEnumerator BashRoutine(Vector3 dashDir)
    {
        Debug.Log("BashRoutine ����!");

        // ���콺 �������� �����ϰ� ��λ� �ε��� ���� �������� �ְ� �� Hero�� ��ֹ��� �ε����� ���ߴ� ��ų

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
                LGH_IDamagable damagable = collider.GetComponent<LGH_IDamagable>();
                PhotonView view = collider.GetComponent<PhotonView>();
                bool isPlayerOrObstacle = collider.CompareTag("Player") || collider.CompareTag("Obstacle");

                // �ڽ��� collider���� ����
                if (collider.gameObject == gameObject) continue;

                // ���� �������� �� �� �ִ� �浹���� üũ
                if (damagable != null && view != null && !view.IsMine)
                {
                    object targetTeam, myTeam;
                    if (view.Owner.CustomProperties.TryGetValue("Team", out targetTeam) &&
                        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out myTeam))
                    {
                        if (targetTeam == myTeam) continue;
                    }

                    damagable.TakeDamage(skill_E.damage);
                    Debug.Log("Bash Hit");
                }

                // ���� �� �Ǵ� ��� ������ �浹�ߴٸ� ����
                if (isPlayerOrObstacle)
                {
                    Debug.Log("�� �Ǵ� ��� ������ �浹 ����");
                    hitDetected = true;
                    break;
                }
            }

            if (hitDetected)
            {
                //transform.position -= dashDir * 0.2f;
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
        // �����Ÿ� ���� Hero�� �����ؼ� ���� ����, ������ ū �������� �ְ� �̵��ӵ��� 1�ʵ��� ���ҽ�Ű�� ��ų
        StartCoroutine(BrutalSmiteRoutine());
    }

    private IEnumerator BrutalSmiteRoutine()
    {
        if (!TryGetValidTarget(out Transform target, out LGH_IDamagable damagable, out HeroController targetHero))
            yield break;

        while (true)
        {
            if (IsInSkillRange(target.position))
            {
                ExecuteSkill(target, damagable, targetHero);
                break;
            }
            else
            {
                FollowTarget(target.position);
            }

            yield return distCheck;
        }
    }

    private bool TryGetValidTarget(out Transform target, out LGH_IDamagable damagable, out HeroController targetHero)
    {
        target = null;
        damagable = null;
        targetHero = null;

        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            return false;

        damagable = hit.collider.GetComponent<LGH_IDamagable>();
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

    private void ExecuteSkill(Transform target, LGH_IDamagable damagable, HeroController targetHero)
    {
        hero.mov.ExecuteAttack(target, damagable, skill_R.damage);

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
