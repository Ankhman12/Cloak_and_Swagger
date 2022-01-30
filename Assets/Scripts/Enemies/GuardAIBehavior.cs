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
    [SerializeField] Transform player;

    //private GameObject player;
    private bool playerInVision;
    //Range that chasing state will be entered from
    private float attackRange = 4f;
    // Instead of changing the vision cone to a square, we could use this to just have a circular radius around the guard in which he will continue chasing the player.
    private float chasingRange = 6f;
    /** Empty transform child of guard. Set in inspector. Used for tracking the target during investigation phase */
    [SerializeField] private Vector3 investigationTarget;
    /** True when guard is looking back and forth after searching */
    private bool looking = false;
    private Coroutine lookCoroutine;

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
        switch(currentBehaviorState)
        {
            case GuardBehaviorState.Patrolling:
                //Debug.Log("patrolling");
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
                break;
            case GuardBehaviorState.Investigating:
                atPatrolPoint = false;
                if (playerInVision)
                {
                    //Debug.Log("Investigating");
                    investigationTarget = player.transform.position;
                    //Debug.Log("investTarget: " + investigationTarget);
                    destination.position = investigationTarget;
                    float distanceFromPlayer = (this.transform.position - player.transform.position).magnitude;
                    if (distanceFromPlayer <= attackRange)
                    {
                        currentBehaviorState = GuardBehaviorState.Chasing;
                    }
                } 
                else
                {
                    currentBehaviorState = GuardBehaviorState.Searching;
                }
                break;
            case GuardBehaviorState.Searching:
                //Debug.Log("searching");
                atPatrolPoint = false;
                if (!looking)
                {
                    if (Mathf.Abs(this.transform.position.x - investigationTarget.x) < patrolRadius) //1f is a number that is supposed to be "close enough" to destination
                    {
                        lookCoroutine = StartCoroutine(Look());
                    }
                    else
                    {
                        destination.position = investigationTarget;
                    }
                }
                break;
            case GuardBehaviorState.Chasing:
                //Debug.Log("chasing");
                atPatrolPoint = false;
                Vector2 currentDist = (this.transform.position - player.transform.position);
                if (currentDist.magnitude <= chasingRange)
                {
                    //investigationTarget = player.transform;
                    investigationTarget = player.transform.position;
                    destination.position = investigationTarget;
                } else
                {
                    currentBehaviorState = GuardBehaviorState.Searching;
                }
                break;
            default:
                break;
        }

        
        /**
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
        */
        if (!atPatrolPoint) {
            //Debug.Log(guardTransform.position);
            float direction = Mathf.Clamp(destination.position.x - guardTransform.position.x, -1, 1);
            bool isRunning = currentMovementState == GuardMovementState.Running;
            guardController.Move(direction, false, isRunning, false, false, false);
        }
    }

    IEnumerator Look()
    {
        //if (currentBehaviorState == GuardBehaviorState.Searching)
        //{
            looking = true;
        //}
        //if (currentBehaviorState == GuardBehaviorState.Searching)
        //{
            yield return new WaitForSeconds(1f);
        //}
        //if (currentBehaviorState == GuardBehaviorState.Searching)
        //{
            guardController.Flip();
        //}
        //if (currentBehaviorState == GuardBehaviorState.Searching)
        //{
            yield return new WaitForSeconds(1f);
        //}
        //if (currentBehaviorState == GuardBehaviorState.Searching)
        //{
            guardController.Flip();
        //}
        //if (currentBehaviorState == GuardBehaviorState.Searching)
        //{
            currentBehaviorState = GuardBehaviorState.Patrolling;
        //}
        //if (currentBehaviorState == GuardBehaviorState.Searching)
        //{
            looking = false;
        //}
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            looking = false;
            if (lookCoroutine != null) {
                StopCoroutine(lookCoroutine);
            }
            bool isHiding = player.GetComponent<PlayerMovement>().isHiding;
            float distance = (transform.position - col.transform.position).magnitude;
            if (isHiding) {
                playerInVision = false;
            }
            else if (distance > attackRange)
            {
                playerInVision = true;
                investigationTarget = player.transform.position;
                destination.position = investigationTarget;
                currentBehaviorState = GuardBehaviorState.Investigating;
            } 
            else
            {
                playerInVision = true;
                currentBehaviorState = GuardBehaviorState.Chasing;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInVision = false;
        }
    }
}
