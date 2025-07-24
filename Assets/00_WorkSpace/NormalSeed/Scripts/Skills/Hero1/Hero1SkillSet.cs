using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero1SkillSet : SkillSet
{
    //public SkillSO skill_Q;
    //public SkillSO skill_W;
    //public SkillSO skill_E;
    //public SkillSO skill_R;

    //protected HeroController hero;

    protected override void UseQ()
    {
        // 마우스 방향에 부채꼴로 공격하는 스킬
    }

    protected override void UseW()
    {
        // 5초동안 방어력을 총 공격력의 0.2배만큼 올려주는 스킬
    }

    protected override void UseE()
    {
        // 마우스 방향으로 돌진하고 경로상에 부딪힌 적에 데미지를 주고 적 Hero와 부딪히면 멈추는 스킬
    }

    protected override void UseR()
    {
        // 사정거리 안의 Hero를 선택해서 공격 가능, 적에게 큰 데미지를 주고 이동속도를 1초동안 감소시키는 스킬
    }
}
