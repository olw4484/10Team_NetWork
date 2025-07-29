using Runtime.UI;
using System.Collections.Generic;
using UnityEngine;

public class YSJ_PopupController
{
    #region Constants

    private const int DESTROY_POPUP_INDEX_VALUE = -2;
    private const int POPUP_LAYER_INDEX_VALUE = (int)YSJ_UITypes.Popup;

    #endregion

    #region Fields

    private bool _isInit = false;
    private Canvas _popupCanvas;
    private readonly Stack<YSJ_PopupBaseUI> _popupStack = new();

    #endregion

    #region Init

    public void Init()
    {
        if (_isInit) return;

        _popupCanvas = YSJ_UIManager.Instance.GetCanvas(YSJ_UITypes.Popup);
        _isInit = _popupCanvas != null;
    }

    #endregion

    #region Register / Unregister

    public void Register(GameObject popup)
    {
        if (!_isInit) Init();

        if (popup == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("팝업 사용 불가 > (팝업 오브젝트 Null)");
#endif
            return;
        }

        YSJ_PopupBaseUI popupBase = popup.GetComponent<YSJ_PopupBaseUI>();
        if (popupBase == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("팝업 사용 불가 > (오브젝트에 UIPopupBase 없음)");
#endif
            return;
        }
        else if (popupBase.PopupIndex == DESTROY_POPUP_INDEX_VALUE)
        {
#if UNITY_EDITOR
            Debug.LogWarning("팝업 사용 불가 > (정상적으로 삭제가 안된 오브젝트)");
#endif
            Object.Destroy(popup);
            return;
        }

        popupBase.SetPopupIndex(POPUP_LAYER_INDEX_VALUE + _popupStack.Count);
        _popupStack.Push(popupBase);

        RefreshSiblingOrder();
    }

    public void Unregister(GameObject popup)
    {
        YSJ_PopupBaseUI popupBase = popup.GetComponent<YSJ_PopupBaseUI>();

        if (!IsTopPopup(popup))
        {
#if UNITY_EDITOR
            Debug.LogWarning("팝업 Unregister 실패: Stack의 최상단이 아님.");
#endif
            return;
        }

        popupBase.SetPopupIndex(DESTROY_POPUP_INDEX_VALUE);
        _popupStack.Pop();
        Object.Destroy(popup);

        RefreshSiblingOrder();
    }

    #endregion

    #region Close Methods

    public void CloseTop()
    {
        if (_popupStack.Count == 0)
            return;

        var popup = _popupStack.Pop();
        popup.Close();
    }

    public void CloseAll()
    {
        while (_popupStack.Count > 0)
            CloseTop();
    }

    #endregion

    #region Utility

    public int GetPopupCount() => _popupStack.Count;

    private void RefreshSiblingOrder()
    {
        var popups = _popupStack.ToArray();
        System.Array.Reverse(popups);

        for (int i = 0; i < popups.Length; i++)
        {
            popups[i].transform.SetSiblingIndex(i);
        }
    }

    public bool IsTopPopup(GameObject popup)
    {
        if (_popupStack.Count == 0 || popup == null)
            return false;

        var top = _popupStack.Peek();
        return top != null && top.gameObject == popup;
    }

    #endregion
}
