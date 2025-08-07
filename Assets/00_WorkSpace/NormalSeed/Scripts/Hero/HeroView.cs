using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroView : MonoBehaviour
{
    public Animator animator;
    [SerializeField] public HeroHPBarUI hpBarUI;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        hpBarUI = GetComponentInChildren<HeroHPBarUI>();
    }

    // HeroController에서 설정한 애니메이션 hash값을 가져옴
    [PunRPC]
    public void PlayAnimation(int hash)
    {
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.Play(hash, 0, 0f);
        }
    }

    /// <summary>
    /// HP 바 UI 최신화 메서드
    /// </summary>
    /// <param name="maxHp"></param>
    /// <param name="curHp"></param>
    public void SetHpBar(float maxHp, float curHp)
    {
        hpBarUI.hpImage.fillAmount = Mathf.Clamp01(curHp / maxHp);
    }

    public void SetMpBar(float maxMp, float curMp)
    {
        hpBarUI.mpImage.fillAmount = Mathf.Clamp01(curMp / maxMp);
    }
}
