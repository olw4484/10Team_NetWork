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
    /// �̵������� Raycast�� �����ؼ� �̵������� ���� �����̰� �ϴ� �޼���
    /// </summary>
    /// <param name="moveSpd"></param>
    public void GetMoveDestination(float moveSpd)
    {
        // ī�޶󿡼� ���� ���� Ground�� �¾��� �� �װ��� ��ǥ�������� ���� 
        RaycastHit hit;
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            SetDestination(hit.point, moveSpd);
        }
    }

    /// <summary>
    /// Hero�� �̵� ������ ������ �� �̵������� �˷��ִ� �޼���
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
    /// �̵������� �����Ǿ��� �� �̵������� �ٶ󺸵��� �ϴ� �޼���
    /// </summary>
    public void LookMoveDir()
    {
        // �̵� ������ �����Ǿ��� ��
        if (isMove)
        {
            // �̵��� �����ٸ�
            if (agent.velocity.magnitude == 0.0f)
            {
                isMove = false;
                return;
            }
        }

        // �̵����̶�� �̵������� �ٶ󺸵��� ����
        var dir = new Vector3(agent.steeringTarget.x, transform.position.y, agent.steeringTarget.z) - transform.position;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.forward = dir;
        }
    }
}
