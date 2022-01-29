using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class HidingPlace : MonoBehaviour
{
    // Set in editor, where the player will be located while hiding
    public Transform hidingLocation;
    Controls interaction;
    private Color defaultColor;
    private Color highlightColor;
    private GameObject player;

    private SpriteRenderer spriteRenderer;
    private bool touchingPlayer = false;
    private bool hidingPlayer = false;

    private void Start()
    {
        interaction = new Controls();
        interaction.OperatorActionMap.Interact.performed += HidePlayer;
        interaction.OperatorActionMap.Interact.Enable();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        highlightColor = new Color(1, 1, 0, .5f);
        defaultColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != null && collision.tag == "Player")
        {
            if (!hidingPlayer)
            {
                spriteRenderer.color = highlightColor;
            }
            player = collision.gameObject;
            touchingPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        spriteRenderer.color = defaultColor;
        touchingPlayer = false;
    }

    public void HidePlayer(CallbackContext ctx)
    {
        if (touchingPlayer)
        {
            if (hidingPlayer)
            {
                UnHidePlayer();
            } else
            {
                hidingPlayer = true;
                player.GetComponent<PlayerMovement>().isHiding = true;
                spriteRenderer.color = defaultColor;
                //player.GetComponent<PlayerMovement>().enabled = false;
                player.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                player.transform.position = hidingLocation.position;
            }
            
        } 
        
    }

    public void UnHidePlayer()
    {
        hidingPlayer = false;
        player.GetComponent<PlayerMovement>().isHiding = false;
        spriteRenderer.color = highlightColor;
        //player.GetComponent<PlayerMovement>().enabled = true;
    }
}

