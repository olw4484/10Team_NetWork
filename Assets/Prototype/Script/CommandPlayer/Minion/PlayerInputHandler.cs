using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ISelectable;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("레이어 설정")]
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
        // 공격 이동 명령 처리
        if (isAttackMoveMode && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f, commandMask))
            {
                foreach (var minion in selectedMinions)
                {
                    minion.SetAttackMoveTarget(hit.point); // 또는 hit.transform
                }
                isAttackMoveMode = false; // 한 번만 실행
            }
        }

        // 마우스 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            dragStartScreenPos = Input.mousePosition;
            isDragging = false; // 무조건 false로 시작
        }

        // 마우스 이동하면서 드래그인지 체크
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

        // 마우스 뗐을 때
        if (Input.GetMouseButtonUp(0))
        {
            dragBoxVisual.gameObject.SetActive(false);

            if (isDragging)
            {
                // 드래그로 영역 선택
                SelectUnitsInDragBox(dragStartScreenPos, Input.mousePosition);
            }
            else
            {
                // 단일 클릭 선택 처리
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
                // HQ가 선택된 상태면 RallyPoint
                if (currentSelected is HQCommander hq)
                {
                    hq.SetRallyPoint(hit.point);
                }
                else if (selectedMinions.Count == 1)
                {
                    // 단일 선택일 때
                    IssueCommand(hit);
                }
                // 미니언 여러 마리 선택된 상태면 IssueCommand로 분기 처리
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

        // 기존 선택 해제
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

        currentSelected = null; // 드래그는 HQ 선택 취소
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
                Debug.LogWarning($"[PlayerInputHandler] {hit.collider.name}에서 ISelectable 컴포넌트를 찾지 못함");
            }
            else if (selectable != null)
            {
                currentSelected?.Deselect();
                currentSelected = selectable;
                currentSelected.Select();

                switch (selectable.GetSelectableType())
                {
                    case SelectableType.Minion:
                        Debug.Log("유닛 선택됨");
                        break;
                    case SelectableType.Building:
                        Debug.Log("건물 선택됨");
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
                    // 온라인 상태일 경우 포톤함수
                    if (hit.collider.CompareTag("Ground"))
                    {
                        minion.photonView.RPC("RpcMoveToPosition", RpcTarget.All, hit.point);
                    }
                    else // 타겟 (적이 될 수 있음)
                    {
                        PhotonView targetView = hit.transform.GetComponent<PhotonView>();
                        if (hit.transform.TryGetComponent<IDamageable>(out var dmg))
                            minion.photonView.RPC("RpcSetTarget", RpcTarget.All, targetView.ViewID);
                    }
                }
                else
                {
                    // 오프라인 상태일 경우 로컬 함수
                    if (hit.collider.CompareTag("Ground"))
                        minion.MoveToPosition(hit.point);
                    else
                        minion.SetTarget(hit.transform);
                }
            }
        }
    }
}