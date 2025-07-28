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
        //// ��� �÷��̾ �˻��� �� Photon�� �̿��ؼ� �ڽ��� Player�� ������ ��
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
        //// ī�޶��� �þ߰� ��������
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

        SetCameraOnPlayer();  // ù ��ġ ����
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

            // Y����� �����ٸ�
            if (isFollowing)
            {
                SetCameraOnPlayer();
            }

            // Y����� �����ְ� �����̽��ٸ� ������ �ִ� ���¶��
            if (!isFollowing && Input.GetKey(KeyCode.Space))
            {
                SetCameraOnPlayer();
            }

            // Y����� �����ְ� �����̽��ٸ� ������ ���� ���� ���¶��
            if (!isFollowing && !Input.GetKey(KeyCode.Space))
            {
                MoveCamera();
            }

            // ī�޶� ����/�ܾƿ�
            ZoomCamera();
        }
    }

    /// <summary>
    /// ī�޶� �÷��̾� ��ġ�� �̵���Ű�� �޼���
    /// </summary>
    void SetCameraOnPlayer()
    {
        transform.position = new Vector3(player.transform.position.x + 5f, 20f, player.transform.position.z - 5f);
    }

    /// <summary>
    /// ���콺 ��ġ�� ���� ī�޶� �̵���Ű�� �޼���
    /// </summary>
    void MoveCamera()
    {
        Vector3 pos = transform.position;
        // ���콺�� ��ġ�� ��ũ�� ������ screenBoarder ���� ���� ��ġ�� �� ī�޶� ��������
        if (Input.mousePosition.y >= Screen.height - screenBoarderThickness)
        {
            pos.x -= camSpd * Time.deltaTime;
            pos.z += camSpd * Time.deltaTime;
        }
        // �Ʒ���
        if (Input.mousePosition.y <= screenBoarderThickness)
        {
            pos.x += camSpd * Time.deltaTime;
            pos.z -= camSpd * Time.deltaTime;
        }
        // ����������
        if (Input.mousePosition.x >= Screen.width - screenBoarderThickness)
        {
            pos.x += camSpd * Time.deltaTime;
            pos.z += camSpd * Time.deltaTime;
        }
        // ��������
        if (Input.mousePosition.x <= screenBoarderThickness)
        {
            pos.x -= camSpd * Time.deltaTime;
            pos.z -= camSpd * Time.deltaTime;
        }

        transform.position = pos;
    }

    /// <summary>
    /// ī�޶� ����/�ܾƿ� �����ִ� �޼���
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
