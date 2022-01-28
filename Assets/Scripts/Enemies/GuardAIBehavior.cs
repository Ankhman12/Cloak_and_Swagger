using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

public class GuardAIBehavior : MonoBehaviour
{
    public enum GuardBehaviorState { 
        Patrolling,

    }
    
    [Header("Events")]
    [Space]

    public UnityEvent onPlayerSpotted;
    public UnityEvent onPlayerLost;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
