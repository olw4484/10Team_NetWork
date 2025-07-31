using UnityEngine;

namespace Scripts.Scene
{
    public class InGameScene : SceneBase
    {
        public override SceneID SceneID => SceneID.InGameScene;

        [SerializeField] private InventoryHUDView inventoryView;

        protected override void Initialize()
        {
        }
    }
}
