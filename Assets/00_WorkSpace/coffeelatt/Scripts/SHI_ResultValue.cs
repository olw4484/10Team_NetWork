using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHI_ResultValue : MonoBehaviour
{
    public static SHI_ResultValue instance; //  �پ��ϰ� �������� ���� ���ܾ��⿡ ����ƽ���.
    public HeroModel stat;
    public SHI_ItemManager addstat;
    public SkillSO skillSO; // ��ų ������ ����
    public float Damage;
    public float Hp;
    public float Mp;
    public float ApDamage; // Ability Power Damage
    public float AtkSpeed;
    public float MoveSpeed;
    public float CritChance;
    public float SkillTimeDown; // ��Ÿ�� ����
    public float LifeSteal; // ����� ���
    public float MinionHitup; // �̴Ͼ� ���ݷ� ���� 
    public float NexusHitUp; // �ؼ��� ���ݷ� ����
    public float DefByMinion; // �̴Ͼ� ���ݷ� ����
    public float MinionAttackRegeneration; // �̴Ͼ� ���ݽ� ��� �ӵ� ����
    public float AtkRange; // ���� ��Ÿ�
    public float Def; // ���� (�߰� �ʿ��)

    bool isconnect = false;


    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� �ߺ� ���� ����
        }
    }
    void Start()
    {
        //stat = GetComponent<HeroModel>();
        //addstat = GetComponent<SHI_ItemManager>();
        //skillSO = GetComponent<SkillSO>();
        Init();
        if(!isconnect)
        {
            addstat.refrash.AddListener(Init);
            //stat.refrash.AddListener(Init);
            //skillSO.refrash.AddListener(Init);
            isconnect = true;
        }
    }
    private void Init()
    {
        Damage = (stat.Atk + addstat.Tatk);
        Hp = (stat.MaxHP + addstat.Thp);
        Mp = (stat.MaxMP + addstat.Tmp);
        AtkRange = (stat.AtkRange + addstat.TrangeUp); // ���� ��Ÿ� ����
        AtkSpeed = (/*stat.AtkSpeed*/ + addstat.TatkSpeed);//���� �ӵ� ���� 
        MoveSpeed = (stat.MoveSpd + addstat.TmoveSpeed);// �̵� �ӵ� ����
        CritChance = (/*stat.CritChance*/ +addstat.TcritChance); // ���� ���� �� ���� �Լ��� ���� 0~1���̰��� �������� �ο�
        //ũ��Ƽ�� Ȯ���� �ʿ���. ������ �⺻ ũ��Ƽ���� ���� ����ٸ� �̶��� �״�� �۵���.
        Def =(stat.Def *addstat.Tallup); // ���� ���� (�߰� �ʿ��)
                                        // �������� ũ��Ƽ�� �������� ������ ũ��Ƽ���� �ߵ�
                                        // ũ��Ƽ�õ������� �⺻ 2��ΰ��� ũ��Ƽ�� ������ ��������� �ش���� ������ �Ϲݰ��ݷ����� ����.
                                        //********��ų������ �̿�����*********
                                        //ApDamage = (skillSO.damage *addstat.TskillPower); // ��ų ���ݷ� ����
                                        //��ų ���ݷ� ������ ������� ������ �������� �ӽ���.
                                        //SkillTimeDown = (skillSO.cooldown *addstat.TcoolDown);//�ణ �ָ�..?
        LifeSteal = (/*stat.LifeSteal*/ +addstat.TlifeSteal); //�����Ҷ� �����ϴ� �Լ����� �߰� �����
        //�⺻ �������� �൵ ��. ��ر��� ���ʿ� ���ٰ� ���������� �൵�ǰ� ����ġ ��� �۵���.
                                                              //���ݷ�* �� ���� ������. �� ���� ����ü���̸� ����ü���� maxhp�� ������ ���� ���.
        MinionHitup = (addstat.TminionHitup2+addstat.TminionHitup);
        NexusHitUp = (addstat.TnexusHitUp); //�ؼ����� �����Ҷ� ���ݷ� * �� ���� ������.
        DefByMinion = (1-addstat.TminionDamageDown); // ���Ŀ��� �̴Ͼ��� �����ϴ� ��� �� �ڿ� * �� ��. 
        MinionAttackRegeneration = (/*stat.Regeneration*/ addstat.TminionAttackRegeneration); // ü�¸��� ��� �ӵ� ����
        //�⺻ ȸ���� ���� �ʿ�.
    }

    // Update is called once per frame
    //void Update()
    //{
    //    Init();
    //}
}
