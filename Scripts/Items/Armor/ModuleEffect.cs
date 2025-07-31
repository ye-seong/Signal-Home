using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleEffect : MonoBehaviour
{
    public ArmorModuleType moduleType;

    [ShowIf("moduleType", ArmorModuleType.Defense)]
    public float defensePower;

    [ShowIf("moduleType", ArmorModuleType.Resistance)]
    public EnvironmentType environmentType;
    [ShowIf("moduleType", ArmorModuleType.Resistance)]
    public float resistancePower;

    private void Start()
    {
        
    }

    public void ApplyArmorModule()
    {
        
    }
    public float ApplyDefenseEffect(float damage)
    {
        float reducedDamage = damage * (1 - defensePower);
        //Debug.Log(defensePower * 100 + "%�� ������� ���ҽ��� " + reducedDamage + "�� ������� �Ծ����ϴ�.");
        return reducedDamage;
    }

    public float ApplyResistanceEffect(float time)
    {
        float reducedTime = time + time * resistancePower;
        Debug.Log($"time:{reducedTime}");
        return reducedTime;
    }
}
