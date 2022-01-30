using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    bool isAlarmSounding;
    public float alarmRadius;

    public List<GuardAIBehavior> localGuards;

    // Start is called before the first frame update
    void Start()
    {
        isAlarmSounding = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlarmSounding) { 
            
        }
    }

    public void TriggerAlarm() { 
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.gameObject != null && other.CompareTag("Guard")) 
        {
            localGuards.Add(other.gameObject.GetComponent<GuardAIBehavior>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != null && other.gameObject != null && other.CompareTag("Guard"))
        {
            localGuards.Remove(other.gameObject.GetComponent<GuardAIBehavior>());
        }
    }


}
