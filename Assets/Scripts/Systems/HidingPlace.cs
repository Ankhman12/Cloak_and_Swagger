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
        spriteRenderer.sortingOrder = -1;
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
        //Debug.Log("I hit: " + collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != null && other.tag == "Player") {
            spriteRenderer.color = defaultColor;
            touchingPlayer = false;   
        }
        //Debug.Log(other.gameObject + " left my trigger");
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
                player.GetComponent<SpriteRenderer>().sortingOrder = -2;
                //player.GetComponent<PlayerMovement>().enabled = false;
                player.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                player.transform.position = hidingLocation.position;
                Physics2D.IgnoreLayerCollision(7, 8, true);
            }
            
        } 
        
    }

    public void UnHidePlayer()
    {
        hidingPlayer = false;
        player.GetComponent<PlayerMovement>().isHiding = false;
        player.GetComponent<SpriteRenderer>().sortingOrder = 0;
        spriteRenderer.color = highlightColor;
        Physics2D.IgnoreLayerCollision(7, 8, false);
        //player.GetComponent<PlayerMovement>().enabled = true;
    }
}

