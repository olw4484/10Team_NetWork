using System.Collections;
using UnityEngine;



public class SHI_ItemManager : MonoBehaviour
{
    //public static SHI_ItemManager instance;  //�ʿ�����.
    //[SerializeField] GameObject player; // �÷��̾� ������Ʈ�� �����ϱ� ���� ����
    public float Thp = 0; // �������� ������Ű�� HP ��
    public float Tmp = 0; // �������� ������Ű�� MP ��
    public float Tatk = 0; // �������� ������Ű�� ���ݷ� ��
    public float TcoolDown = 1; // �������� ������Ű�� ��Ÿ�� ���� ��
    public float TmoveSpeed = 0; // �������� ������Ű�� �̵� �ӵ� ��
    public float TatkSpeed = 0; // �������� ������Ű�� ���� �ӵ� ��
    public float TcritChance = 0; // �������� ������Ű�� ġ��Ÿ Ȯ�� ��
    public float TskillPower = 1; // �������� ������Ű�� ��ų ���ݷ� ��
    public float TrangeUp = 0; // �������� ������Ű�� ��Ÿ� ��
    public float TlifeSteal = 0; // �������� ������Ű�� ����� ��� ��
    public float TminionHitup = 1; // �������� ������Ű�� �̴Ͼ� ���ݷ� ��
    public float TminionHitup2 = 0; // �������� ������Ű�� �̴Ͼ� ���ݷ� ��2 (�߰� ������ ��� ����)
    public float TnexusHitUp = 1; // �������� ������Ű�� �ؼ��� ���ݷ� ��
    public float Tallup = 1; // �������� ������Ű�� ��ü ���� ��
    public float TminionDamageDown = 1; // �������� ���� ��Ű�� �̴Ͼ� ���ݷ� ��
    public float TminionAttackRegeneration = 0;// �������� ������Ű�� �̴Ͼ� ���ݽ� ��� �ӵ� ����
    [SerializeField] float BuffItemCoolDown = 0; // ���� �������� ��Ÿ�� ���� ��

    public Events.VoidEvent refrash = new Events.VoidEvent();
    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}

    }
    private void Update()
    {
        if (BuffItemCoolDown > 0)
        {
            BuffItemCoolDown -= Time.deltaTime; // ���� �������� ��Ÿ�� ����
        }
    }
    public void UseItem(SHI_ItemBase item)
    {

        if (item.type == 0) // �Һ� ������
        {
            // if (����ü�� <= 0) // �÷��̾��� hp�� 0�϶��� ���Ұ� Ȥ�� �÷��̾� ���̵� �Ұ� �޾ƿ�
            // {
            //     Debug.Log("�÷��̾��� HP�� 0�̹Ƿ� �������� ����� �� �����ϴ�.");
            //     return;
            //}
            Debug.Log("���Գ�?");
            HpUp(item.Healhp); // �÷��̾��� HP ȸ��
            MpUp(item.Healmp); // �÷��̾��� MP ȸ��
            ExpUp(item.GainExp); // �÷��̾��� ����ġ ����
            refrash.Invoke(); //����Ƽ�̺�Ʈ
            if (BuffItemCoolDown > 0)
            {
                return; // ���� �������� ��Ÿ���� ���������� ������ ��� ����
            }
            if (BuffItemCoolDown <= 0) // ���� �������� ��Ÿ���� �ִٸ�
            {

                if (item.lifeSteal > 0 || item.nexusHitUp > 1 || item.minionHitup > 1)
                {
                    Buff(item); // �������� ���� ȿ�� ����

                }
            }
            Destroy(item.gameObject); // ������ ��� �� ����
        }

        else if (item.type == 1) // ��� ������
        {
            Equip(item); // �÷��̾��� ������ �������� �������� ����
            refrash.Invoke(); //����Ƽ�̺�Ʈ
        }
        else if (item.type == 2) // ������ ������
        {
            UnEquip(item); // �÷��̾��� ������ �������� �������� ����
            refrash.Invoke(); //����Ƽ�̺�Ʈ
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
        TminionHitup2 = (item.minionHitup2 + TminionHitup); // �̴Ͼ� ���ݷ� ����
        TnexusHitUp *= item.nexusHitUp; // �ؼ��� ���ݷ� ����
        Tallup *= item.allup; // ��ü ���� ����
        refrash.Invoke(); //����Ƽ�̺�Ʈ
        StartCoroutine(BuffDuration(item)); // ���� ���� �ð� ���� ȿ�� ����
    }
    void Equip(SHI_ItemBase item)
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
        item.type = 2; // ������ Ÿ���� ������ ���������� ����
    }
    void UnEquip(SHI_ItemBase item) // �������� ������ �� ȣ��Ǵ� �Լ� onclick �̺�Ʈ�� ����Ͽ� ����?
    {
        Thp -= item.hp;
        Tmp -= item.mp; // �÷��̾��� MP ����
        Tatk -= item.atk; // �÷��̾��� ���ݷ� ����
        TcoolDown += item.coolDown; // �÷��̾��� ��Ÿ�� ����
        TmoveSpeed -= item.moveSpeed; // �÷��̾��� �̵� �ӵ� ����
        TatkSpeed -= item.atkSpeed; // �÷��̾��� ���� �ӵ� ����
        TcritChance -= item.critChance; // �÷��̾��� ġ��Ÿ Ȯ�� ����
        TskillPower -= item.skillPower; // �÷��̾��� ��ų ���ݷ� ����
        TrangeUp -= item.rangeUp; // �÷��̾��� ��Ÿ� ����
        TminionDamageDown += item.minionDamageDown; // �÷��̾��� �̴Ͼ� ���ݷ� ����
        TminionHitup -= item.minionHitup; // �÷��̾��� �̴Ͼ���ݽ� ���ݷ� ����
        TminionAttackRegeneration -= item.minionAttackRegeneration; // �÷��̾��� �̴Ͼ� ���ݽ� ��� �ӵ� ����
        item.type = 1; // ������ Ÿ���� ��� ���������� ����
    }
    IEnumerator BuffDuration(SHI_ItemBase item)
    {
        BuffItemCoolDown = item.Duration; // ���� �������� ��Ÿ�� ����
        yield return new WaitForSeconds(item.Duration); // ���� ���� �ð� ���� ���

        TlifeSteal = 0; // ����� ��� ����
        TminionHitup2 = TminionHitup; // �̴Ͼ� ���ݷ� ����
        TnexusHitUp = 1; // �ؼ��� ���ݷ� ����
        Tallup = 1; // ��ü ���� ����
        refrash.Invoke(); //����Ƽ�̺�Ʈ
    }

}
