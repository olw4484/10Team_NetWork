using Photon.Pun;
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

    private WaitForSeconds distCheck = new WaitForSeconds(2f);

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
            if (hit.collider.CompareTag("Ground"))
            {
                SetDestination(hit.point, moveSpd);
            }
        }
    }

    /// <summary>
    /// ����� �⺻���� �޼���
    /// </summary>
    /// <param name="moveSpd"></param>
    /// <param name="damage"></param>
    /// <param name="atkRange"></param>
    public void HeroAttack(float moveSpd, int damage, float atkRange)
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            LGH_IDamagable damagable = hit.collider.GetComponent<LGH_IDamagable>();
            PhotonView view = hit.collider.GetComponent<PhotonView>();

            if (damagable != null && view != null && !view.IsMine)
            {
                object targetTeam, myTeam;
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out myTeam) &&
                    view.Owner.CustomProperties.TryGetValue("Team", out targetTeam))
                {
                    if (!targetTeam.Equals(myTeam))
                    {
                        // ���� ��Ÿ� �ȿ� �ִٸ� �⺻���� ����(�ִϸ��̼� ���, TakeDamage�� ������ ��)
                        StartCoroutine(HeroAttackRoutine(hit.collider.transform, damagable, atkRange, damage, moveSpd));
                        return;
                    }
                }
            }
        }
    }

    private IEnumerator HeroAttackRoutine(Transform target, LGH_IDamagable damagable, float atkRange, int damage, float moveSpd)
    {
        while (true)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= atkRange)
            {
                // TakeDamage�� RPC�� ������ ��
                damagable.TakeDamage(damage);
                Debug.Log("Hero1 �⺻ ����");
                yield break;
            }
            else
            {
                SetDestination(target.position, moveSpd);
            }

            yield return distCheck;
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
