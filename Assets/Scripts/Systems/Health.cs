using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int health;

    SpriteRenderer sr;
    IEnumerator coroutine;

    public void Damage(int damage)
    {
        //Damage VFX
        coroutine = BlinkSprite();
        StartCoroutine(coroutine);
        //Damage math
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //death VFX
        //...
        if (this.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
        //Destroy game object
        Destroy(gameObject);
    }

    IEnumerator BlinkSprite()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sr.color = Color.white;
        yield return new WaitForSeconds(.1f);
        sr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sr.color = Color.white;
    }

}
