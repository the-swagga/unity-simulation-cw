using System.Collections;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private string powerupType;

    private PlayerController playerController;

    private void Start()
    {
        powerupType = powerupType.ToLower();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController == null) return;

            switch (powerupType)
            {
                case "banana":
                    if (playerController != null)
                    {
                        Debug.Log("Banana powerup activating");
                        playerController.BananaPowerup();
                        Destroy(this.gameObject);
                    }
                    break;

                case "hotdog":
                    if (playerController != null)
                    {
                        Debug.Log("Hotdog powerup activating");
                        playerController.HotdogPowerup();
                        Destroy(this.gameObject);
                    }
                    break;
            }
        }
    }
}
