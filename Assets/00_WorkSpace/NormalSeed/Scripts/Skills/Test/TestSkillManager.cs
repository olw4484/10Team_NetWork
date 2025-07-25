using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkillManager : MonoBehaviour
{
    [SerializeField] private GameObject hero;
    [SerializeField] private SkillSet skillSet;

    private void Start()
    {
        hero = GameObject.FindWithTag("Player");
        skillSet = hero.GetComponent<SkillSet>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            skillSet.UseQ();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            skillSet.UseW();
        }
    }
}
