using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGH_TestGameManager : MonoBehaviourPunCallbacks
{
    public CameraController camController;
    public TestSkillManager skillManager;

    public override void OnJoinedRoom()
    {
        StartCoroutine(WaitForLocalPlayer());
    }

    private IEnumerator WaitForLocalPlayer()
    {
        GameObject localPlayer = null;
        while (localPlayer == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject obj in players)
            {
                PhotonView pv = obj.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    localPlayer = obj;
                    break;
                }
            }
            yield return null;
        }

        camController.InitCamera(localPlayer);  // 플레이어 전달
        skillManager.InitSkillManager(localPlayer);
    }

}
