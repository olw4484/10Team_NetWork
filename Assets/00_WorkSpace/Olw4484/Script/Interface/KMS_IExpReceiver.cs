using Photon.Pun;


/// <summary>
/// ����ġ ���� �������̽�.
/// - AddExp: ����ġ �߰�, ����/����ȭ ��� ȣ��.
/// - GetCurrentExp/GetLevel: ��ȸ��.
/// - [PunRPC] RpcAddExp�� �ݵ�� ����ü(Ŭ����)���� ���� ����.
/// </summary>

public interface KMS_IExpReceiver
{
    void AddExp(int amount);
    int GetCurrentExp();
    int GetLevel();

    [PunRPC]
    public void RpcAddExp(int amount);
}
