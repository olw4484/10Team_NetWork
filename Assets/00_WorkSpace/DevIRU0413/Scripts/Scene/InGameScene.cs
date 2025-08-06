using UnityEngine;

namespace Scripts.Scene
{
    public class InGameScene : SceneBase
    {
        public override SceneID SceneID => SceneID.InGameScene;

        [Header("UI")]
        [SerializeField] private InventoryHUDView inventoryView;
        InventoryHUDPresenter inventoryHUDPresenter;

        protected override void Initialize()
        {
            base.Initialize();
            // InventoryUI();
        }

        private void InventoryUI()
        {
            LGH_TestGameManager gm = ManagerGroup.Instance.GetManager<LGH_TestGameManager>();
            Debug.Log($"{gm.gameObject} is Null. {gm != null}");
            if (gm != null)
            {
                InventoryHUDModel model = new(gm.localPlayer);
                Debug.Log($"{gm.gameObject} is Null. {gm != null}");
                inventoryHUDPresenter = new(inventoryView, model);
                inventoryView.InitBaseUI();
            }
        }
    }
}
