using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCannonController : MonoBehaviour
{
    [SerializeField] private KeyCode swapKeybind = KeyCode.F;
    [SerializeField] private PlayerCannon cannon;
    [SerializeField] private PlayerCannon explosiveCannon;
    private bool cannonActive = true;
    private bool explosiveCannonActive = false;
    private PlayerCannon activeWeapon;
    private bool canSwap = true;

    [SerializeField] private ThirdPersonCamera playerCam;
    private bool isAiming;
    public bool GetIsAiming() { return isAiming; }

    private void Start()
    {
        if (cannonActive)
        {
            cannon.gameObject.SetActive(true);
            activeWeapon = cannon;
        } else cannon.gameObject.SetActive(false);

        if (explosiveCannonActive)
        {
            explosiveCannon.gameObject.SetActive(true);
            activeWeapon = explosiveCannon;
        }
        else explosiveCannon.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleCannonSwitch();
        HandleAim();
    }

    private void HandleCannonSwitch()
    {
        if (Input.GetKeyDown(swapKeybind) && canSwap)
        {
            if (cannonActive)
            {
                cannon.gameObject.SetActive(false);
                explosiveCannon.gameObject.SetActive(true);
                cannonActive = false;
                explosiveCannonActive = true;
                activeWeapon = explosiveCannon;

            }
            else if (explosiveCannonActive)
            {
                explosiveCannon.gameObject.SetActive(false);
                cannon.gameObject.SetActive(true);
                explosiveCannonActive = false;
                cannonActive = true;
                activeWeapon = cannon;
            }
        }
    }

    private void HandleAim()
    {
        isAiming = Input.GetMouseButton(1);
        activeWeapon.SetAim(isAiming, playerCam);
    }

    public void SetCanSwap(bool newCanSwap)
    {
        canSwap = newCanSwap;
    }
}
