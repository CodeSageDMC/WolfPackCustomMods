﻿// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public class StateMachineFloatRandomizer : StateMachineBehaviour
{
    public string parameterName = "RandFloat";
    private int parameterID;

    private void Awake()
    {
        parameterID = Animator.StringToHash(parameterName);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat(parameterID, Random.value);
    }
}