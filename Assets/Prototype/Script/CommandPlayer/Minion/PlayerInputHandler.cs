using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ISelectable;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("���̾� ����")]
    public LayerMask selectionMask;
    public LayerMask commandMask;
    public RectTransform dragBoxVisual;

    private Vector2 dragStartScreenPos;
    private bool isDragging = false;
    private bool isAttackMoveMode = false;
    private float dragThreshold = 10f;

    private ISelectable currentSelected;
    private List<BaseMinionController> selectedMinions = new();


    void Update()
    {
        HandleSelectionInput();
        HandleCommandInput();

        if (Input.GetKeyDown(KeyCode.A))
        {
            isAttackMoveMode = true;
        }
    }

    private void HandleSelectionInput()
    {
        // ���� �̵� ��� ó��
        if (isAttackMoveMode && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f, commandMask))
            {
                foreach (var minion in selectedMinions)
                {
                    minion.SetAttackMoveTarget(hit.point); // �Ǵ� hit.transform
                }
                isAttackMoveMode = false; // �� ���� ����
            }
        }

        // ���콺 ������ ��
        if (Input.GetMouseButtonDown(0))
        {
            dragStartScreenPos = Input.mousePosition;
            isDragging = false; // ������ false�� ����
        }

        // ���콺 �̵��ϸ鼭 �巡������ üũ
        if (Input.GetMouseButton(0))
        {
            if (!isDragging && Vector2.Distance(Input.mousePosition, dragStartScreenPos) > dragThreshold)
            {
                isDragging = true;
                dragBoxVisual.gameObject.SetActive(true);
            }
            if (isDragging)
            {
                UpdateDragBoxVisual(dragStartScreenPos, Input.mousePosition);
            }
        }

        // ���콺 ���� ��
        if (Input.GetMouseButtonUp(0))
        {
            dragBoxVisual.gameObject.SetActive(false);

            if (isDragging)
            {
                // �巡�׷� ���� ����
                SelectUnitsInDragBox(dragStartScreenPos, Input.mousePosition);
            }
            else
            {
                // ���� Ŭ�� ���� ó��
                HandleSingleClickSelection();
            }
            isDragging = false;
        }
    }

    private void UpdateDragBoxVisual(Vector2 start, Vector2 current)
    {
        Vector2 size = current - start;
        Vector2 center = start + size / 2;
        dragBoxVisual.position = center;
        dragBoxVisual.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
    }

    private void HandleCommandInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f, commandMask))
            {
                // HQ�� ���õ� ���¸� RallyPoint
                if (currentSelected is HQCommander hq)
                {
                    hq.SetRallyPoint(hit.point);
                }
                else if (selectedMinions.Count == 1)
                {
                    // ���� ������ ��
                    IssueCommand(hit);
                }
                // �̴Ͼ� ���� ���� ���õ� ���¸� IssueCommand�� �б� ó��
                else if (selectedMinions.Count > 1)
                {
                    IssueCommand(hit);
                }
            }
        }
    }

    private void SelectUnitsInDragBox(Vector2 start, Vector2 end)
    {
        Rect selectionRect = GetScreenRect(start, end);

        // ���� ���� ����
        foreach (var minion in selectedMinions)
        {
            minion?.SetSelected(false);
        }

        selectedMinions.Clear();

        foreach (var minion in FindObjectsOfType<MinionController>())
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(minion.transform.position);
            if (SelectionUtility.IsInSelectionBox(start, end, screenPos))
            {
                minion.Select();
                selectedMinions.Add(minion);
            }
            else
            {
                minion.Deselect();
            }
        }

        currentSelected = null; // �巡�״� HQ ���� ���
    }

    private Rect GetScreenRect(Vector2 start, Vector2 end)
    {
        Vector2 bottomLeft = Vector2.Min(start, end);
        Vector2 size = Vector2.Max(start, end) - bottomLeft;
        return new Rect(bottomLeft, size);
    }

    private void HandleSingleClickSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f, selectionMask))
        {
            Debug.Log($"Ray Hit: {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            var selectable = hit.collider.GetComponent<ISelectable>();
            if (selectable == null)
            {
                Debug.LogWarning($"[PlayerInputHandler] {hit.collider.name}���� ISelectable ������Ʈ�� ã�� ����");
            }
            else if (selectable != null)
            {
                currentSelected?.Deselect();
                currentSelected = selectable;
                currentSelected.Select();

                switch (selectable.GetSelectableType())
                {
                    case SelectableType.Minion:
                        Debug.Log("���� ���õ�");
                        break;
                    case SelectableType.Building:
                        Debug.Log("�ǹ� ���õ�");
                        break;
                }
            }
        }
    }

    private void IssueCommand(RaycastHit hit)
    {
        foreach (var minion in selectedMinions)
        {
            if (minion != null)
            {
                if (PhotonNetwork.InRoom)
                {
                    // �¶��� ������ ��� �����Լ�
                    if (hit.collider.CompareTag("Ground"))
                    {
                        minion.photonView.RPC("RpcMoveToPosition", RpcTarget.All, hit.point);
                    }
                    else // Ÿ�� (���� �� �� ����)
                    {
                        PhotonView targetView = hit.transform.GetComponent<PhotonView>();
                        if (hit.transform.TryGetComponent<IDamageable>(out var dmg))
                            minion.photonView.RPC("RpcSetTarget", RpcTarget.All, targetView.ViewID);
                    }
                }
                else
                {
                    // �������� ������ ��� ���� �Լ�
                    if (hit.collider.CompareTag("Ground"))
                        minion.MoveToPosition(hit.point);
                    else
                        minion.SetTarget(hit.transform);
                }
            }
        }
    }
}