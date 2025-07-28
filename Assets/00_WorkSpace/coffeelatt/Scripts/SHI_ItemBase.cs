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
        TrinityForce, //  Ʈ��
        RapidFirecannon, //����
        MinionSlayer, // �̴Ͼ� �����̾�
        BuffLifeSteal, // ����� ���
        BuffSlayer, // �̴Ͼ�+�ؼ��� ���� ����
        BuffImmortal, // ��ü ���� ����
    }
    public ItemName itemNameEnum; // enum Ÿ������ ������ �̸��� ����
    [Header("���ӽð� , 0�� ������")]
    public float Duration; // ������ ���� �ð�
    [Header("0: �Һ� ������,���� ������ 1: ��� ������")]
    public int type; // ������ Ÿ�� (0: �Һ� ������,���� ������ 1: ��� ������) ������ ������ 2
    [Header("�Һ������ ȿ��")]
    public float Healhp; // �������� ȸ���ϴ� HP ��
    public float Healmp; // �������� ȸ���ϴ� MP ��
    public float GainExp; // �������� �ִ� ����ġ ��
    [Header("���� ������")]
    public float hp; // �������� ������Ű�� HP ��
    public float mp; // �������� ������Ű�� MP ��
    public float atk; // �������� ������Ű�� ���ݷ� ��
    
    public float moveSpeed; // �������� ������Ű�� �̵� �ӵ� ��
    public float atkSpeed; // �������� ������Ű�� ���� �ӵ� ��
    
    public float skillPower; // �������� ������Ű�� ��ų ���ݷ� ��
    public float rangeUp; // �������� ������Ű�� ��Ÿ� ��
    [Range(0, 1)]
    public float coolDown; // �������� ������Ű�� ��Ÿ�� ���� ��
    [Range(0, 1)]
    public float critChance; // �������� ������Ű�� ġ��Ÿ Ȯ�� ��
    [Header("Ư�� ȿ��")]
    [Range(0,1)]
    public float lifeSteal; // �������� ������Ű�� ����� ��� ��
    [Header("���� ���� ������MinionSlayer")]
    [Range(0, 1)]
    public float minionHitup; // �������� ������Ű�� �̴Ͼ� ���ݷ� ��
    [Header("������ ���� ������BuffSlayer")]
    [Range(0,2)]
    public float minionHitup2; // �������� ������Ű�� �̴Ͼ� ���ݷ� ��2 (�߰� ������ ��� ����)
    [Range(1, 2)]
    public float nexusHitUp; // �������� ������Ű�� �ؼ��� ���ݷ� ��
    [Range(1,2)]
    public float allup ; // �������� ������Ű�� ��ü ���� ��
    [Range(0, 1)]
    public float minionDamageDown; // �������� ���� ��Ű�� �̴Ͼ� ���ݷ� ��
    [Range(1, 5)]
    public float minionAttackRegeneration; // �������� ������Ű�� �̴Ͼ� ���ݽ� ��� �ӵ� ����

}