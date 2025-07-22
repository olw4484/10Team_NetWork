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
        // 카메라에서 빛을 쏴서 Ground에 맞았을 때 그곳을 목표지점으로 설정 
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
            // Hero의 position을 dir 방향으로 프레임과 상관없이 단위시간당 moveSpd만큼 이동
            transform.position += dir.normalized * Time.deltaTime * moveSpd;
        }

        // 목적지에 충분히 가까워지면 이동 중지
        if (Vector3.Distance(transform.position, destination) <= 0.1f)
        {
            isMove = false;
        }
    }
}
