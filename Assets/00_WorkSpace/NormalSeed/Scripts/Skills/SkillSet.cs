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
    public PhotonView pv;

    private void Start()
    {
        hero = GetComponent<HeroController>();
        agent = GetComponent<NavMeshAgent>();
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForPhotonViewReady());
    }

    private IEnumerator WaitForPhotonViewReady()
    {
        while (pv == null || !pv.IsMine)
        {
            pv = GetComponent<PhotonView>();
            yield return null; // 다음 프레임까지 대기
        }

        Debug.Log("PhotonView is ready and owned by this client.");
        // 여기서부터 안전하게 RPC 호출 가능
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
