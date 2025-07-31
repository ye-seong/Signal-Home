using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    [Header("Settings")]
    public float ikWeight = 1f;
    public ItemInstance currentItem;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (currentItem != null && animator != null)
        {
            ApplyWeaponIK();
        }
    }

    void ApplyWeaponIK()
    {
        // ������ IK (�� �׸�)
        if (currentItem.gripPosition != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);

            animator.SetIKPosition(AvatarIKGoal.RightHand, currentItem.gripPosition.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, currentItem.gripPosition.rotation);
        }

        // �޼� IK (��� ����)
        if (currentItem.isTwoHanded && currentItem.leftHandGrip != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, currentItem.leftHandGrip.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, currentItem.leftHandGrip.rotation);
        }
    }

    // ���� ����
    public void EquipWeapon(ItemInstance weapon)
    {
        currentItem = weapon;

        // ���⸦ �����տ� ����
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        weapon.transform.SetParent(rightHand);

        // �⺻ ��ġ ���� (IK�� ������)
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
    }

    // ���� ����
    public void UnequipWeapon()
    {
        if (currentItem != null)
        {
            // IK ����
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

            currentItem.transform.SetParent(null);
            currentItem = null;
        }
    }
}
