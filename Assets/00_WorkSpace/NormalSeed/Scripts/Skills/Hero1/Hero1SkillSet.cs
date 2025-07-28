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
                    damagable.TakeDamage(skill_Q.damage);
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
        StartCoroutine(BashRoutine());
    }

    private IEnumerator BashRoutine()
    {
        // ���콺 �������� �����ϰ� ��λ� �ε��� ���� �������� �ְ� �� Hero�� �ε����� ���ߴ� ��ų
        Vector3 originPos = new Vector3(transform.position.x, 0, transform.position.z);
        Collider heroCollider = hero.GetComponent<Collider>();
        RaycastHit hit;

        agent.isStopped = true;
        agent.enabled = false;

        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            Vector3 dashDir = (hit.point - originPos).normalized;
            float dashSpeed = 10f;
            float dashDuration = 0.5f;
            float timer = 0f;

            heroCollider.isTrigger = true;

            transform.forward = new Vector3(dashDir.x, 0, dashDir.z);

            while (timer < dashDuration)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
                bool hitDetected = false;

                foreach (Collider collider in hits)
                {
                    LGH_IDamagable damagable = collider.GetComponent<LGH_IDamagable>();
                    PhotonView view = collider.GetComponent<PhotonView>();

                    if (damagable == null || view == null || view.IsMine) continue;

                    object targetTeam, myTeam;
                    if (view.Owner.CustomProperties.TryGetValue("Team", out targetTeam) &&
                        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out myTeam))
                    {
                        if (targetTeam == myTeam) continue;
                    }

                    damagable.TakeDamage(skill_E.damage);
                    Debug.Log("Bash Hit");

                    if (collider.gameObject.CompareTag("Hero") || collider.gameObject.CompareTag("Ground"))
                    {
                        Debug.Log("�� �Ǵ� ��� ������ �浹 ����");
                        hitDetected = true;
                        break;
                    }
                }

                if (hitDetected)
                {
                    transform.position -= dashDir * 0.2f;
                    dashSpeed = 0f;
                    yield break;
                }

                timer += Time.deltaTime;
                transform.position += dashDir * dashSpeed * Time.deltaTime;
                yield return null;
            }

            agent.enabled = true;
            agent.isStopped = false;
            agent.ResetPath();
            heroCollider.isTrigger = false;
        }
    }

    public override void UseR()
    {
        // �����Ÿ� ���� Hero�� �����ؼ� ���� ����, ������ ū �������� �ְ� �̵��ӵ��� 1�ʵ��� ���ҽ�Ű�� ��ų
        StartCoroutine(BrutalSmiteRoutine());
    }

    private IEnumerator BrutalSmiteRoutine()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            LGH_IDamagable damagable = hit.collider.GetComponent<LGH_IDamagable>();
            PhotonView view = hit.collider.GetComponent<PhotonView>();

            if (damagable == null || view == null || view.IsMine) yield break;

            object targetTeam, myTeam;
            if (view.Owner.CustomProperties.TryGetValue("Team", out targetTeam) &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out myTeam))
            {
                if (targetTeam == myTeam) yield break;
            }

            // Ÿ���� �ٶ󺸰� ����
            Vector3 targetPos = new Vector3(hit.collider.transform.position.x, transform.position.y, hit.collider.transform.position.z);
            transform.forward = (targetPos - transform.position).normalized;

            // ������ �ο�
            damagable.TakeDamage(skill_R.damage);
            Debug.Log("Brutal Smite Hit");

            // �̵��ӵ� ���� ����
            HeroController targetHero = hit.collider.GetComponent<HeroController>();
            if (targetHero != null)
            {
                float originalSpeed = targetHero.model.MoveSpd;
                targetHero.model.MoveSpd *= 0.5f;
                yield return new WaitForSeconds(1f);
                targetHero.model.MoveSpd = originalSpeed;
            }
        }
    }

}
