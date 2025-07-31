using UnityEngine;

namespace Scripts.Scene
{
    public class InGameScene : SceneBase
    {
        public override SceneID SceneID => SceneID.InGameScene;

        [SerializeField] private InventoryHUDView inventoryView;

        InventoryHUDPresenter inventoryHUDPresenter;

        protected override void RegisterServices()
        {
            
        }

        protected override void InitManagers()
        {
            // 필요한 매니저 초기화 (예: UIManager 등)
            Debug.Log(YSJ_SystemManager.Instance.ToString());
        }

        protected override void Initialize()
        {
            YSJ_SystemManager.Instance.ResumeGame();
        }

        protected override void InitUI()
        {

            // inventoryHUDPresenter = new(inventoryView);
            inventoryView.Open();
        }
    }
}
