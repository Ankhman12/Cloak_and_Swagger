using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityLaser : MonoBehaviour
{
    public Alarm laserAlarm;
    bool playerInLaser = false;

    void Update()
    {
        if (playerInLaser)
        {
            //trigger alarm
            laserAlarm.TriggerAlarm();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != null && collision.CompareTag("Player"))
        {
            bool isHiding = collision.GetComponent<PlayerMovement>().isHiding;
            bool isInvisible = collision.GetComponent<Invisibility>().getInvisible();

            if (isHiding || isInvisible)
            {
                playerInLaser = false;
            }
            else
            {
                playerInLaser = true;
            }
        }
    }

}
