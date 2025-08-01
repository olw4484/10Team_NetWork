using UnityEngine;

public interface IManager
{
    bool IsDontDestroy { get; }     // DontDestroy 체크
    void Initialize();              // 초기화
    void Cleanup();                 // 해제/정리
    GameObject GetGameObject();     // 소속 GameObject 반환
}