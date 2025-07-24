using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSet : MonoBehaviour
{
    public SkillSO skill_Q;
    public SkillSO skill_W;
    public SkillSO skill_E;
    public SkillSO skill_R;

    protected Camera mainCam;
    protected HeroController hero;

    private void Start()
    {
        hero = GetComponent<HeroController>();
    }

    protected virtual void UseQ()
    {

    }

    protected virtual void UseW()
    {
        
    }
    
    protected virtual void UseE()
    {

    }

    protected virtual void UseR()
    {

    }
}
