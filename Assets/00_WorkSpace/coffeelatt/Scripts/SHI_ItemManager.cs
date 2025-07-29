using System.Collections;
using UnityEngine;



public class SHI_ItemManager : MonoBehaviour
{
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
    [SerializeField] float BuffItemCoolDown = 0; // 버프 아이템의 쿨타임 감소 양

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
            BuffItemCoolDown -= Time.deltaTime; // 버프 아이템의 쿨타임 감소
        }
    }
    public void UseItem(SHI_ItemBase item)
    {

        if (item.type == 0) // 소비 아이템
        {
            // if (현재체력 <= 0) // 플레이어의 hp가 0일때는 사용불가 혹은 플레이어 다이드 불값 받아옴
            // {
            //     Debug.Log("플레이어의 HP가 0이므로 아이템을 사용할 수 없습니다.");
            //     return;
            //}
            Debug.Log("들어왔나?");
            HpUp(item.Healhp); // 플레이어의 HP 회복
            MpUp(item.Healmp); // 플레이어의 MP 회복
            ExpUp(item.GainExp); // 플레이어의 경험치 증가
            refrash.Invoke(); //유니티이벤트
            if (BuffItemCoolDown > 0)
            {
                return; // 버프 아이템의 쿨타임이 남아있으면 아이템 사용 중지
            }
            if (BuffItemCoolDown <= 0) // 버프 아이템의 쿨타임이 있다면
            {

                if (item.lifeSteal > 0 || item.nexusHitUp > 1 || item.minionHitup > 1)
                {
                    Buff(item); // 아이템의 버프 효과 적용

                }
            }
            Destroy(item.gameObject); // 아이템 사용 후 제거
        }

        else if (item.type == 1) // 장비 아이템
        {
            Equip(item); // 플레이어의 스탯을 아이템의 스탯으로 증가
            refrash.Invoke(); //유니티이벤트
        }
        else if (item.type == 2) // 장착된 아이템
        {
            UnEquip(item); // 플레이어의 스탯을 아이템의 스탯으로 감소
            refrash.Invoke(); //유니티이벤트
        }

    }
    void HpUp(float hp)
    {
        //플레이어 현재hp += item.Healhp; // 플레이어의 HP 회복
        //if(플레이어 최대HP < 플레이어 현재hp)
        {
            //플레이어 현재hp = 플레이어 최대HP; // 플레이어의 HP가 최대치를 넘지 않도록 조정
        }

    }
    void MpUp(float mp)
    {
        //플레이어 현재mp += item.Healmp; // 플레이어의 MP 회복
        //if(플레이어 최대MP < 플레이어 현재mp)
        {
            //플레이어 현재mp = 플레이어 최대MP; // 플레이어의 MP가 최대치를 넘지 않도록 조정
        }
    }
    void ExpUp(float exp)
    {
        //플레이어 현재exp += item.GainExp; // 플레이어의 경험치 증가
        // exp는 level up 함수에서 관리 함으로 추가만.
    }
    void Buff(SHI_ItemBase item)
    {
        //현재 플레이어의 atk
        TlifeSteal += item.lifeSteal; // 생명력 흡수 증가
        TminionHitup2 = (item.minionHitup2 + TminionHitup); // 미니언 공격력 증가
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
        BuffItemCoolDown = item.Duration; // 버프 아이템의 쿨타임 설정
        yield return new WaitForSeconds(item.Duration); // 버프 지속 시간 동안 대기

        TlifeSteal = 0; // 생명력 흡수 감소
        TminionHitup2 = TminionHitup; // 미니언 공격력 감소
        TnexusHitUp = 1; // 넥서스 공격력 감소
        Tallup = 1; // 전체 스탯 감소
        refrash.Invoke(); //유니티이벤트
    }

}
