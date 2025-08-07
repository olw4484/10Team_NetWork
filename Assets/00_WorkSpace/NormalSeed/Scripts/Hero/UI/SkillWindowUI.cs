using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillWindowUI : MonoBehaviour
{
    [SerializeField] private List<Image> QSkillLevelIcons = new();
    [SerializeField] private List<Image> WSkillLevelIcons = new();
    [SerializeField] private List<Image> ESkillLevelIcons = new();
    [SerializeField] private List<Image> RSkillLevelIcons = new();

    public GameObject LevelUpButtons;

    public Button QLevelUpButton;
    public Button WLevelUpButton;
    public Button ELevelUpButton;
    public Button RLevelUpButton;

    public Image QShadow;
    public Image WShadow;
    public Image EShadow;
    public Image RShadow;

    private void Start()
    {
        PhotonView pv = GetComponentInParent<PhotonView>();
        if (!pv.IsMine)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void ActivateQSkillLevelIcon(int level)
    {
        QSkillLevelIcons[level - 1].enabled = true;
    }

    public void ActivateWSkillLevelIcon(int level)
    {
        WSkillLevelIcons[level - 1].enabled = true;
    }

    public void ActivateESkillLevelIcon(int level)
    {
        ESkillLevelIcons[level - 1].enabled = true;
    }

    public void ActivateRSkillLevelIcon(int level)
    {
        RSkillLevelIcons[level - 1].enabled = true;
    }

    public void ActivateSkillLevelUpButton()
    {
        LevelUpButtons.SetActive(true);
    }
    public void UnactivateSkillLevelUpButton()
    {
        LevelUpButtons.SetActive(false);
    }
}
