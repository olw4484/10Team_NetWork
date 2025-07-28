using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkillManager : MonoBehaviour
{
    [SerializeField] private GameObject hero;
    [SerializeField] private HeroController controller;
    [SerializeField] private SkillSet skillSet;

    private void Start()
    {
        hero = GameObject.FindWithTag("Player");
        controller = hero.GetComponent<HeroController>();
        skillSet = hero.GetComponent<SkillSet>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            controller.mov.isMove = false;
            skillSet.UseQ();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            controller.mov.isMove = false;
            skillSet.UseW();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            controller.mov.isMove = false;
            skillSet.UseE();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            controller.mov.isMove = false;

        }
    }
}
