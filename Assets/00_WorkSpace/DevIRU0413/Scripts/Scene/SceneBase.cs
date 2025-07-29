using UnityEngine;

namespace Scripts.Scene
{
    public abstract class SceneBase : MonoBehaviour
    {
        // 현재 씬의 고유 ID (상속 클래스에서 지정)
        public abstract SceneID SceneID { get; }

        protected virtual void Awake()
        {
            RegisterServices(); // 1단계: DI 등록
            InitManagers();     // 2단계: 매니저 초기화
            InitUI();           // 3단계: UI 연결 및 Presenter 주입
            Initialize();       // 4단계: 해당 씬 개별 초기화
        }

        protected abstract void RegisterServices();     // DI 등록
        protected abstract void InitManagers();         // 매니저 초기화
        protected abstract void InitUI();               // UI 연결 및 Presenter 주입
        protected abstract void Initialize();           // 개별 초기화
    }
}
