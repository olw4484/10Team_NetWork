using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHI_ResultValue : MonoBehaviour
{
    public static SHI_ResultValue instance; //  다양하게 여러가지 에서 땡겨쓰기에 스태틱사용.
    public HeroModel stat;
    public SHI_ItemManager addstat;
    public SkillSO skillSO; // 스킬 데이터 참조
    public float Damage;
    public float Hp;
    public float Mp;
    public float ApDamage; // Ability Power Damage
    public float AtkSpeed;
    public float MoveSpeed;
    public float CritChance;
    public float SkillTimeDown; // 쿨타임 감소
    public float LifeSteal; // 생명력 흡수
    public float MinionHitup; // 미니언 공격력 증가 
    public float NexusHitUp; // 넥서스 공격력 증가
    public float DefByMinion; // 미니언 공격력 감소
    public float MinionAttackRegeneration; // 미니언 공격시 재생 속도 증가
    public float AtkRange; // 공격 사거리
    public float Def; // 방어력 (추가 필요시)

    bool isconnect = false;


    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성 방지
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
        AtkRange = (stat.AtkRange + addstat.TrangeUp); // 공격 사거리 증가
        AtkSpeed = (/*stat.AtkSpeed*/ + addstat.TatkSpeed);//공격 속도 증가 
        MoveSpeed = (stat.MoveSpd + addstat.TmoveSpeed);// 이동 속도 증가
        CritChance = (/*stat.CritChance*/ +addstat.TcritChance); // 공격 로직 에 조건 함수가 붙음 0~1사이값을 랜덤으로 부여
        //크리티컬 확률이 필요함. 하지만 기본 크리티컬을 없게 만든다면 이또한 그대로 작동함.
        Def =(stat.Def *addstat.Tallup); // 방어력 증가 (추가 필요시)
                                        // 랜덤수가 크리티컬 찬스보다 낮을때 크리티컬이 발동
                                        // 크리티컬데미지는 기본 2배로고정 크리티컬 찬스를 먼저계산후 해당되지 않을때 일반공격력으로 계산됨.
                                        //********스킬데이터 미연결중*********
                                        //ApDamage = (skillSO.damage *addstat.TskillPower); // 스킬 공격력 증가
                                        //스킬 공격력 데이터 계산방식이 안정해 졌음으로 임시임.
                                        //SkillTimeDown = (skillSO.cooldown *addstat.TcoolDown);//약간 애매..?
        LifeSteal = (/*stat.LifeSteal*/ +addstat.TlifeSteal); //공격할때 흡혈하는 함수로직 추가 만들기
        //기본 흡혈량을 줘도 됨. 밸붕금지 기초엔 없다가 렙업했을때 줘도되고 스탤치 없어도 작동함.
                                                              //공격력* 이 값이 곱해짐. 의 값은 현재체력이며 현재체력은 maxhp를 넘을수 없다 계산.
        MinionHitup = (addstat.TminionHitup2+addstat.TminionHitup);
        NexusHitUp = (addstat.TnexusHitUp); //넥서스를 공격할때 공격력 * 이 값이 곱해짐.
        DefByMinion = (1-addstat.TminionDamageDown); // 계산식에서 미니언이 공격하는 계수 맨 뒤에 * 이 값. 
        MinionAttackRegeneration = (/*stat.Regeneration*/ addstat.TminionAttackRegeneration); // 체력마력 재생 속도 증가
        //기본 회복량 스탯 필요.
    }

    // Update is called once per frame
    //void Update()
    //{
    //    Init();
    //}
}
