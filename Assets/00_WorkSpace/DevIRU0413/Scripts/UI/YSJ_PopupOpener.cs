using UnityEngine;

namespace Runtime.UI
{
    public class YSJ_PopupOpener : MonoBehaviour
    {
        [Tooltip("Start 시 자동으로 팝업 열지 여부")]
        public bool m_isStartOpenPopup = false;

        [Tooltip("열릴 팝업 프리팹")]
        public GameObject popupPrefab;

        protected virtual void Start()
        {
            if (m_isStartOpenPopup)
                OpenPopup();
        }

        public virtual void OpenPopup()
        {
            if (popupPrefab == null)
            {
                Debug.LogWarning("[PopupOpener] popupPrefab이 설정되지 않았습니다.");
                return;
            }

            // 캔버스 매니저 또는 UIManager에서 부모 가져오기
            var popupCanvas = YSJ_UIManager.Instance?.GetCanvas(YSJ_UIType.Popup);
            if (popupCanvas == null)
            {
                Debug.LogError("[PopupOpener] PopupRoot가 UIManager에 설정되어 있지 않습니다.");
                return;

            }

            var popup = YSJ_UISpawnFactory.SpawnPopup(popupPrefab);
        }
    }
}
