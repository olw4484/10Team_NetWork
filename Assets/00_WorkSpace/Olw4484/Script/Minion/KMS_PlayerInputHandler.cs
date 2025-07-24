using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS_PlayerInputHandler : MonoBehaviour
{
    [Header("���̾� ����")]
    public LayerMask selectionMask;  // ���� �̴Ͼ� ���ÿ�
    public LayerMask commandMask;    // ��� ���޿� (Ground, Enemy ��)
    public RectTransform dragBoxVisual; // �巡�׹ڽ�

    private Vector2 dragStartScreenPos;
    private bool isDragging = false;

    private List<MinionController> selectedMinions = new();

    void Update()
    {
        HandleSelectionInput();
        HandleCommandInput();
    }

    private void HandleSelectionInput()
    {
        // �巡�� ����
        if (Input.GetMouseButtonDown(0)) 
        {
            dragStartScreenPos = Input.mousePosition;
            isDragging = true;
            dragBoxVisual.gameObject.SetActive(true);
        }

        // �巡�� ����
        if (Input.GetMouseButton(0) && isDragging)
        {
            UpdateDragBoxVisual(dragStartScreenPos, Input.mousePosition);
        }

        // �巡�� 
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            dragBoxVisual.gameObject.SetActive(false);

            Vector2 dragEndScreenPos = Input.mousePosition;
            SelectUnitsInDragBox(dragStartScreenPos, dragEndScreenPos);
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
                IssueCommand(hit);
            }
        }
    }
    private Rect GetScreenRect(Vector2 start, Vector2 end)
    {
        Vector2 bottomLeft = Vector2.Min(start, end);
        Vector2 topRight = Vector2.Max(start, end);
        return new Rect(bottomLeft, topRight - bottomLeft);
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
            if (!minion.IsManual) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(minion.transform.position);
            if (selectionRect.Contains(screenPos, true))
            {
                selectedMinions.Add(minion);
                minion.SetSelected(true); // ���� ȿ��
            }
        }
    }

    private void IssueCommand(RaycastHit hit)
    {
        Debug.Log($"[Input] ��� �߼�: {hit.collider.gameObject.name} ��ġ {hit.point}");

        foreach (var minion in selectedMinions)
        {
            if (minion != null)
            {
                // ��ġ ��� ����
                minion.SetTarget(hit.transform);
            }
        }
    }
}
