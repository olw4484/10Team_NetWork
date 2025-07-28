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
                LGH_IDamagable damagable = collider.GetComponent<LGH_IDamagable>();
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
                    damagable.TakeDamage(skill_Q.damage);
                }
            }
            Debug.Log("BladeWind");
        }
    }
    #endregion

    public override void UseW()
    {
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
        StartCoroutine(BashRoutine());
    }

    private IEnumerator BashRoutine()
    {
        // 마우스 방향으로 돌진하고 경로상에 부딪힌 적에 데미지를 주고 적 Hero와 부딪히면 멈추는 스킬
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
                        Debug.Log("벽 또는 상대 영웅과 충돌 감지");
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
        // 사정거리 안의 Hero를 선택해서 공격 가능, 적에게 큰 데미지를 주고 이동속도를 1초동안 감소시키는 스킬
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

            // 타겟을 바라보게 설정
            Vector3 targetPos = new Vector3(hit.collider.transform.position.x, transform.position.y, hit.collider.transform.position.z);
            transform.forward = (targetPos - transform.position).normalized;

            // 데미지 부여
            damagable.TakeDamage(skill_R.damage);
            Debug.Log("Brutal Smite Hit");

            // 이동속도 감소 적용
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
