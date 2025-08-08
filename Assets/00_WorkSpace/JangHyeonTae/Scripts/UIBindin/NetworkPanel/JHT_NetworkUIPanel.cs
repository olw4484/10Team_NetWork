using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class JHT_NetworkUIPanel : YSJ_PanelBaseUI
{
    #region 로비
    private GameObject lobbyPanel => GetUI("LobbyPanel");
    private GameObject createRoomPanel => GetUI("CreateRoomPanel");
    private GameObject roomPanelItem => GetUI("RoomPanelItemParent");
    private Button createLobbyButton => GetUI<Button>("CreateLobbyButton");
    private Button createRoomButton => GetUI<Button>("CreateRoomButton");
    private Button secretButton => GetUI<Button>("SecretButton");
    private Button backButton => GetUI<Button>("BackButton");
    private TMP_InputField roomNameInput => GetUI<TMP_InputField>("RoomNameInput");
    private Image secretImage => GetUI<Image>("SecretButton");

    #endregion

    #region 룸
    private GameObject roomPanel => GetUI("RoomPanel");
    private Button leaveRoomButton => GetUI<Button>("LeaveRoomButton");
    private Button startButton => GetUI<Button>("StartButton");
    private Image redTeamPanel => GetUI<Image>("RedTeamPanel");
    private Image blueTeamPanel => GetUI<Image>("BlueTeamPanel");

    #endregion

    #region Character Select

    [SerializeField] private GameObject descPopUp;

    Color normalColor = Color.white;

    public Action OnChangedClick;
    public int curIndex = -1;
    #endregion

    #region 툴팁

    GameObject tool1 => GetUI("DescPopUp1");
    GameObject tool2 => GetUI("DescPopUp2");
    GameObject tool3 => GetUI("DescPopUp3");

    #endregion

    private bool isSecret;

    private JHT_RoomManager roomManager;     // Manager가 아닌 스크립트도 ManagerGroup.Instance.GetManager<JHT_RoomManager>()로 접급해야하는지
    private JHT_TeamManager teamManager;     // Manager가 아닌 스크립트도 ManagerGroup.Instance.GetManager<JHT_TeamManager>()로 접급해야하는지

    #region NetworkManager
    private GameObject loadingPanel => GetUI("LoadingPanel");
    
    private RectTransform roomListParent => GetUI<RectTransform>("RoomPanelItemParent");
    
    private JHT_NetworkManager networkManager;
    #endregion

    #region RoomManager
    private RectTransform playerRedPanelParent => GetUI<RectTransform>("PlayerListRedParent");
    private RectTransform playerBluePanelParent => GetUI<RectTransform>("PlayerListBlueParent");
    #endregion

    #region TeamManager


    #region Option Popup
    private Slider bgmSlider => GetUI<Slider>("BGMSlider");
    private Slider sfxSlider => GetUI<Slider>("SFXSlider");
    private TextMeshProUGUI bgmAmountText => GetUI<TextMeshProUGUI>("BGMAmountText");
    private TextMeshProUGUI sfxAmountText => GetUI<TextMeshProUGUI>("SFXAmountText");
    private GameObject optionPopUp => GetUI("OptionPopUp");

    private bool isBGMMute = false;
    private bool isSFXMute = false;

    private event Action<Slider> OnBGMSlider;
    private event Action<Slider> OnSFXSlider;
    #endregion

    YSJ_AudioManager audioManager;
    JHT_DataManager dataManager;
    private void Start()
    {

        #region audioSetting
        audioManager = ManagerGroup.Instance.GetManager<YSJ_AudioManager>();
        dataManager = ManagerGroup.Instance.GetManager<JHT_DataManager>();

        GetEvent("OptionButton").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["SimpleWood"]);
            if (!optionPopUp.activeSelf)
            {
                bgmSlider.value = audioManager.GetBgmVolume();
                sfxSlider.value = audioManager.GetSfxVolume();

                bgmAmountText.text = ((int)(bgmSlider.value * 100f)).ToString();
                sfxAmountText.text = ((int)(sfxSlider.value * 100f)).ToString();

                GetUI("BGMMuteImage").SetActive(isBGMMute);
                GetUI("SFXMuteImage").SetActive(isSFXMute);

                optionPopUp.SetActive(true);

                bgmSlider.onValueChanged.AddListener(bgmChanged);
                sfxSlider.onValueChanged.AddListener(sfxChanged);
                OnBGMSlider += ChangeBGMInteractable;
                OnSFXSlider += ChangeSFXIngeractable;
            }
        };


        GetEvent("OptionExitButton").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["MetalImpact"]);
            if (optionPopUp.activeSelf)
            {
                optionPopUp.SetActive(false);

                bgmSlider.onValueChanged.RemoveListener(bgmChanged);
                sfxSlider.onValueChanged.RemoveListener(sfxChanged);
                OnBGMSlider -= ChangeBGMInteractable;
                OnSFXSlider -= ChangeSFXIngeractable;
            }
        };


        GetEvent("BGMMuteButton").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["SimpleWood"]);
            isBGMMute = !isBGMMute;

            GetUI("BGMMuteImage").SetActive(isBGMMute);
            OnBGMSlider?.Invoke(bgmSlider);
            
        };

        GetEvent("SFXMuteButton").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["SimpleWood"]);
            isSFXMute = !isSFXMute;

            GetUI("SFXMuteImage").SetActive(isSFXMute);
            OnSFXSlider?.Invoke(sfxSlider);
        };
        #endregion
    }

    #region audioSetting
    private void ChangeBGMInteractable(Slider snd)
    {
        if (isBGMMute)
        {
            ManagerGroup.Instance.GetManager<YSJ_AudioManager>().MuteBgm();
            snd.interactable = false;
        }
        else
        {
            snd.interactable = true;
            ManagerGroup.Instance.GetManager<YSJ_AudioManager>().SetBgmVolume(snd.value);
        }
    }

    private void ChangeSFXIngeractable(Slider snd)
    {
        if (isSFXMute)
        {
            ManagerGroup.Instance.GetManager<YSJ_AudioManager>().MuteSfx();
            snd.interactable = false;
        }
        else
        {
            snd.interactable = true;
            ManagerGroup.Instance.GetManager<YSJ_AudioManager>().SetSfxVolume(snd.value);
        }
    }


    private void bgmChanged(float value)
    {
        if (!isBGMMute)
        {
            ManagerGroup.Instance.GetManager<YSJ_AudioManager>().SetBgmVolume(value);
            bgmAmountText.text = ((int)(value * 100)).ToString();
        }
    }

    private void sfxChanged(float value)
    {
        if (!isSFXMute)
        {
            ManagerGroup.Instance.GetManager<YSJ_AudioManager>().SetSfxVolume(value);
            sfxAmountText.text = ((int)(value * 100)).ToString();
        }
    }
    #endregion
    public void TeamInit()
    {
        teamManager = ManagerGroup.Instance.GetManager<JHT_TeamManager>();

        if (teamManager == null)
        {
            teamManager = FindObjectOfType<JHT_TeamManager>();
        }

        #region 팀 들어가고 나가기
        Color redBasicColor = redTeamPanel.color;
        Color blueBasicColor = blueTeamPanel.color;

        GetEvent("RedTeamPanel").Enter += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["SimpleWood"]);
            redTeamPanel.color = new Color(redTeamPanel.color.r, redTeamPanel.color.g, redTeamPanel.color.b, 0.4f);
        };
        GetEvent("RedTeamPanel").Exit += data => redTeamPanel.color = redBasicColor;

        GetEvent("BlueTeamPanel").Enter += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["SimpleWood"]);
            blueTeamPanel.color = new Color(blueTeamPanel.color.r, blueTeamPanel.color.g, blueTeamPanel.color.b, 0.4f);
        };
        GetEvent("BlueTeamPanel").Exit += data => blueTeamPanel.color = blueBasicColor;


        GetEvent("RedTeamPanel").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["Multipurpose"]);
            teamManager.OnRedSelect?.Invoke(PhotonNetwork.LocalPlayer);

        };

        GetEvent("BlueTeamPanel").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["Multipurpose"]);
            teamManager.OnBlueSelect?.Invoke(PhotonNetwork.LocalPlayer);
            Debug.Log($"{PhotonNetwork.LocalPlayer.ActorNumber} 블루버튼 클릭");
        };
        #endregion
    }
    #endregion
    public void NetInit()
    {
        networkManager = ManagerGroup.Instance.GetManager<JHT_NetworkManager>();

        if (networkManager == null)
            networkManager = FindObjectOfType<JHT_NetworkManager>();

        networkManager.OnLoading += AddLoading;
        networkManager.OnLobbyIn += AddLobby;
        networkManager.OnRoomIn += AddRoom;
        networkManager.OnParent += AddParent;

        #region 룸 버튼 이벤트
        
        GetEvent("CreateLobbyButton").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["SimpleWood"]);
            createRoomPanel.SetActive(true);
            roomPanelItem.SetActive(false);
            createLobbyButton.interactable = false;
        };

        GetEvent("SecretButton").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["Multipurpose"]);
            StartCoroutine(ButtonColorChange());
        };

        GetEvent("CreateRoomButton").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["SimpleWood"]);
            if (string.IsNullOrEmpty(roomNameInput.text))
            {
                return;
            }

            createRoomButton.interactable = false;
            roomPanelItem.SetActive(true);
            createRoomPanel.SetActive(false);

            RoomOptions options = new RoomOptions();
            int maxPlayer = 4;
            options.IsVisible = !isSecret;
            options.MaxPlayers = maxPlayer;

            PhotonNetwork.CreateRoom(roomNameInput.text, options);

            roomPanel.SetActive(true);
            createLobbyButton.interactable = true;
            createRoomButton.interactable = true;
            lobbyPanel.SetActive(false);
        };

        GetEvent("BackButton").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["MetalImpact"]);
            if (!createRoomPanel.activeSelf)
                return;

            createLobbyButton.interactable = true;
            roomPanelItem.SetActive(true);
            createRoomPanel.SetActive(false);
        };

        #endregion

        #region Character select
        for (int i = 0; i < networkManager.characters.Length; i++)
        {
            ChangeClick();
            GetUI<Image>($"CharacterPanel{i + 1}").sprite = networkManager.characters[i].icon;
        }
        Debug.Log($"{networkManager.characters[0].name}");

        GetEvent("CharacterPanel1").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["LeverDesigned"]);
            ChangeClick();
            GetUI<Image>("CharacterPanel1").color = Color.yellow;
            curIndex = 0;
            OnChangedClick?.Invoke();
            //YSJ_GameManager.Instance.playerName = character[0].name;
        };
        GetEvent("CharacterPanel2").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["LeverDesigned"]);
            ChangeClick();
            GetUI<Image>("CharacterPanel2").color = Color.yellow;
            curIndex = 1;
            OnChangedClick?.Invoke();
        };
        GetEvent("CharacterPanel3").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["LeverDesigned"]);
            ChangeClick();
            GetUI<Image>("CharacterPanel3").color = Color.yellow;
            curIndex = 2;
            OnChangedClick?.Invoke();
        };
        #endregion


        #region ToolTip for Character
        for (int i = 0; i < networkManager.characters.Length; i++)
        {
            GetUI($"DescPopUp{i + 1}").SetActive(false);
        }

        GetEvent("CharacterPanel1").Enter += data =>
        {
            GetUI("DescPopUp1").SetActive(true);

            GetUI("DescText1").GetComponent<TextMeshProUGUI>().text = networkManager.characters[0].desc;
            audioManager.PlaySfx(dataManager.audioDic["FireBurst"]);
        };

        GetEvent("CharacterPanel1").Exit += data =>
        {
            GetUI("DescPopUp1").SetActive(false);
        };

        GetEvent("CharacterPanel2").Enter += data =>
        {
            GetUI("DescPopUp2").SetActive(true);
            GetUI("DescText2").GetComponent<TextMeshProUGUI>().text = networkManager.characters[1].desc;
            audioManager.PlaySfx(dataManager.audioDic["FireBurst"]);
        };

        GetEvent("CharacterPanel2").Exit += data =>
        {
            GetUI("DescPopUp2").SetActive(false);
        };

        GetEvent("CharacterPanel3").Enter += data =>
        {
            GetUI("DescPopUp3").SetActive(true);
            GetUI("DescText3").GetComponent<TextMeshProUGUI>().text = networkManager.characters[2].desc;
            audioManager.PlaySfx(dataManager.audioDic["FireBurst"]);
        };

        GetEvent("CharacterPanel3").Exit += data =>
        {
            GetUI("DescPopUp3").SetActive(false);
        };
        #endregion
    }

    public void RoomInit()
    {
        roomManager = ManagerGroup.Instance.GetManager<JHT_RoomManager>();


        if (roomManager == null)
        {
            roomManager = FindObjectOfType<JHT_RoomManager>();
        }

        roomManager.OnStartButtonActive += StartButtonActive;
        roomManager.OnSetRedParent += RedParent;
        roomManager.OnSetBlueParent += BlueParent;

        GetEvent("StartButton").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["BigExplosion"]);
            roomManager.GameStart();
        };


        GetEvent("LeaveRoomButton").Click += data =>
        {
            audioManager.PlaySfx(dataManager.audioDic["MetalImpact"]);
            roomPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            roomManager.OnLeaveRoom?.Invoke();
        };

    }

    #region RoomManager Event

    private void StartButtonActive(bool value)
    {
        startButton.interactable = value;
    }

    private RectTransform RedParent()
    {
        return playerRedPanelParent;
    }

    private RectTransform BlueParent()
    {
        return playerBluePanelParent;
    }

    #endregion 

    #region NetworkManager Event
    private void AddLoading(bool value)
    {
        if (this == null || gameObject == null) return;

        if (loadingPanel.activeSelf)
            loadingPanel.SetActive(value);
            
    }

    private void AddLobby(bool value)
    {
        if (this == null || gameObject == null) return;

        lobbyPanel.SetActive(value);
    }

    private void AddRoom(bool value, bool value2 = false)
    {
        if (this == null || gameObject == null) return;

        roomPanel.SetActive(value);
        if (value2)
        {
            lobbyPanel.SetActive(!value2);
        }
    }

    private RectTransform AddParent()
    {
        return roomListParent;
    }
    #endregion

    private IEnumerator ButtonColorChange()
    {
        isSecret = !isSecret;
        yield return new WaitForSeconds(0.2f);
        secretImage.color = isSecret ? Color.red : Color.green;
    }

    private void ChangeClick()
    {
        for (int i = 0; i < networkManager.characters.Length; i++)
        {
            GetUI<Image>($"CharacterPanel{i + 1}").color = normalColor;
        }
    }
}
