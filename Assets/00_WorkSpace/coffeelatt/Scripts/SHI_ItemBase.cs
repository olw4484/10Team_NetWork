using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SHI_ItemBase : MonoBehaviour
{

    public enum ItemName
    {
       HealHp,
       HealMp,
        GainExp,
        TrinityForce, //  트포
        RapidFirecannon, //고연포
        MinionSlayer, // 미니언 슬레이어
        BuffLifeSteal, // 생명력 흡수
        BuffSlayer, // 미니언+넥서스 피해 증가
        BuffImmortal, // 전체 스탯 증가
    }
    public ItemName itemNameEnum; // enum 타입으로 아이템 이름을 저장
    [Header("지속시간 , 0은 무제한")]
    public float Duration; // 아이템 지속 시간
    [Header("0: 소비 아이템,버프 아이템 1: 장비 아이템")]
    public int type; // 아이템 타입 (0: 소비 아이템,버프 아이템 1: 장비 아이템) 장착된 아이템 2
    [Header("소비아이템 효과")]
    public float Healhp; // 아이템이 회복하는 HP 양
    public float Healmp; // 아이템이 회복하는 MP 양
    public float GainExp; // 아이템이 주는 경험치 양
    [Header("스탯 증가량")]
    public float hp; // 아이템이 증가시키는 HP 양
    public float mp; // 아이템이 증가시키는 MP 양
    public float atk; // 아이템이 증가시키는 공격력 양
    
    public float moveSpeed; // 아이템이 증가시키는 이동 속도 양
    public float atkSpeed; // 아이템이 증가시키는 공격 속도 양
    
    public float skillPower; // 아이템이 증가시키는 스킬 공격력 양
    public float rangeUp; // 아이템이 증가시키는 사거리 양
    [Range(0, 1)]
    public float coolDown; // 아이템이 증가시키는 쿨타임 감소 양
    [Range(0, 1)]
    public float critChance; // 아이템이 증가시키는 치명타 확률 양
    [Header("특수 효과")]
    [Range(0,1)]
    public float lifeSteal; // 아이템이 증가시키는 생명력 흡수 양
    [Header("장비로 인한 증가량MinionSlayer")]
    [Range(0, 1)]
    public float minionHitup; // 아이템이 증가시키는 미니언 공격력 양
    [Header("버프로 인한 증가량BuffSlayer")]
    [Range(0,2)]
    public float minionHitup2; // 아이템이 증가시키는 미니언 공격력 양2 (추가 변수로 사용 가능)
    [Range(1, 2)]
    public float nexusHitUp; // 아이템이 증가시키는 넥서스 공격력 양
    [Range(1,2)]
    public float allup ; // 아이템이 증가시키는 전체 스탯 양
    [Range(0, 1)]
    public float minionDamageDown; // 아이템이 감소 시키는 미니언 공격력 양
    [Range(1, 5)]
    public float minionAttackRegeneration; // 아이템이 증가시키는 미니언 공격시 재생 속도 증가

}