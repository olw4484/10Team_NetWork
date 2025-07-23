using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginFailPanel : MonoBehaviour
{
    [SerializeField] GameObject LoginPanel;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(Back);
    }

    private void Back()
    {
        LoginPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
