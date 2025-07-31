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
        // 오른손 IK (주 그립)
        if (currentItem.gripPosition != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);

            animator.SetIKPosition(AvatarIKGoal.RightHand, currentItem.gripPosition.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, currentItem.gripPosition.rotation);
        }

        // 왼손 IK (양손 무기)
        if (currentItem.isTwoHanded && currentItem.leftHandGrip != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, currentItem.leftHandGrip.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, currentItem.leftHandGrip.rotation);
        }
    }

    // 무기 장착
    public void EquipWeapon(ItemInstance weapon)
    {
        currentItem = weapon;

        // 무기를 오른손에 부착
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        weapon.transform.SetParent(rightHand);

        // 기본 위치 설정 (IK가 보정함)
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
    }

    // 무기 해제
    public void UnequipWeapon()
    {
        if (currentItem != null)
        {
            // IK 해제
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

            currentItem.transform.SetParent(null);
            currentItem = null;
        }
    }
}
