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
        if (hero == null) return;

        if (Input.GetKeyDown(KeyCode.Q) && skillQCooldown == skillSet.skill_Q.cooldown)
        {
            skillSet.UseQ();
        }
        if (Input.GetKeyDown(KeyCode.W) && skillWCooldown == skillSet.skill_W.cooldown)
        {
            skillSet.UseW();
        }
        if (Input.GetKeyDown(KeyCode.E) && skillECooldown == skillSet.skill_E.cooldown)
        {
            Debug.Log("E키 입력 감지");

            skillSet.UseE();
        }
        if (Input.GetKeyDown(KeyCode.R) && skillRCooldown == skillSet.skill_R.cooldown)
        {
            skillSet.UseR();
        }

        if (skillSet.isQExecuted)
        {
            controller.model.CurMP.Value -= (int)skillSet.skill_Q.mana;
            Debug.Log($"현재 마나 : {controller.model.CurMP.Value}");
            skillQCooldown = 0f;
            skillSet.isQExecuted = false;
        }

        if (skillSet.isWExecuted)
        {
            controller.model.CurMP.Value -= (int)skillSet.skill_W.mana;
            Debug.Log($"현재 마나 : {controller.model.CurMP.Value}");
            skillWCooldown = 0f;
            skillSet.isWExecuted = false;
        }

        if (skillSet.isEExecuted)
        {
            controller.model.CurMP.Value -= (int)skillSet.skill_E.mana;
            Debug.Log($"현재 마나 : {controller.model.CurMP.Value}");
            skillECooldown = 0f;
            skillSet.isEExecuted = false;
        }

        if (skillSet.isRExecuted)
        {
            controller.model.CurMP.Value -= (int)skillSet.skill_R.mana;
            Debug.Log($"현재 마나 : {controller.model.CurMP.Value}");
            skillRCooldown = 0f;
            skillSet.isRExecuted = false;
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
