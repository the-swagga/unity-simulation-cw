using UnityEngine;

public class GrasslandsBoxCollider : MonoBehaviour
{
    private PlayerController player;
    private void OnTriggerStay(Collider other)
    {
        player = other.GetComponent<PlayerController>();
        if (player != null)
            player.Grow();
    }

    private void OnTriggerExit(Collider other)
    {
        player = other.GetComponent<PlayerController>();
        if (player != null)
            player.ShrinkToOriginal();
    }
}
