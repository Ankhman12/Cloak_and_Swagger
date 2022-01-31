using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{

    Controls controls;
    bool canExit;

    float timer = 0f;
    public float goalTimer = 2f;

    public string nextLevelString;

    // Start is called before the first frame update
    void Awake()
    {
        controls = new Controls();
        //Movement controls setup
        controls.OperatorActionMap.Interact.performed += LeaveLevel;
        controls.OperatorActionMap.Interact.Enable();
        canExit = false;
    }

    void LeaveLevel(CallbackContext ctx) 
    {
        if (canExit)
        {
            SceneManager.LoadScene(nextLevelString);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > goalTimer) { 
            canExit = false;
            timer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != null && collision.CompareTag("Player"))
        {
            canExit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != null && collision.CompareTag("Player"))
        {
            canExit = false;
        }
    }
}
