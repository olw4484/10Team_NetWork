using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JHT_CharacterPanel : YSJ_PanelBaseUI
{

    [SerializeField] private GameObject descPopUp;
    public JHT_Character[] character;

    Color nomalColor = Color.white;
    private void Start()
    {
        for (int i = 0; i < character.Length; i++)
        {
            GetUI<Image>($"CharacterPanel{i + 1}").GetComponent<Image>().sprite = character[i].icon;
        }

        GetEvent("CharacterPanel1").Click += data =>
        {
            //YSJ_GameManager.Instance.playerName = character[0].name;
        };
        GetEvent("CharacterPanel2").Click += data =>
        {
            //YSJ_GameManager.Instance.playerName = character[1].name;
        };
        GetEvent("CharacterPanel3").Click += data =>
        {
            //YSJ_GameManager.Instance.playerName = character[2].name;
        };

        GetEvent("CharacterPanel1").Enter += data =>
        {
            GetUI<Image>("CharacterPanel1").GetComponent<Image>().color = Color.yellow;
            //GameObject obj = YSJ_UISpawnFactory.ShowPopup(descPopUp);
            //obj.GetComponent<JHT_DescPopUp>().Init(character[0].desc,new Vector2(117,-120));
        };

        GetEvent("CharacterPanel1").Exit += data =>
        {
            GetUI<Image>("CharacterPanel1").GetComponent<Image>().color = nomalColor;
        };



        GetEvent("CharacterPanel2").Enter += data =>
        {
            GetUI<Image>("CharacterPanel2").GetComponent<Image>().color = Color.yellow;
            //GameObject obj = YSJ_UISpawnFactory.ShowPopup(descPopUp);
            //obj.GetComponent<JHT_DescPopUp>().Init(character[1].desc,new Vector2(340.7816f,-120));
        };

        GetEvent("CharacterPanel2").Exit += data =>
        {
            GetUI<Image>("CharacterPanel2").GetComponent<Image>().color = nomalColor;
        };



        GetEvent("CharacterPanel3").Enter += data =>
        {
            GetUI<Image>("CharacterPanel3").GetComponent<Image>().color = Color.yellow;
            //GameObject obj = YSJ_UISpawnFactory.ShowPopup(descPopUp);
            //obj.GetComponent<JHT_DescPopUp>().Init(character[2].desc,new Vector2(564.5632f,-120));
        };

        GetEvent("CharacterPanel3").Exit += data =>
        {
            GetUI<Image>("CharacterPanel3").GetComponent<Image>().color = nomalColor;
        };
    }
}
