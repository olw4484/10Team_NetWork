using UnityEngine;

namespace Scripts.Scene
{
    public class TestScene : SceneBase
    {
        [Header("UI View")]
        [SerializeField] private LoginView loginView;

        public override SceneID SceneID => SceneID.TestScene;

        protected override void RegisterServices()
        {
            var authService = new FirebaseAuthService();
            YSJ_DIContainer.Instance.Register<IAuthService>(authService);
        }

        protected override void InitManagers()
        {
            // 필요한 매니저 초기화 (예: UIManager 등)
            Debug.Log(FirebaseManager.Instance.ToString());
        }

        protected override void InitUI()
        {
            var authService = YSJ_DIContainer.Instance.Resolve<IAuthService>();
            var presenter = new LoginPresenter(loginView, authService);
            loginView.Init(presenter);
        }

        protected override void Initialize()
        {
            // 게임 시작 직후 로직이 있다면 여기에
        }
    }
}
