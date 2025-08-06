using Photon.Pun;


public class JHT_SceneManager : YSJ_SimpleSingleton<JHT_SceneManager>
{
    public void AllPlayerChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
