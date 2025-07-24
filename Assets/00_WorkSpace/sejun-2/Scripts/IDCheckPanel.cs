using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IDCheckPanel : MonoBehaviour
{
    [SerializeField] GameObject signUpPanel;
    [SerializeField] TMP_Text idText;
    [SerializeField] Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(Back);
    }

    private void Back()
    {
        signUpPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
