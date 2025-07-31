using Photon.Pun;

public class JHT_SceneManager : YSJ_SimpleSingleton<JHT_SceneManager>
{
    public void AllPlayerChangeScene(string sceneName)
    {
        JHT_NetworkManager.NetworkInstance.characterParent.SetActive(false);
        JHT_NetworkManager.NetworkInstance.canvasPanel.gameObject.SetActive(false);

        PhotonNetwork.LoadLevel(sceneName);
    }
}
