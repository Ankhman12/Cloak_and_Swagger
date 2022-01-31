using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

public class GuardAIBehavior : MonoBehaviour
{
    //Guard movement fields
    public Transform guardTransform;
    public float walkSpeed = 5f;
    public CharacterController2D guardController;

    //Guard weapon fields
    public Transform firePoint;
    public ParticleSystem muzzleFlash;
    public int weaponDamage = 5;
    public float weaponRange = 20f;
    public float attackDistRatio = 0.4f;
    public float fireRate = 0.5f;
    private Coroutine attackCoroutine;
    private float attackTimer;


    //Patrol fields
    public List<Transform> PatrolPoints;
    int patrolIndex;
    float waitTimer;
    bool atPatrolPoint = false;
    public float maxWaitTime = 2f;
    public float patrolRadius = 1f;

    //Guard target fields
    Transform destination;
    [SerializeField] Transform player;

    //private GameObject player;
    private bool playerInVision;
    //Range that chasing state will be entered from
    [SerializeField] private float attackRange = 10f;
    // Instead of changing the vision cone to a square, we could use this to just have a circular radius around the guard in which he will continue chasing the player.
    [SerializeField] private float chasingRange = 15f;
    /** Empty transform child of guard. Set in inspector. Used for tracking the target during investigation phase */
    [SerializeField] private Vector3 investigationTarget;
    /** True when guard is looking back and forth after searching */
    private bool looking = false;
    private Coroutine lookCoroutine;

    public enum GuardBehaviorState { 
        Patrolling,
        Investigating,
        Chasing,
        Attacking,
        Alarm,
        Searching
    };

    public enum GuardMovementState { 
        Walking,
        Running
    };

    GuardMovementState currentMovementState;
    GuardBehaviorState currentBehaviorState;

    //[Header("Events")]
    //[Space]

    //public UnityEvent onTargetSpotted;
    //public UnityEvent onTargetLost;
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
                Debug.Log("patrolling");
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
                    Debug.Log("Investigating");
                    investigationTarget = player.transform.position;
                    //Debug.Log("investTarget: " + investigationTarget);
                    destination.position = investigationTarget;
                    float distanceFromPlayer = (this.transform.position - player.transform.position).magnitude;
                    if (distanceFromPlayer <= attackRange)
                    {
                        currentMovementState = GuardMovementState.Running;
                        currentBehaviorState = GuardBehaviorState.Chasing;
                    }
                } 
                else
                {
                    currentBehaviorState = GuardBehaviorState.Searching;
                }
                break;
            case GuardBehaviorState.Searching:
                Debug.Log("searching");
                atPatrolPoint = false;
                if (!looking)
                {
                    if (Mathf.Abs(this.transform.position.x - investigationTarget.x) < patrolRadius) 
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
                Debug.Log("chasing");
                atPatrolPoint = false;
                Vector2 currentDist = (this.transform.position - player.transform.position);
                if (currentDist.magnitude <= chasingRange && currentDist.magnitude > attackRange)
                {
                    // Move towards player, stop with room to shoot
                    investigationTarget = player.transform.position;
                    destination.position = investigationTarget;
                }
                else if (currentDist.magnitude <= attackRange)
                {
                    currentBehaviorState = GuardBehaviorState.Attacking;
                }
                else
                {
                    currentMovementState = GuardMovementState.Walking;
                    currentBehaviorState = GuardBehaviorState.Searching;
                }
                break;
            case GuardBehaviorState.Attacking:
                Debug.Log("Attacking");
                atPatrolPoint = false;
                if ((this.transform.position - player.transform.position).magnitude <= attackRange)
                {
                    if (attackTimer >= fireRate)
                    {
                        attackTimer = 0f;
                        Attack();
                    }
                    else
                    {
                        attackTimer += Time.fixedDeltaTime;
                    }
                }
                else if ((this.transform.position - player.transform.position).magnitude <= chasingRange)
                {
                    //StopCoroutine(attackCoroutine);
                    currentBehaviorState = GuardBehaviorState.Chasing;
                }
                else
                {
                    currentMovementState = GuardMovementState.Walking;
                    currentBehaviorState = GuardBehaviorState.Searching;
                }
                break;
            case GuardBehaviorState.Alarm:
                Debug.Log("Alarm");
                atPatrolPoint = false;
                if (Mathf.Abs(destination.position.x - guardTransform.position.x) <= patrolRadius) 
                {
                    currentMovementState = GuardMovementState.Walking;
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

        if (playerInVision) {
            OrientTowardsPlayer();
        }
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

    void OrientTowardsPlayer()
    {
        if ((player.transform.position.x - this.transform.position.x) < 0 && guardController.m_FacingRight) 
        {
            Debug.Log("boop1");
            guardController.Flip();
        }
        if ((player.transform.position.x - this.transform.position.x) > 0 && !guardController.m_FacingRight)
        {
            Debug.Log("boop2");
            guardController.Flip();
        }
    }

    void Attack()
    {
        if (currentBehaviorState != GuardBehaviorState.Attacking)
        {
            return;
        }
        Instantiate(muzzleFlash, firePoint);
        Vector2 shootVec = (player.transform.position - this.transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, shootVec, weaponRange);
        Debug.DrawRay(firePoint.position, shootVec * weaponRange, Color.yellow, 1f);
        if (hit && hit.collider != null && hit.collider.CompareTag("Player"))
        {
            hit.collider.gameObject.GetComponent<Health>()
                .Damage(weaponDamage);
        }
    }

    public void HearAlarm(Transform alarmPoint) 
    {
        destination.position = alarmPoint.position;
        currentMovementState = GuardMovementState.Running;
        currentBehaviorState = GuardBehaviorState.Alarm;
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
            bool isInvisible = player.GetComponent<Invisibility>().getInvisible();
            float distance = (transform.position - col.transform.position).magnitude;
            if (isHiding || isInvisible)
            {
                playerInVision = false;
            }
            else if (distance > chasingRange)
            {
                playerInVision = true;
                investigationTarget = player.transform.position;
                destination.position = investigationTarget;
                currentBehaviorState = GuardBehaviorState.Investigating;
            }
            else if (distance > attackRange)
            {
                playerInVision = true;
                currentMovementState = GuardMovementState.Running;
                currentBehaviorState = GuardBehaviorState.Chasing;
            }
            else
            {
                playerInVision = true;
                currentBehaviorState = GuardBehaviorState.Attacking;
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

    private void OnDrawGizmosSelected()
    {
        //if (firePoint != null)
        //{
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chasingRange);
        // }
    }
}
