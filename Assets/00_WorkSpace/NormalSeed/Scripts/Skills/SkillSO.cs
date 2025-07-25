using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillSO : ScriptableObject
{
    public string skillName; // ��ų �̸�
    public int skillLevel; // ��ų ����
    public bool isPassive; // �нú� ��ų���� ���� üũ
    public int damage; // ��ų ������
    public float skillRange; // ��ų ��Ÿ�
    public List<float> buffAmount = new(); // ��ų�� ���� ��ų�� �� ���� ��ġ��. ������ ���� ��ġ�� ������ �� �� �����Ƿ� ����Ʈ�� ������
    public float mana; // ��ų ��뿡 �ʿ��� ������
    public float cooldown; // ��ų ��Ÿ��
    public Sprite icon; // ��ų ������

    private float lastUsedTime = -Mathf.Infinity;

    // SkillSet���� �ش� Skill�� Use �޼��带 ������ �� ��ٿ� ������ �˷��ִ� �޼���
    public bool IsOnCooldown()
    {
        return Time.time < lastUsedTime + cooldown;
    }

    // Skill�� ��ٿ� ���� �ð��� �����ϴ� �޼���
    public void TriggerCooldown()
    {
        lastUsedTime = Time.time;
    }
}
