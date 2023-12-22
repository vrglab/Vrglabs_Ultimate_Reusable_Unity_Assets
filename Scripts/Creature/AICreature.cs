using CleverCrow.Fluid.BTs.Trees;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICreature : Creature
{
    public AiProfile aiProfile;

    [SerializeField]
    BehaviorTree behaviorTree;

    public void Start()
    {
        behaviorTree = aiProfile.BuildBehaviour(gameObject).End().Build();
    }

    protected override void Movement()
    {
        behaviorTree.Tick();
    }
}
