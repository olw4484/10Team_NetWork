using System.Collections;
using UnityEngine;


public class SHI_ItemManager : MonoBehaviour
{
    public static SHI_ItemManager instance;
    [SerializeField] GameObject player; // �÷��̾� ������Ʈ�� �����ϱ� ���� ����
    float Thp =0; // �������� ������Ű�� HP ��
    float Tmp =0; // �������� ������Ű�� MP ��
    float Tatk = 0; // �������� ������Ű�� ���ݷ� ��
    float TcoolDown = 1; // �������� ������Ű�� ��Ÿ�� ���� ��
    float TmoveSpeed = 0; // �������� ������Ű�� �̵� �ӵ� ��
    float TatkSpeed = 0; // �������� ������Ű�� ���� �ӵ� ��
    float TcritChance = 0; // �������� ������Ű�� ġ��Ÿ Ȯ�� ��
    float TskillPower = 0; // �������� ������Ű�� ��ų ���ݷ� ��
    float TrangeUp = 0; // �������� ������Ű�� ��Ÿ� ��
    float TlifeSteal = 0; // �������� ������Ű�� ����� ��� ��
    float TminionHitup = 0; // �������� ������Ű�� �̴Ͼ� ���ݷ� ��
    float TminionHitup2 = 1; // �������� ������Ű�� �̴Ͼ� ���ݷ� ��2 (�߰� ������ ��� ����)
    float TnexusHitUp = 1; // �������� ������Ű�� �ؼ��� ���ݷ� ��
    float Tallup = 1; // �������� ������Ű�� ��ü ���� ��
    float TminionDamageDown = 1; // �������� ���� ��Ű�� �̴Ͼ� ���ݷ� ��
    float TminionAttackRegeneration =0;// �������� ������Ű�� �̴Ͼ� ���ݽ� ��� �ӵ� ����
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void UseItem(SHI_ItemBase item)
    {
        if (item.type == 0) // �Һ� ������
        {
            //if (����ü�� <= 0) // �÷��̾��� hp�� 0�϶��� ���Ұ� Ȥ�� �÷��̾� ���̵� �Ұ� �޾ƿ�
            //{
                //Debug.Log("�÷��̾��� HP�� 0�̹Ƿ� �������� ����� �� �����ϴ�.");
                //return;
            //}
            
            HpUp(item.Healhp); // �÷��̾��� HP ȸ��
            MpUp(item.Healmp); // �÷��̾��� MP ȸ��
            ExpUp(item.GainExp); // �÷��̾��� ����ġ ����
            if (item.lifeSteal > 0 || item.nexusHitUp > 1 || item.minionHitup > 1)
            {
                Buff(item); // �������� ���� ȿ�� ����
            }
        }

        else if (item.type == 1) // ��� ������
        {
            StatUp(item); // �÷��̾��� ������ �������� �������� ����
        }
       
    }
    void HpUp(float hp)
    {
        //�÷��̾� ����hp += item.Healhp; // �÷��̾��� HP ȸ��
        //if(�÷��̾� �ִ�HP < �÷��̾� ����hp)
        {
            //�÷��̾� ����hp = �÷��̾� �ִ�HP; // �÷��̾��� HP�� �ִ�ġ�� ���� �ʵ��� ����
        }

    }
    void MpUp(float mp)
    {
        //�÷��̾� ����mp += item.Healmp; // �÷��̾��� MP ȸ��
        //if(�÷��̾� �ִ�MP < �÷��̾� ����mp)
        {
            //�÷��̾� ����mp = �÷��̾� �ִ�MP; // �÷��̾��� MP�� �ִ�ġ�� ���� �ʵ��� ����
        }
    }
    void ExpUp(float exp)
    {
        //�÷��̾� ����exp += item.GainExp; // �÷��̾��� ����ġ ����
        // exp�� level up �Լ����� ���� ������ �߰���.
    }
    void Buff(SHI_ItemBase item)
    {
        //���� �÷��̾��� atk
        TlifeSteal += item.lifeSteal; // ����� ��� ����
        TminionHitup2 *= (item.minionHitup+TminionHitup); // �̴Ͼ� ���ݷ� ����
        TnexusHitUp *= item.nexusHitUp; // �ؼ��� ���ݷ� ����
        Tallup *= item.allup; // ��ü ���� ����
        StartCoroutine(BuffDuration(item)); // ���� ���� �ð� ���� ȿ�� ����
    }
    void StatUp(SHI_ItemBase item)
    {
        Thp += item.hp; // �÷��̾��� HP ����
        Tmp += item.mp; // �÷��̾��� MP ����
        Tatk += item.atk; // �÷��̾��� ���ݷ� ����
        TcoolDown -= item.coolDown; // �÷��̾��� ��Ÿ�� ����
        TmoveSpeed += item.moveSpeed; // �÷��̾��� �̵� �ӵ� ����
        TatkSpeed += item.atkSpeed; // �÷��̾��� ���� �ӵ� ����
        TcritChance += item.critChance; // �÷��̾��� ġ��Ÿ Ȯ�� ����
        TskillPower += item.skillPower; // �÷��̾��� ��ų ���ݷ� ����
        TrangeUp += item.rangeUp; // �÷��̾��� ��Ÿ� ����
        TminionDamageDown -= item.minionDamageDown; // �÷��̾��� �̴Ͼ� ���ݷ� ����
        TminionHitup += item.minionHitup; // �÷��̾��� �̴Ͼ���ݽ� ���ݷ� ����
        TminionAttackRegeneration += item.minionAttackRegeneration; // �÷��̾��� �̴Ͼ� ���ݽ� ��� �ӵ� ����
    }
     IEnumerator BuffDuration(SHI_ItemBase item)
    {
        yield return new WaitForSeconds(item.Duration); // ���� ���� �ð� ���� ���
        TlifeSteal = 0; // ����� ��� ����
        TminionHitup2 = 1; // �̴Ͼ� ���ݷ� ����
        TnexusHitUp = 1; // �ؼ��� ���ݷ� ����
        Tallup = 1; // ��ü ���� ����
    }

}
