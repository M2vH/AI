using UnityEngine;
using System.Collections;
using System;

public class IdleBehaviour : BehaviourState
{
    public IdleBehaviour(CowController _cow)
        :base (_cow)
    { }

    public override void StartBehaviour()
    {
        base.StartBehaviour();
        foreach (Renderer r in m_cow.m_renderers)
        {
            r.material.color = Color.green;
        }
    }

    protected override void Behave()
    {
        
    }
}
