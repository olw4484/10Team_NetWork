using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillSO : ScriptableObject
{
    public string skillName; // 스킬 이름
    public int skillLevel; // 스킬 레벨
    public bool isPassive; // 패시브 스킬인지 여부 체크
    public int damage; // 스킬 데미지
    public float skillRange; // 스킬 사거리
    public List<float> buffAmount = new(); // 스킬이 버프 스킬일 때 버프 수치들. 버프가 여러 수치에 영향을 줄 수 있으므로 리스트로 생성함
    public float mana; // 스킬 사용에 필요한 마나량
    public float cooldown; // 스킬 쿨타임
    public Sprite icon; // 스킬 아이콘

    private float lastUsedTime = -Mathf.Infinity;

    // SkillSet에서 해당 Skill의 Use 메서드를 실행할 때 쿨다운 중인지 알려주는 메서드
    public bool IsOnCooldown()
    {
        return Time.time < lastUsedTime + cooldown;
    }

    // Skill의 쿨다운 시작 시간을 설정하는 메서드
    public void TriggerCooldown()
    {
        lastUsedTime = Time.time;
    }
}
