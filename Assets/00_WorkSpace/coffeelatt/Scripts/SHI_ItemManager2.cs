using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SHI_ItemBase;



public class SHI_ItemManager2 : MonoBehaviour ,IManager
{
    public int Priority => (int)ManagerPriority.ItemManager;
    public bool IsDontDestroy => false; // 이 매니저는 씬 전환 시 파괴됨
    public void Initialize() 
    {
        resultValue = GameObject.Find("Result").GetComponent<SHI_ResultValue>();
        Debug.Log("아이템 매니저 초기화됨");
    } // 초기화 메서드
    
    public void Cleanup() { } // 정리 메서드
    public GameObject GetGameObject() => this.gameObject; // 현재 게임 오브젝트 반환

    //public static SHI_ItemManager instance;  //필요없어보임.
    //[SerializeField] GameObject player; // 플레이어 오브젝트를 참조하기 위한 변수
    public float Thp = 0; // 아이템이 증가시키는 HP 양
    public float Tmp = 0; // 아이템이 증가시키는 MP 양
    public float Tatk = 0; // 아이템이 증가시키는 공격력 양
    public float TcoolDown = 1; // 아이템이 증가시키는 쿨타임 감소 양
    public float TmoveSpeed = 0; // 아이템이 증가시키는 이동 속도 양
    public float TatkSpeed = 0; // 아이템이 증가시키는 공격 속도 양
    public float TcritChance = 0; // 아이템이 증가시키는 치명타 확률 양
    public float TskillPower = 1; // 아이템이 증가시키는 스킬 공격력 양
    public float TrangeUp = 0; // 아이템이 증가시키는 사거리 양
    public float TlifeSteal = 0; // 아이템이 증가시키는 생명력 흡수 양
    public float TminionHitup = 1; // 아이템이 증가시키는 미니언 공격력 양
    public float TminionHitup2 = 0; // 아이템이 증가시키는 미니언 공격력 양2 (추가 변수로 사용 가능)
    public float TnexusHitUp = 1; // 아이템이 증가시키는 넥서스 공격력 양
    public float Tallup = 1; // 아이템이 증가시키는 전체 스탯 양
    public float TminionDamageDown = 1; // 아이템이 감소 시키는 미니언 공격력 양
    public float TminionAttackRegeneration = 0;// 아이템이 증가시키는 미니언 공격시 재생 속도 증가
    private HashSet<ItemName> activeBuffs = new HashSet<ItemName>();

    public Events.VoidEvent refrash = new Events.VoidEvent();
    public HeroModel stat;
    public SHI_ResultValue resultValue; // 스탯을 가져오기 위한 참조 변수
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
            Debug.Log("플레이어가 죽어있습니다. 아이템을 사용할 수 없습니다.");
            return false; // 플레이어가 죽어있을 경우 아이템 사용 불가
        }

        if (item.type <= 0) // 소비 아이템
        {

            Debug.Log("들어왔나?");
            HpUp(item.Healhp); // 플레이어의 HP 회복
            MpUp(item.Healmp); // 플레이어의 MP 회복
            ExpUp(item.GainExp); // 플레이어의 경험치 증가
            refrash.Invoke(); //유니티이벤트
            if (item.Duration <= 0)
            {
                return true; // 버프 아이템이 아닐경우 돌아감.
            }
            if (activeBuffs.Contains(item.itemNameEnum))
            {
                Debug.Log("이미 적용 중인 버프입니다.");
                return false; // 이미 적용 중인 버프 아이템일 경우, 다시 적용하지 않음

            }

            if (item.lifeSteal > 0 || item.allup > 1 || item.minionHitup > 1)
            {
                Buff(item); // 아이템의 버프 효과 적용

            }

            return true; // 아이템 사용 성공
        }

        else if (item.type == 1) // 장비 아이템
        {
            Equip(item); // 플레이어의 스탯을 아이템의 스탯으로 증가

            refrash.Invoke(); //유니티이벤트
            return true;
        }
        else if (item.type == 2)  // 장착된 아이템
        {
            UnEquip(item); // 플레이어의 스탯을 아이템의 스탯으로 감소

            refrash.Invoke(); //유니티이벤트
            return true;
        }
        else
        {
            Debug.LogError("알 수 없는 아이템 타입입니다.");
            return false; // 알 수 없는 아이템 타입일 경우, 실패 처리
        }



    }
    void HpUp(float hp)
    {
        stat.CurHP.Value = Mathf.Min(stat.CurHP.Value + (int)hp, (int)resultValue.Hp); // 플레이어의 HP 회복
        //if(플레이어 최대HP < 플레이어 현재hp)
        {
            //플레이어 현재hp = 플레이어 최대HP; // 플레이어의 HP가 최대치를 넘지 않도록 조정
        }

    }
    void MpUp(float mp)
    {
        stat.CurMP.Value = Mathf.Min(stat.CurMP.Value + (int)mp, (int)resultValue.Mp); // 플레이어의 MP 회복
        //플레이어 현재mp += item.Healmp; // 플레이어의 MP 회복
        //if(플레이어 최대MP < 플레이어 현재mp)
        {
            //플레이어 현재mp = 플레이어 최대MP; // 플레이어의 MP가 최대치를 넘지 않도록 조정
        }
    }
    void ExpUp(float exp)
    {//아직 경험치로직 없음.
        //플레이어 현재exp += item.GainExp; // 플레이어의 경험치 증가
        // exp는 level up 함수에서 관리 함으로 추가만.
    }
    void Buff(SHI_ItemBase item)
    {
        activeBuffs.Add(item.itemNameEnum);
        //현재 플레이어의 atk
        TlifeSteal += item.lifeSteal; // 생명력 흡수 증가
        TminionHitup2 += item.minionHitup2; // 미니언 공격력 증가
        TnexusHitUp *= item.nexusHitUp; // 넥서스 공격력 증가
        Tallup *= item.allup; // 전체 스탯 증가
        refrash.Invoke(); //유니티이벤트
        StartCoroutine(BuffDuration(item)); // 버프 지속 시간 동안 효과 적용
    }
    void Equip(SHI_ItemBase item)
    {
        Thp += item.hp; // 플레이어의 HP 증가
        Tmp += item.mp; // 플레이어의 MP 증가
        Tatk += item.atk; // 플레이어의 공격력 증가
        TcoolDown -= item.coolDown; // 플레이어의 쿨타임 감소
        TmoveSpeed += item.moveSpeed; // 플레이어의 이동 속도 증가
        TatkSpeed += item.atkSpeed; // 플레이어의 공격 속도 증가
        TcritChance += item.critChance; // 플레이어의 치명타 확률 증가
        TskillPower += item.skillPower; // 플레이어의 스킬 공격력 증가
        TrangeUp += item.rangeUp; // 플레이어의 사거리 증가
        TminionDamageDown -= item.minionDamageDown; // 플레이어의 미니언 공격력 감소
        TminionHitup += item.minionHitup; // 플레이어의 미니언공격시 공격력 증가
        TminionAttackRegeneration += item.minionAttackRegeneration; // 플레이어의 미니언 공격시 재생 속도 증가
        item.type = 2; // 아이템 타입을 장착된 아이템으로 변경
    }
    void UnEquip(SHI_ItemBase item) // 아이템을 해제할 때 호출되는 함수 onclick 이벤트를 사용하여 해제?
    {
        Thp -= item.hp;
        Tmp -= item.mp; // 플레이어의 MP 감소
        Tatk -= item.atk; // 플레이어의 공격력 감소
        TcoolDown += item.coolDown; // 플레이어의 쿨타임 증가
        TmoveSpeed -= item.moveSpeed; // 플레이어의 이동 속도 감소
        TatkSpeed -= item.atkSpeed; // 플레이어의 공격 속도 감소
        TcritChance -= item.critChance; // 플레이어의 치명타 확률 감소
        TskillPower -= item.skillPower; // 플레이어의 스킬 공격력 감소
        TrangeUp -= item.rangeUp; // 플레이어의 사거리 감소
        TminionDamageDown += item.minionDamageDown; // 플레이어의 미니언 공격력 증가
        TminionHitup -= item.minionHitup; // 플레이어의 미니언공격시 공격력 감소
        TminionAttackRegeneration -= item.minionAttackRegeneration; // 플레이어의 미니언 공격시 재생 속도 감소
        item.type = 1; // 아이템 타입을 장비 아이템으로 변경
    }
    IEnumerator BuffDuration(SHI_ItemBase item)
    {

        yield return new WaitForSeconds(item.Duration); // 버프 지속 시간 동안 대기

        switch (item.itemNameEnum)
        {
            case ItemName.BuffLifeSteal:
                TlifeSteal -= item.lifeSteal; // 생명력 흡수 감소
                break;

            case ItemName.BuffSlayer:
                TminionHitup2 -= item.minionHitup2; // 미니언 공격력 감소2
                TnexusHitUp /= item.nexusHitUp; // 넥서스 공격력 감소
                break;
            case ItemName.BuffImmortal:
                Tallup /= item.allup; // 전체 스탯 감소
                break;
        }

        RemoveBuff(item.itemNameEnum);
        refrash.Invoke(); //유니티이벤트
    }
    public void RemoveBuff(ItemName name)
    {
        activeBuffs.Remove(name);
        Debug.Log($"{name} 버프 종료됨");
    }

}
