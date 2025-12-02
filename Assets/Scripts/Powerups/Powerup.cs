using UnityEngine;
using UnityEngine.iOS;

public class Powerup : MonoBehaviour
{
    [SerializeField] private string powerupType;

    private PlayerController playerController;
    private PlayerCannon cannon;

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
                        playerController.BananaPowerup();
                        Destroy(this.gameObject);
                    }
                    break;

                case "hotdog":
                    if (playerController != null)
                    {
                        playerController.HotdogPowerup();
                        Destroy(this.gameObject);
                    }
                    break;

                case "cherry":
                    cannon = FindObjectOfType<PlayerCannon>();

                    if (cannon != null)
                    {
                        cannon.CherryPowerup();
                        Destroy (this.gameObject);
                    }
                    break;
            }
        }
    }
}
