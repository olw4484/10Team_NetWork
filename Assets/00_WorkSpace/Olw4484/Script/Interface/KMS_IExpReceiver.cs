using Photon.Pun;


/// <summary>
/// 경험치 수령 인터페이스.
/// - AddExp: 경험치 추가, 로컬/동기화 모두 호출.
/// - GetCurrentExp/GetLevel: 조회용.
/// - [PunRPC] RpcAddExp는 반드시 구현체(클래스)에서 직접 구현.
/// </summary>

public interface KMS_IExpReceiver
{
    void AddExp(int amount);
    int GetCurrentExp();
    int GetLevel();

    [PunRPC]
    public void RpcAddExp(int amount);
}
