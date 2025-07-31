using UnityEngine;

namespace Scripts.Scene
{
    public class TestScene : SceneBase
    {
        public override SceneID SceneID => SceneID.TestScene;

        [SerializeField] private LoginView loginView;
        
        protected override void Initialize()
        {
        }
    }
}
