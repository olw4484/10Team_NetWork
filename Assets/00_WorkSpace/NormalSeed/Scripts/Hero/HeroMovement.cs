using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class HeroMovement : MonoBehaviour
{
    [SerializeField] public Camera camera;
    private NavMeshAgent agent;

    private bool isMove;
    private Vector3 destination;

    private void Awake() => Init();

    private void Init()
    {
        camera = Camera.main;
        agent = GetComponent<NavMeshAgent>();

        agent.acceleration = 100f;
        agent.angularSpeed = 720f;
        agent.stoppingDistance = 0.5f;
    }

    /// <summary>
    /// 이동지점을 Raycast로 지정해서 이동지점을 향해 움직이게 하는 메서드
    /// </summary>
    /// <param name="moveSpd"></param>
    public void GetMoveDestination(float moveSpd)
    {
        // 카메라에서 빛을 쏴서 Ground에 맞았을 때 그곳을 목표지점으로 설정 
        RaycastHit hit;
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            SetDestination(hit.point, moveSpd);
        }
    }

    /// <summary>
    /// Hero의 이동 지점을 결정한 후 이동중임을 알려주는 메서드
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="moveSpd"></param>
    public void SetDestination(Vector3 dest, float moveSpd)
    {
        agent.speed = moveSpd;
        agent.SetDestination(dest);
        destination = dest;
        isMove = true;
    }

    /// <summary>
    /// 이동지점이 설정되었을 때 이동방향을 바라보도록 하는 메서드
    /// </summary>
    public void LookMoveDir()
    {
        // 이동 지점이 설정되었을 때
        if (isMove)
        {
            // 이동이 끝난다면
            if (agent.velocity.magnitude == 0.0f)
            {
                isMove = false;
                return;
            }
        }

        // 이동중이라면 이동방향을 바라보도록 설정
        var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.forward = dir;
        }
    }
}
