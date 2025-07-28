using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject[] players;
    private GameObject player;
    private bool isFollowing;
    public float camSpd;
    public float screenBoarderThickness;
    private float camFov;
    private float zoomSpd;
    private float mouseScrollInput;

    private void Start()
    {
        //// 모든 플레이어를 검색한 후 Photon을 이용해서 자신의 Player만 참조해 옴
        //players = GameObject.FindGameObjectsWithTag("Player");
        //foreach (GameObject obj in players)
        //{
        //    PhotonView pv = obj.GetComponent<PhotonView>();
        //    if (pv != null && pv.IsMine)
        //    {
        //        player = obj;
        //        break;
        //    }
        //}
        //isFollowing = true;
        //camSpd = 20f;
        //screenBoarderThickness = 10f;
        //zoomSpd = 10f;
        //// 카메라의 시야각 가져오기
        //camFov = gameObject.GetComponent<Camera>().fieldOfView;
        //transform.position = new Vector3(player.transform.position.x + 5f, 20, player.transform.position.z - 5f);
    }

    public void InitCamera(GameObject targetPlayer)
    {
        player = targetPlayer;
        isFollowing = true;
        camSpd = 20f;
        screenBoarderThickness = 10f;
        zoomSpd = 10f;
        camFov = GetComponent<Camera>().fieldOfView;

        SetCameraOnPlayer();  // 첫 위치 설정
    }

    private void Update()
    {
        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (isFollowing)
                {
                    isFollowing = false;
                }
                else
                {
                    isFollowing = true;
                }
            }

            // Y토글이 켜졌다면
            if (isFollowing)
            {
                SetCameraOnPlayer();
            }

            // Y토글이 꺼져있고 스페이스바를 누르고 있는 상태라면
            if (!isFollowing && Input.GetKey(KeyCode.Space))
            {
                SetCameraOnPlayer();
            }

            // Y토글이 꺼져있고 스페이스바를 누르고 있지 않은 상태라면
            if (!isFollowing && !Input.GetKey(KeyCode.Space))
            {
                MoveCamera();
            }

            // 카메라 줌인/줌아웃
            ZoomCamera();
        }
    }

    /// <summary>
    /// 카메라를 플레이어 위치로 이동시키는 메서드
    /// </summary>
    void SetCameraOnPlayer()
    {
        transform.position = new Vector3(player.transform.position.x + 5f, 20f, player.transform.position.z - 5f);
    }

    /// <summary>
    /// 마우스 위치에 따라 카메라를 이동시키는 메서드
    /// </summary>
    void MoveCamera()
    {
        Vector3 pos = transform.position;
        // 마우스의 위치가 스크린 위쪽의 screenBoarder 영역 내에 위치할 때 카메라를 위쪽으로
        if (Input.mousePosition.y >= Screen.height - screenBoarderThickness)
        {
            pos.x -= camSpd * Time.deltaTime;
            pos.z += camSpd * Time.deltaTime;
        }
        // 아래로
        if (Input.mousePosition.y <= screenBoarderThickness)
        {
            pos.x += camSpd * Time.deltaTime;
            pos.z -= camSpd * Time.deltaTime;
        }
        // 오른쪽으로
        if (Input.mousePosition.x >= Screen.width - screenBoarderThickness)
        {
            pos.x += camSpd * Time.deltaTime;
            pos.z += camSpd * Time.deltaTime;
        }
        // 왼쪽으로
        if (Input.mousePosition.x <= screenBoarderThickness)
        {
            pos.x -= camSpd * Time.deltaTime;
            pos.z -= camSpd * Time.deltaTime;
        }

        transform.position = pos;
    }

    /// <summary>
    /// 카메라를 줌인/줌아웃 시켜주는 메서드
    /// </summary>
    void ZoomCamera()
    {
        mouseScrollInput = Input.GetAxis("Mouse ScrollWheel");

        camFov -= mouseScrollInput * zoomSpd;
        camFov = Mathf.Clamp(camFov, 30, 60);
        gameObject.GetComponent<Camera>().fieldOfView
            = Mathf.Lerp(gameObject.GetComponent<Camera>().fieldOfView, camFov, zoomSpd);
    }
}
