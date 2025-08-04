using Photon.Pun;
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

    public bool isQExecuted = false;
    public bool isWExecuted = false;
    public bool isEExecuted = false;
    public bool isRExecuted = false;

    protected Camera mainCam;
    protected HeroController hero;
    protected NavMeshAgent agent;
    protected PhotonView pv;

    private void Start()
    {
        hero = GetComponent<HeroController>();
        agent = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();
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
