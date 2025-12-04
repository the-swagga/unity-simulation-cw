using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private string powerupType;

    private Collider col;
    private GameObject powerupContent;

    private void Awake()
    {
        col = GetComponent<Collider>();

        powerupContent = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        powerupType = powerupType.ToLower();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            col.enabled = false;

            switch (powerupType)
            {
                case "banana":
                    PlayerController bananaController = other.GetComponent<PlayerController>();

                    if (bananaController != null)
                    {
                        bananaController.BananaPowerup();
                    }

                    break;

                case "hotdog":
                    PlayerController hotdogController = other.GetComponent<PlayerController>();

                    if (hotdogController != null)
                    {
                        hotdogController.HotdogPowerup();
                    }

                    break;

                case "cherry":
                    PlayerController cherryController = other.GetComponent<PlayerController>();
                    PlayerCannon cannon = FindObjectOfType<PlayerCannon>();
                    
                    if (cherryController != null && cannon != null)
                    {
                        cannon.CherryPowerup();
                    }

                    break;
            }

            powerupContent.SetActive(false);
        }
    }
}
