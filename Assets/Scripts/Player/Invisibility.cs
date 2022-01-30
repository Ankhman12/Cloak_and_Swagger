using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisibility : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprtRend;

    [SerializeField] Color visColor;
    [SerializeField] Color invisColor;

    [SerializeField] bool isInvisible = false;

    private void Start()
    {
        isInvisible = false;
    }

    public void CloakOn() {
        sprtRend.color = invisColor;
        isInvisible = true;
    }

    public void CloakOff() {
        sprtRend.color = visColor;
        isInvisible = false;
    }

    public bool getInvisible() {
        return isInvisible;
    }
}
