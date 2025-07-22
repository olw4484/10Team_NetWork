using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    [SerializeField] Camera camera;

    private bool isMove;
    private Vector3 destination;

    private void Awake() => Init();

    private void Init()
    {
        camera = Camera.main;
    }

    public void GetMoveDestination()
    {
        // ī�޶󿡼� ���� ���� Ground�� �¾��� �� �װ��� ��ǥ�������� ���� 
        RaycastHit hit;
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            SetDestination(hit.point);
        }
    }

    public void SetDestination(Vector3 dest)
    {
        destination = dest;
        isMove = true;
    }

    public void Move(float moveSpd)
    {
        if (isMove)
        {
            var dir = destination - transform.position;
            // Hero�� position�� dir �������� �����Ӱ� ������� �����ð��� moveSpd��ŭ �̵�
            transform.position += dir.normalized * Time.deltaTime * moveSpd;
        }

        // �������� ����� ��������� �̵� ����
        if (Vector3.Distance(transform.position, destination) <= 0.1f)
        {
            isMove = false;
        }
    }
}
