using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{

    public GameObject guardPrefab;
    public Alarm camAlarm;

    bool playerInVision = false;

    [SerializeField] protected Vector3 m_from = new Vector3(0.0F, 45.0F, 0.0F);
    [SerializeField] protected Vector3 m_to = new Vector3(0.0F, -45.0F, 0.0F);
    [SerializeField] protected float m_frequency = 1.0F;

    void Update()
    {
        Quaternion from = Quaternion.Euler(this.m_from);
        Quaternion to = Quaternion.Euler(this.m_to);

        float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * this.m_frequency));
        this.transform.localRotation = Quaternion.Lerp(from, to, lerp);

        if (playerInVision) 
        { 
            //trigger alarm

            //spawn guard

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != null && collision.CompareTag("Player")) {
            bool isHiding = collision.GetComponent<PlayerMovement>().isHiding;
            bool isInvisible = collision.GetComponent<Invisibility>().getInvisible();

            if (isHiding && isInvisible)
            {
                playerInVision = false;
            }
            else {
                playerInVision = true;
            }
        }
    }
}
