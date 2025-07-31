using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSkill : MonoBehaviour
{
    public ArmorSkillType skillType;

    [Header("Skill Properties")]
    public float skillCooldown;
    public float skillDuration; 

    [ShowIf("skillType", ArmorSkillType.SpeedBoost)]
    public float speed;

    [ShowIf("skillType", ArmorSkillType.AttackSpeed)]
    public float attackSpeed;

    [ShowIf("skillType", ArmorSkillType.AutoHeal)]
    public float healAmount;
    [ShowIf("skillType", ArmorSkillType.AutoHeal)]
    public float healthRatio;

    [HideInInspector] public float skillCooldownTimer;
    [HideInInspector] public float skillDurationTimer;
    [HideInInspector] public bool isSkill = false;
    [HideInInspector] public bool isCooldown = false;

    private PlayerController cachedController;

    public void Initialize()
    {
        if (skillCooldownTimer == 0f) skillCooldownTimer = skillCooldown;
        if (skillDurationTimer == 0f) skillDurationTimer = skillDuration;
    }

    public void SetSkill(PlayerController playerController)
    {
        if (isCooldown) return;

        Initialize();

        if (!cachedController)
        {
            cachedController = playerController;
        }
        isSkill = true;
        switch(skillType)
        {
            case ArmorSkillType.SpeedBoost:
                ApplySpeedBoost();
                break;
            case ArmorSkillType.AttackSpeed:
                // 패시브 스킬임
                break;
            case ArmorSkillType.AutoHeal:
                // 패시브 스킬임
                break;
            case ArmorSkillType.Invisibility:
                ApplyInvisibility();
                break;
            default:
                Debug.LogWarning("No skill type set.");
                break;
        }
    }

    public void ResetSkill()
    {
        if (!cachedController) return;
        switch (skillType)
        {
            case ArmorSkillType.SpeedBoost:
                cachedController.SetMoveSpeed(5f);
                break;
            case ArmorSkillType.Invisibility:
                cachedController.SetPlayerInvisibility(false);
                break;
            default:
                break;
        }
    }
    private void ApplySpeedBoost()
    {
        cachedController.SetMoveSpeed(speed);
    }

    private void ApplyInvisibility()
    {
        cachedController.SetPlayerInvisibility(true);
    }
}

public enum ArmorSkillType
{
    None,
    SpeedBoost,
    AttackSpeed,
    AutoHeal,
    Invisibility
}
