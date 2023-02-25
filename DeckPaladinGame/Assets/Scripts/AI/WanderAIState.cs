using System;
using System.Collections.Generic;
using UnityEngine;

class WanderAIState : AIState
{


    public WanderAIState(Player owningCharacter, AIPlayerController aiController)
        : base(owningCharacter, aiController)
    {
    }

    public override void Activate()
    {
        m_WanderBounds = GameObject.Find("AIWonderBounds").GetComponent<Collider>().bounds;

        AIController.Target = null;

        AIController.NavAgent.stoppingDistance = AIController.ArriveAtDestinationDist;

        ChooseNewDirection();
    }

    public override void Deactivate()
    {
        AIController.NavAgent.updateRotation = false;
    }

    public override void Update()
    {
        //Jump if needed
        if (AIController.ShouldAIJump())
        {
            AIController.SetState(new JumpAIState(Owner, AIController, GetType()));

            return;
        }

        //Find target if there isn't one already
        if (AIController.Target == null)
        {
            //Search for objects in the radius that:
            //  -have the player tag
            //  -are not this AI's player
            //  -are visible to the AI
            AIController.Target = AIUtils.FindClosestObjectInRadius(
                Owner.transform.position,
                AIController.MaxSightRange,
                (obj) => (obj.tag == "Player" && obj != Owner.gameObject && AIController.CanSeeObject(obj))
                );
        }
        
        //If you have a valid target move into attack range.
        if (AIController.Target != null)
        {
            AIController.SetState(new MoveToAttackRangeAIState(Owner, AIController));

            return;
        }

        bool recomputePath = AIController.NavAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid &&
            !AIController.NavAgent.pathPending;

        AIController.AimPosition = AIController.NavAgent.desiredVelocity.normalized * 10.0f;

        //Continue in the same direction for a bit of time then choose a new direction
        if (m_TimeLeftTillChangeDirection > 0.0f || recomputePath)
        {
            m_TimeLeftTillChangeDirection -= Time.deltaTime;
        }
        else
        {
            ChooseNewDirection();
        }
    }

    public override string GetName()
    {
        return "Wander State";
    }

    private void ChooseNewDirection()
    {
        //Choose a random position to go to
        Vector3 destination = new Vector3(
            UnityEngine.Random.Range(m_WanderBounds.min.x, m_WanderBounds.max.x),
            Owner.transform.position.y,
            UnityEngine.Random.Range(m_WanderBounds.min.z, m_WanderBounds.max.z)
            );

        //Set navigation destination
        AIController.NavAgent.SetDestination(destination);

        //update timer
        m_TimeLeftTillChangeDirection = UnityEngine.Random.Range(
            AIController.MinTimeToChangeDirection,
            AIController.MaxTimeToChangeDirection
            );
    }

    float m_TimeLeftTillChangeDirection;

    Bounds m_WanderBounds;
}
