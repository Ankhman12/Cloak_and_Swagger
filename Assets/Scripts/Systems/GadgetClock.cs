using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetClock : MonoBehaviour
{
    public enum GadgetState { Cloaking, Grapple };
    GadgetState currentGadget;

    public Invisibility playerCloaking;
    public GrapplingGunScript playerGrapple;
    
    public float tickTime = 0.1f;
    public int maxTicks = 4;

    int ticks = -1;
    float timer = 0f;

    private void Start()
    {
        currentGadget = GadgetState.Grapple;
        playerGrapple.gameObject.SetActive(true);
        playerCloaking.CloakOff();

        timer = 0f;
        ticks = -1;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= tickTime) {
            //Reset timer
            timer = 0f;

            //Tick
            ticks = (ticks + 1) % maxTicks;
            //Play tick sound
            //...

            if (ticks == (maxTicks - 1))
            {
                //Switch player abilities
                if (currentGadget == GadgetState.Cloaking)
                {
                    playerCloaking.CloakOff();
                    playerGrapple.gameObject.SetActive(true);
                    currentGadget = GadgetState.Grapple;
                }
                else if (currentGadget == GadgetState.Grapple) 
                {
                    playerGrapple.gameObject.SetActive(false);
                    playerCloaking.CloakOn();
                    currentGadget = GadgetState.Cloaking;
                }
                //Debug.Log("Tack!");
            }
            else {
                //Debug.Log("Tick!");
            }
            //Debug.Log(ticks);
        }

        
        
    }
}
