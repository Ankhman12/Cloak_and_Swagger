using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Alarm : MonoBehaviour
{

    public List<GuardAIBehavior> localGuards;
    Light alarmLight; 
    IEnumerator alarmCoroutine;

    private void Start()
    {
        alarmLight = GetComponentInChildren<Light>();
        alarmLight.enabled = false;
    }

    public void TriggerAlarm() {
        foreach (GuardAIBehavior g in localGuards)
        {
            g.HearAlarm(this.transform);
        }
        alarmCoroutine = Flicker(); 
        StartCoroutine(alarmCoroutine);

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

    IEnumerator Flicker() 
    { 
        alarmLight.enabled = true;
        yield return new WaitForSeconds(1f);
        alarmLight.enabled = false;
        yield return new WaitForSeconds(1f);
        alarmLight.enabled = true;
        yield return new WaitForSeconds(1f);
        alarmLight.enabled = false;
    }

}
