using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class AnimationController : Singleton<AnimationController>
{
    private readonly int _2HWeaponWithdraw = Animator.StringToHash("2HWeaponWithdraw");
    
    public void TwoHandsWeaponWithdraw(Animator animator)
    {
        animator.SetBool(_2HWeaponWithdraw, true);
    }
}
