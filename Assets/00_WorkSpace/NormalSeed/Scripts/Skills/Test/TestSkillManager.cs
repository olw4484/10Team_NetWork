using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkillManager : MonoBehaviour
{
    [SerializeField] private GameObject hero;
    [SerializeField] private HeroController controller;
    [SerializeField] private SkillSet skillSet;
    [SerializeField] private float skillQCooldown;
    [SerializeField] private float skillWCooldown;
    [SerializeField] private float skillECooldown;
    [SerializeField] private float skillRCooldown;

    public void InitSkillManager(GameObject targetPlayer)
    {
        hero = targetPlayer;
        controller = hero.GetComponent<HeroController>();
        skillSet = hero.GetComponent<SkillSet>();
        skillQCooldown = skillSet.skill_Q.cooldown;
        skillWCooldown = skillSet.skill_W.cooldown;
        skillECooldown = skillSet.skill_E.cooldown;
        skillRCooldown = skillSet.skill_R.cooldown;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && skillQCooldown == skillSet.skill_Q.cooldown)
        {
            skillSet.UseQ();
            skillQCooldown = 0f;
        }
        if (Input.GetKeyDown(KeyCode.W) && skillWCooldown == skillSet.skill_W.cooldown)
        {
            skillSet.UseW();
            skillWCooldown = 0f;
        }
        if (Input.GetKeyDown(KeyCode.E) && skillECooldown == skillSet.skill_E.cooldown)
        {
            Debug.Log("E키 입력 감지");

            controller.mov.isMove = false;
            skillSet.UseE();
            skillECooldown = 0f;
        }
        if (Input.GetKeyDown(KeyCode.R) && skillRCooldown == skillSet.skill_R.cooldown)
        {
            skillSet.UseR();
            skillRCooldown = 0f;
        }

        if (hero != null)
        {
            if (skillQCooldown == 0f || skillQCooldown < skillSet.skill_Q.cooldown)
            {
                skillQCooldown += Time.deltaTime;
            }
            else if (skillQCooldown > skillSet.skill_Q.cooldown)
            {
                skillQCooldown = skillSet.skill_Q.cooldown;
            }

            if (skillWCooldown == 0f || skillWCooldown < skillSet.skill_W.cooldown)
            {
                skillWCooldown += Time.deltaTime;
            }
            else if (skillWCooldown > skillSet.skill_W.cooldown)
            {
                skillWCooldown = skillSet.skill_W.cooldown;
            }

            if (skillECooldown == 0f || skillECooldown < skillSet.skill_E.cooldown)
            {
                skillECooldown += Time.deltaTime;
            }
            else if (skillECooldown > skillSet.skill_E.cooldown)
            {   
                skillECooldown = skillSet.skill_E.cooldown;
            }

            if (skillRCooldown == 0f || skillRCooldown < skillSet.skill_R.cooldown)
            {
                skillRCooldown += Time.deltaTime;
            }
            else if (skillRCooldown > skillSet.skill_R.cooldown)
            {
                skillRCooldown = skillSet.skill_R.cooldown;
            }
        }
    }
}
