using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkillSet : MonoBehaviour
{
    public SkillSO skill_Q;
    public SkillSO skill_W;
    public SkillSO skill_E;
    public SkillSO skill_R;

    protected Camera mainCam;
    protected HeroController hero;
    protected NavMeshAgent agent;

    private void Start()
    {
        hero = GetComponent<HeroController>();
        agent = GetComponent<NavMeshAgent>();
        mainCam = Camera.main;
    }

    public virtual void UseQ()
    {

    }

    public virtual void UseW()
    {
        
    }
    
    public virtual void UseE()
    {

    }

    public virtual void UseR()
    {

    }
}
