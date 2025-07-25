using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero1SkillSet : SkillSet
{
    //public SkillSO skill_Q;
    //public SkillSO skill_W;
    //public SkillSO skill_E;
    //public SkillSO skill_R;

    // protected Camera mainCam;
    //protected HeroController hero;

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

    public override void UseW()
    {
        // 5�ʵ��� ������ �� ���ݷ��� 0.2�踸ŭ �÷��ִ� ��ų
        StartCoroutine(BashRoutine());
    }

    private IEnumerator BashRoutine()
    {
        
        hero.model.Def = hero.model.Def + hero.model.Atk * 0.2f;
        Debug.Log($"���� �ö�. ���� : {hero.model.Def}");
        yield return new WaitForSeconds(5f);
        hero.model.Def = hero.model.Def - hero.model.Atk * 0.2f;
        Debug.Log($"���� ���ư�. ���� : {hero.model.Def}");
    }

    public override void UseE()
    {
        // ���콺 �������� �����ϰ� ��λ� �ε��� ���� �������� �ְ� �� Hero�� �ε����� ���ߴ� ��ų
    }

    public override void UseR()
    {
        // �����Ÿ� ���� Hero�� �����ؼ� ���� ����, ������ ū �������� �ְ� �̵��ӵ��� 1�ʵ��� ���ҽ�Ű�� ��ų
    }
}
