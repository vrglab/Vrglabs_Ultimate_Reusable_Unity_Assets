using CleverCrow.Fluid.BTs.Trees;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AiProfiles/ChasePlayerInRange")]
public class ChasePlayerInRange : AiProfile
{
    public override BehaviorTreeBuilder BuildBehaviour(GameObject owner)
    {
        return base.BuildBehaviour(owner)
            //Chase player if he is out of range
            .Sequence().IsPlayerInRange().TurnMovementOff().End()
            //Stop chasing if he is in range
            .Sequence().IsPlayerInRange().ChasePlayer();
    }
}
