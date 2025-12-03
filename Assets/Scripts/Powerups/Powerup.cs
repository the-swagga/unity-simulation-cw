using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private string powerupType;

    private void Start()
    {
        powerupType = powerupType.ToLower();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (powerupType)
            {
                case "banana":
                    PlayerController bananaController = other.GetComponent<PlayerController>();

                    if (bananaController != null)
                    {
                        bananaController.BananaPowerup();
                        Destroy(this.gameObject);
                    }

                    break;

                case "hotdog":
                    PlayerController hotdogController = other.GetComponent<PlayerController>();

                    if (hotdogController != null)
                    {
                        hotdogController.HotdogPowerup();
                        Destroy(this.gameObject);
                    }

                    break;

                case "cherry":
                    PlayerController cherryController = other.GetComponent<PlayerController>();
                    PlayerCannon cannon = FindObjectOfType<PlayerCannon>();
                    
                    if (cherryController != null && cannon != null)
                    {
                        cannon.CherryPowerup();
                        Destroy (this.gameObject);
                    }

                    break;
            }
        }
    }
}
