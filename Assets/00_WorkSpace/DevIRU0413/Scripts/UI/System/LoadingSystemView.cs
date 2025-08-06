using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LoadingSystemView : YSJ_SystemBaseUI, ILoadingUI

{
    enum LoadingSystemEnum
    {
        Loading_Slider,
        LoadingLog_Text,
    }

    private UIBinder<LoadingSystemEnum> _uiBinder;
    
    public void Init()
    {
        InitBaseUI();
        _uiBinder = new(this);
        _uiBinder.Get<Slider>(LoadingSystemEnum.Loading_Slider).value = 0.0f;
        _uiBinder.Get<TextMeshProUGUI>(LoadingSystemEnum.LoadingLog_Text).text = "Initialized Loading Log";
    }

    public void Show(string message)
    {
        this.gameObject.SetActive(true);
        Open();
        _uiBinder.Get<Slider>(LoadingSystemEnum.Loading_Slider).value = 0.0f;

        UpdateMessage(message);
    }

    public void Hide()
    {
        if (this == null || gameObject == null)
            return;

        Close();
        gameObject.SetActive(false);

        Debug.Log("하이드");
    }

    public void UpdateMessage(string message)
    {
        _uiBinder.Get<TextMeshProUGUI>(LoadingSystemEnum.LoadingLog_Text).text = message;

    }

    public float GetProgress()
    {
        return _uiBinder.Get<Slider>(LoadingSystemEnum.Loading_Slider).value;
    }

    public void SetProgress(float value)
    {
        _uiBinder.Get<Slider>(LoadingSystemEnum.Loading_Slider).value = value;
    }
}
