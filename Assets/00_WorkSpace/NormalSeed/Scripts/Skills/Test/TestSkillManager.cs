using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSkillManager : MonoBehaviour
{
    [SerializeField] private GameObject hero;
    [SerializeField] private HeroController controller;
    [SerializeField] public SkillSet skillSet;
    [SerializeField] private SkillWindowUI ui;
    [SerializeField] private float skillQCooldown;
    [SerializeField] private float skillWCooldown;
    [SerializeField] private float skillECooldown;
    [SerializeField] private float skillRCooldown;

    public ObservableProperty<int> skillPoint { get; private set; } = new();
    public float skillWeight;

    public bool isQUpable = true;
    public bool isWUpable = true;
    public bool isEUpable = true;
    public bool isRUpable = true;

    public static TestSkillManager Instance { get; private set; }

    private void Awake()
    {
        // 이미 인스턴스가 존재하면 중복 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void InitSkillManager(GameObject targetPlayer)
    {
        hero = targetPlayer;
        controller = hero.GetComponent<HeroController>();
        skillSet = hero.GetComponent<SkillSet>();
        ui = hero.GetComponentInChildren<SkillWindowUI>();
        skillPoint.Value = 1;

        skillSet.skill_Q.skillLevel.Value = 0;
        skillSet.skill_W.skillLevel.Value = 0;
        skillSet.skill_E.skillLevel.Value = 0;
        skillSet.skill_R.skillLevel.Value = 0;

        skillSet.skill_Q.skillLevel.Subscribe(OnQSkillLevelChanged);
        skillSet.skill_W.skillLevel.Subscribe(OnWSkillLevelChanged);
        skillSet.skill_E.skillLevel.Subscribe(OnESkillLevelChanged);
        skillSet.skill_R.skillLevel.Subscribe(OnRSkillLevelChanged);
        skillPoint.Subscribe(OnSkillPointChanged);

        skillQCooldown = skillSet.skill_Q.curCooldown;
        skillWCooldown = skillSet.skill_W.curCooldown;
        skillECooldown = skillSet.skill_E.curCooldown;
        skillRCooldown = skillSet.skill_R.curCooldown;
    }

    void OnQSkillLevelChanged(int level)
    {
        Debug.Log("스킬레벨 변경됨. 레벨 : " + skillSet.skill_Q.skillLevel.Value);
        skillSet.skill_Q.curDamage = skillSet.skill_Q.damage[level];
        skillSet.skill_Q.curCooldown = skillSet.skill_Q.cooldown[level];
        skillSet.skill_Q.curMana = skillSet.skill_Q.mana[level];
        ui.ActivateQSkillLevelIcon(level);
    }

    void OnWSkillLevelChanged(int level)
    {
        Debug.Log("스킬레벨 변경됨. 레벨 : " + skillSet.skill_W.skillLevel.Value);
        skillSet.skill_W.curDamage = skillSet.skill_W.damage[level];
        skillSet.skill_W.curCooldown = skillSet.skill_W.cooldown[level];
        skillSet.skill_W.curMana = skillSet.skill_W.mana[level];
        ui.ActivateWSkillLevelIcon(level);
    }

    void OnESkillLevelChanged(int level)
    {
        Debug.Log("스킬레벨 변경됨. 레벨 : " + skillSet.skill_E.skillLevel.Value);
        skillSet.skill_E.curDamage = skillSet.skill_E.damage[level];
        skillSet.skill_E.curCooldown = skillSet.skill_E.cooldown[level];
        skillSet.skill_E.curMana = skillSet.skill_E.mana[level];
        ui.ActivateESkillLevelIcon(level);
    }

    void OnRSkillLevelChanged(int level)
    {
        Debug.Log("스킬레벨 변경됨. 레벨 : " + skillSet.skill_R.skillLevel.Value);
        skillSet.skill_R.curDamage = skillSet.skill_R.damage[level];
        skillSet.skill_R.curCooldown = skillSet.skill_R.cooldown[level];
        skillSet.skill_R.curMana = skillSet.skill_R.mana[level];
        ui.ActivateRSkillLevelIcon(level);
    }

    void OnSkillPointChanged(int point)
    {
        Debug.Log("스킬 포인트 획득함. 현재 스킬 포인트 : " + skillPoint.Value);
        if (point > 0)
        {
            ui.ActivateSkillLevelUpButton();

            RefreshSkillButton();
        }
        else
        {
            ui.UnactivateSkillLevelUpButton();
        }
    }

    private void Update()
    {
        if (hero == null) return;

        if (Input.GetKeyDown(KeyCode.Q) && skillQCooldown == skillSet.skill_Q.curCooldown && skillSet.skill_Q.skillLevel.Value > 0
            && !Input.GetKey(KeyCode.LeftControl))
        {
            skillSet.UseQ();
        }
        if (Input.GetKeyDown(KeyCode.W) && skillWCooldown == skillSet.skill_W.curCooldown && skillSet.skill_W.skillLevel.Value > 0
            && !Input.GetKey(KeyCode.LeftControl))
        {
            skillSet.UseW();
        }
        if (Input.GetKeyDown(KeyCode.E) && skillECooldown == skillSet.skill_E.curCooldown && skillSet.skill_E.skillLevel.Value > 0
            && !Input.GetKey(KeyCode.LeftControl))
        {
            skillSet.UseE();
        }
        if (Input.GetKeyDown(KeyCode.R) && skillRCooldown == skillSet.skill_R.curCooldown && skillSet.skill_R.skillLevel.Value > 0
            && !Input.GetKey(KeyCode.LeftControl))
        {
            skillSet.UseR();
        }

        if (skillSet.isQExecuted)
        {
            controller.model.CurMP.Value -= (int)skillSet.skill_Q.curMana;
            skillQCooldown = 0f;

            skillSet.isQExecuted = false;
        }

        if (skillSet.isWExecuted)
        {
            controller.model.CurMP.Value -= (int)skillSet.skill_W.curMana;
            skillWCooldown = 0f;

            skillSet.isWExecuted = false;
        }

        if (skillSet.isEExecuted)
        {
            controller.model.CurMP.Value -= (int)skillSet.skill_E.curMana;
            skillECooldown = 0f;

            skillSet.isEExecuted = false;
        }

        if (skillSet.isRExecuted)
        {
            controller.model.CurMP.Value -= (int)skillSet.skill_R.curMana;
            skillRCooldown = 0f;

            skillSet.isRExecuted = false;
        }

        if (hero != null)
        {
            if (skillQCooldown == 0f || skillQCooldown < skillSet.skill_Q.curCooldown)
            {
                skillQCooldown += Time.deltaTime;
                SetCooldownShadow(ui.QShadow, skillQCooldown, skillSet.skill_Q.curCooldown);
            }
            else if (skillQCooldown > skillSet.skill_Q.curCooldown)
            {
                skillQCooldown = skillSet.skill_Q.curCooldown;
            }

            if (skillWCooldown == 0f || skillWCooldown < skillSet.skill_W.curCooldown)
            {
                skillWCooldown += Time.deltaTime;
                SetCooldownShadow(ui.WShadow, skillWCooldown, skillSet.skill_W.curCooldown);
            }
            else if (skillWCooldown > skillSet.skill_W.curCooldown)
            {
                skillWCooldown = skillSet.skill_W.curCooldown;
            }

            if (skillECooldown == 0f || skillECooldown < skillSet.skill_E.curCooldown)
            {
                skillECooldown += Time.deltaTime;
                SetCooldownShadow(ui.EShadow, skillECooldown, skillSet.skill_E.curCooldown);
            }
            else if (skillECooldown > skillSet.skill_E.curCooldown)
            {   
                skillECooldown = skillSet.skill_E.curCooldown;
            }

            if (skillRCooldown == 0f || skillRCooldown < skillSet.skill_R.curCooldown)
            {
                skillRCooldown += Time.deltaTime;
                SetCooldownShadow(ui.RShadow, skillRCooldown, skillSet.skill_R.curCooldown);
            }
            else if (skillRCooldown > skillSet.skill_R.curCooldown)
            {
                skillRCooldown = skillSet.skill_R.curCooldown;
            }
        }
        SkillLevelUp();
    }

    private void SetCooldownShadow(Image shadow, float curCooldown, float maxCooldown)
    {
        float ratio = Mathf.Clamp01(curCooldown / maxCooldown);
        shadow.fillAmount = 1f - ratio;
    }

    /// <summary>
    /// 스킬 레벨업을 관리하는 메서드
    /// </summary>
    public void SkillLevelUp()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
        {
            int curLevel = skillSet.skill_Q.skillLevel.Value;

            if (skillPoint.Value <= 0)
            {
                Debug.Log("스킬포인트가 부족합니다.");
                return;
            }

            if (controller.model.Level.Value <= curLevel * 2)
            {
                Debug.Log("스킬을 아직 배울 수 없습니다.");
                return;
            }

            if (curLevel >= skillSet.skill_Q.maxLevel)
            {
                Debug.Log("스킬이 이미 최대 레벨입니다.");
                return;
            }

            skillSet.skill_Q.skillLevel.Value++;
            skillPoint.Value--;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W))
        {
            int curLevel = skillSet.skill_W.skillLevel.Value;

            if (skillPoint.Value < 1)
            {
                Debug.Log("스킬포인트가 부족합니다.");
                return;
            }

            if (controller.model.Level.Value <= curLevel * 2)
            {
                Debug.Log("스킬을 아직 배울 수 없습니다.");
                return;
            }

            if (curLevel >= skillSet.skill_W.maxLevel)
            {
                Debug.Log("스킬이 이미 최대 레벨입니다.");
                return;
            }

            skillSet.skill_W.skillLevel.Value++;
            skillPoint.Value--;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E))
        {
            int curLevel = skillSet.skill_E.skillLevel.Value;

            if (skillPoint.Value < 1)
            {
                Debug.Log("스킬포인트가 부족합니다.");
                return;
            }

            if (controller.model.Level.Value <= curLevel * 2)
            {
                Debug.Log("스킬을 아직 배울 수 없습니다.");
                return;
            }

            if (curLevel >= skillSet.skill_E.maxLevel)
            {
                Debug.Log("스킬이 이미 최대 레벨입니다.");
                return;
            }

            skillSet.skill_E.skillLevel.Value++;
            skillPoint.Value--;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            int curLevel = skillSet.skill_R.skillLevel.Value;

            if (skillPoint.Value < 1)
            {
                Debug.Log("스킬포인트가 부족합니다.");
                return;
            }

            if (controller.model.Level.Value < 6)
            {
                Debug.Log("아직 궁극기를 배울 수 없습니다.");
                return;
            }
            else if (controller.model.Level.Value < 11 && curLevel >= 1)
            {
                Debug.Log("궁극기 레벨을 올릴 수 없습니다.");
                return;
            }
            else if (controller.model.Level.Value < 16 && curLevel >= 2)
            {
                Debug.Log("궁극기 레벨을 올릴 수 없습니다.");
                return;
            }

            if (curLevel >= skillSet.skill_R.maxLevel)
            {
                Debug.Log("스킬이 이미 최대 레벨입니다.");
                return;
            }

            skillSet.skill_R.skillLevel.Value++;
            skillPoint.Value--;
        }
    }

    public void RefreshSkillButton()
    {
        JudgeSkillLevelUpable();

        bool hasSkillPoint = skillPoint.Value > 0;

        ui.QLevelUpButton.gameObject.SetActive(isQUpable);
        ui.WLevelUpButton.gameObject.SetActive(isWUpable);
        ui.ELevelUpButton.gameObject.SetActive(isEUpable);
        ui.RLevelUpButton.gameObject.SetActive(isRUpable);
    }

    //레벨업을 했을 때 스킬 레벨을 올릴 수 있는지 없는지 전달해주는 메서드
    public void JudgeSkillLevelUpable()
    {
        int qLevel = skillSet.skill_Q.skillLevel.Value;
        int wLevel = skillSet.skill_W.skillLevel.Value;
        int eLevel = skillSet.skill_E.skillLevel.Value;
        int rLevel = skillSet.skill_R.skillLevel.Value;

        if (controller.model.Level.Value > qLevel * 2)
        {
            isQUpable = true;
        }
        else
        {
            isQUpable = false;
        }

        if (controller.model.Level.Value > wLevel * 2)
        {
            isWUpable = true;
        }
        else
        {
            isWUpable = false;
        }

        if (controller.model.Level.Value > eLevel * 2)
        {
            isEUpable = true;
        }
        else
        {
            isEUpable = false;
        }

        if (controller.model.Level.Value < 6)
        {
            isRUpable = false;
        }
        else if (controller.model.Level.Value < 11 && rLevel >= 1)
        {
            isRUpable = false;
        }
        else if (controller.model.Level.Value < 16 && rLevel >= 2)
        {
            isRUpable = false;
        }
        else
        {
            isRUpable = true;
        }
    }
}
