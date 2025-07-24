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
