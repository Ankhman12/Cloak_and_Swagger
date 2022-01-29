using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

public class GuardAIBehavior : MonoBehaviour
{
    public Transform guardTransform;
    public float walkSpeed = 5f;
    public CharacterController2D guardController;
    
    public List<Transform> PatrolPoints;
    int patrolIndex;
    float waitTimer;
    bool atPatrolPoint = false;
    public float maxWaitTime = 2f;
    public float patrolRadius = 1f;

    Transform destination;

    public enum GuardBehaviorState { 
        Patrolling,
        Investigating,
        Chasing,
        Searching
    };

    public enum GuardMovementState { 
        Walking,
        Standing,
        Running
    };

    GuardMovementState currentMovementState;
    GuardBehaviorState currentBehaviorState;

    [Header("Events")]
    [Space]

    public UnityEvent onTargetSpotted;
    public UnityEvent onTargetLost;
    //public UnityEvent onPatrolPointReached;
    

    // Start is called before the first frame update
    void Start()
    {
        currentMovementState = GuardMovementState.Walking;
        currentBehaviorState = GuardBehaviorState.Patrolling;
        patrolIndex = 0;
        destination = PatrolPoints[patrolIndex];
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (currentBehaviorState == GuardBehaviorState.Patrolling) {
            //loop throught patrol points
            float distanceFromDestination = Mathf.Abs(destination.position.x - guardTransform.position.x);
            
            if (!atPatrolPoint && distanceFromDestination <= patrolRadius) 
            {
                atPatrolPoint = true;
                //Debug.Log("Arrived!");
                waitTimer = 0;
            }
            if (atPatrolPoint && (waitTimer > maxWaitTime))
            {
                
                atPatrolPoint = false;
                patrolIndex = (patrolIndex + 1) % PatrolPoints.Count;
                destination = PatrolPoints[patrolIndex];
                //Debug.Log(destination);
            }
            else if (atPatrolPoint)
            {
                waitTimer += Time.fixedDeltaTime;
            }
        }
        if (!atPatrolPoint) {
            //Debug.Log(guardTransform.position);
            float direction = Mathf.Clamp(destination.position.x - guardTransform.position.x, -1, 1);
            bool isRunning = currentMovementState == GuardMovementState.Running;
            guardController.Move(direction, false, isRunning, false, false);
        }
    }
}
