using UnityEngine;
using System.Collections;
using RSG;

public class Example2Actor : MonoBehaviour
{
    /// <summary>
    /// Goal to move towards
    /// </summary>
    public Transform goal;

    /// <summary>
    /// State machine
    /// </summary>
    private IState rootState;

    /// <summary>
    /// Distance to retreat before approaching the target again.
    /// </summary>
    float resetDistance = 10f;

    private class MovingState : AbstractState
    {
        public float movementSpeed = 3f;
    }

    private class RetreatingState : MovingState
    {
        public Vector3 direction;
    }

    // Use this for initialization
    void Start ()
    {
        rootState = new StateMachineBuilder()
            // First, approach the goal
            .State<MovingState>("Approach")
                .Enter(state =>
                {
                    Debug.Log("Entering Approach state");
                })
                // Move towards the goal
                .Update((state, deltaTime) =>
                {
                    var directionToTarget = transform.position - goal.position;
                    directionToTarget.y = 0; // Only calculate movement on a 2d plane
                    directionToTarget.Normalize();

                    transform.position -= directionToTarget * deltaTime * state.movementSpeed;
                })
                // Once the TargetReached event is triggered, retreat away again
                .Event("TargetReached", state =>
                {
                    state.PushState("Retreat");
                })
                .Exit(state =>
                {
                    Debug.Log("Exiting Approach state");
                })
                // Retreating state
                .State<RetreatingState>("Retreat")
                    // Set a new destination
                    .Enter(state =>
                    {
                        Debug.Log("Entering Retreat state");

                        // Work out a new target, away from the goal
                        var direction = new Vector3(Random.value, 0f, Random.value);
                        direction.Normalize();

                        state.direction = direction;
                    })
                    // Move towards the new destination
                    .Update((state, deltaTime) =>
                    {
                        transform.position -= state.direction * deltaTime * state.movementSpeed;
                    })
                    // If we go further away from the original target than the reset distance, exit and 
                    // go back to the previous state
                    .Condition(() => 
                    {
                        return Vector3.Distance(transform.position, goal.position) >= resetDistance;
                    },
                    state =>
                    {
                        state.Parent.PopState();
                    })
                    .Exit(state =>
                    {
                        Debug.Log("Exiting Retreat state");
                    })
                    .End()
                .End()
            .Build();

        rootState.ChangeState("Approach");
    }
    
    // Update is called once per frame
    void Update()
    {
        rootState.Update(Time.deltaTime);
    }

    /// <summary>
    /// Tell our state machine that the target has been reached once we hit the trigger
    /// </summary>
    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform == goal)
        {
            rootState.TriggerEvent("TargetReached");
        }
    }
}
