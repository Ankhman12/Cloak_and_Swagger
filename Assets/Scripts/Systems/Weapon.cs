using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Weapon : MonoBehaviour
{
    public int weaponDamage;
    public float weaponRange;
    public ParticleSystem muzzleFlash;
    public Transform firePoint;
    

    Controls weaponControl;

    LineRenderer laserPointer;

    bool isAiming;

    IEnumerator coroutine;

    public float rotationRadius;
    Vector2 unitVector;

    //Position around which the weapon rotates
    public Transform parentPos;
 

    // Start is called before the first frame update
    void Start()
    {
        weaponControl = new Controls();
        weaponControl.OperatorActionMap.Shoot.performed += Shoot;
        weaponControl.OperatorActionMap.Shoot.Enable();
        weaponControl.OperatorActionMap.Aim.performed += AimStart;
        weaponControl.OperatorActionMap.Aim.canceled += AimStop;
        weaponControl.OperatorActionMap.Aim.Enable();

        gameObject.SetActive(false);

        if (gameObject.CompareTag("Gun"))
        {
            laserPointer = GetComponent<LineRenderer>();
        }
    }

    void FixedUpdate()
    {
        CalculatePosition();
        transform.localPosition = rotationRadius * unitVector;
        transform.up = parentPos.position - transform.position;

        if (gameObject.CompareTag("Gun"))
        {
            DrawLaser();
        }

    }
    void CalculatePosition()
    {
        //Finds the world position of the mouse cursor
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        //Translates the cursor's 3D position into 2D space
        Vector2 mousePos = new Vector2(worldPos.x, worldPos.y);
        Vector2 parentPos2;
        parentPos2 = new Vector2(parentPos.position.x, parentPos.position.y);
        unitVector = (mousePos - parentPos2).normalized;
    }

    void DrawLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.transform.position, -transform.up);
        laserPointer.SetPosition(0, firePoint.transform.position);
        if (hit.collider != null)
        {
            laserPointer.SetPosition(1, hit.point);
        }
        else
        {
            laserPointer.SetPosition(1, 2000 * -transform.up);
        }
    }

    void Shoot(CallbackContext ctx) 
    {
        if (isAiming)
        {
            coroutine = BlinkLaser();
            StartCoroutine(coroutine);

            Instantiate(muzzleFlash, firePoint);

            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, -firePoint.up, weaponRange);
            //Debug.DrawRay(firePoint.position, -firePoint.up * weaponRange, Color.yellow, 1f);
            if (hit && hit.collider != null && hit.collider.gameObject.layer == 7) { 
                GameObject gO = hit.collider.gameObject;
                gO.GetComponent<Health>().Damage(weaponDamage);
            }
        }
    }

    void AimStart(CallbackContext ctx)
    {
        isAiming = true;
        gameObject.SetActive(true);
    }

    void AimStop(CallbackContext ctx) 
    {
        muzzleFlash.Stop();
        isAiming = false;
        gameObject.SetActive(false);
    }

    IEnumerator BlinkLaser() 
    {
        laserPointer.enabled = false;
        yield return new WaitForSeconds(0.3f);
        laserPointer.enabled = true;
    }
}
