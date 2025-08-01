using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SHI_ItemBase;



public class SHI_ItemManager2 : MonoBehaviour ,IManager
{
    public int Priority => (int)ManagerPriority.ItemManager;
    public bool IsDontDestroy => false; // �� �Ŵ����� �� ��ȯ �� �ı���
    public void Initialize() 
    {
        resultValue = GameObject.Find("Result").GetComponent<SHI_ResultValue>();
        Debug.Log("������ �Ŵ��� �ʱ�ȭ��");
    } // �ʱ�ȭ �޼���
    
    public void Cleanup() { } // ���� �޼���
    public GameObject GetGameObject() => this.gameObject; // ���� ���� ������Ʈ ��ȯ

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
    private HashSet<ItemName> activeBuffs = new HashSet<ItemName>();

    public Events.VoidEvent refrash = new Events.VoidEvent();
    public HeroModel stat;
    public SHI_ResultValue resultValue; // ������ �������� ���� ���� ����
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
    private void Start()
    {
        resultValue = GameObject.Find("Result").GetComponent<SHI_ResultValue>();
    }
    private void Update()
    {
        
    }
    public bool UseItem(SHI_ItemBase item)
    {
        if (stat.CurHP.Value <= 0)
        {
            Debug.Log("�÷��̾ �׾��ֽ��ϴ�. �������� ����� �� �����ϴ�.");
            return false; // �÷��̾ �׾����� ��� ������ ��� �Ұ�
        }

        if (item.type <= 0) // �Һ� ������
        {

            Debug.Log("���Գ�?");
            HpUp(item.Healhp); // �÷��̾��� HP ȸ��
            MpUp(item.Healmp); // �÷��̾��� MP ȸ��
            ExpUp(item.GainExp); // �÷��̾��� ����ġ ����
            refrash.Invoke(); //����Ƽ�̺�Ʈ
            if (item.Duration <= 0)
            {
                return true; // ���� �������� �ƴҰ�� ���ư�.
            }
            if (activeBuffs.Contains(item.itemNameEnum))
            {
                Debug.Log("�̹� ���� ���� �����Դϴ�.");
                return false; // �̹� ���� ���� ���� �������� ���, �ٽ� �������� ����

            }

            if (item.lifeSteal > 0 || item.allup > 1 || item.minionHitup > 1)
            {
                Buff(item); // �������� ���� ȿ�� ����

            }

            return true; // ������ ��� ����
        }

        else if (item.type == 1) // ��� ������
        {
            Equip(item); // �÷��̾��� ������ �������� �������� ����

            refrash.Invoke(); //����Ƽ�̺�Ʈ
            return true;
        }
        else if (item.type == 2)  // ������ ������
        {
            UnEquip(item); // �÷��̾��� ������ �������� �������� ����

            refrash.Invoke(); //����Ƽ�̺�Ʈ
            return true;
        }
        else
        {
            Debug.LogError("�� �� ���� ������ Ÿ���Դϴ�.");
            return false; // �� �� ���� ������ Ÿ���� ���, ���� ó��
        }



    }
    void HpUp(float hp)
    {
        stat.CurHP.Value = Mathf.Min(stat.CurHP.Value + (int)hp, (int)resultValue.Hp); // �÷��̾��� HP ȸ��
        //if(�÷��̾� �ִ�HP < �÷��̾� ����hp)
        {
            //�÷��̾� ����hp = �÷��̾� �ִ�HP; // �÷��̾��� HP�� �ִ�ġ�� ���� �ʵ��� ����
        }

    }
    void MpUp(float mp)
    {
        stat.CurMP.Value = Mathf.Min(stat.CurMP.Value + (int)mp, (int)resultValue.Mp); // �÷��̾��� MP ȸ��
        //�÷��̾� ����mp += item.Healmp; // �÷��̾��� MP ȸ��
        //if(�÷��̾� �ִ�MP < �÷��̾� ����mp)
        {
            //�÷��̾� ����mp = �÷��̾� �ִ�MP; // �÷��̾��� MP�� �ִ�ġ�� ���� �ʵ��� ����
        }
    }
    void ExpUp(float exp)
    {//���� ����ġ���� ����.
        //�÷��̾� ����exp += item.GainExp; // �÷��̾��� ����ġ ����
        // exp�� level up �Լ����� ���� ������ �߰���.
    }
    void Buff(SHI_ItemBase item)
    {
        activeBuffs.Add(item.itemNameEnum);
        //���� �÷��̾��� atk
        TlifeSteal += item.lifeSteal; // ����� ��� ����
        TminionHitup2 += item.minionHitup2; // �̴Ͼ� ���ݷ� ����
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

        yield return new WaitForSeconds(item.Duration); // ���� ���� �ð� ���� ���

        switch (item.itemNameEnum)
        {
            case ItemName.BuffLifeSteal:
                TlifeSteal -= item.lifeSteal; // ����� ��� ����
                break;

            case ItemName.BuffSlayer:
                TminionHitup2 -= item.minionHitup2; // �̴Ͼ� ���ݷ� ����2
                TnexusHitUp /= item.nexusHitUp; // �ؼ��� ���ݷ� ����
                break;
            case ItemName.BuffImmortal:
                Tallup /= item.allup; // ��ü ���� ����
                break;
        }

        RemoveBuff(item.itemNameEnum);
        refrash.Invoke(); //����Ƽ�̺�Ʈ
    }
    public void RemoveBuff(ItemName name)
    {
        activeBuffs.Remove(name);
        Debug.Log($"{name} ���� �����");
    }

}
