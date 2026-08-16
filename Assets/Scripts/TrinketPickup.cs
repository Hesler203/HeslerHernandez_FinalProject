using UnityEngine;

public class TrinketPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SandcastleHealth.IncreaseDefense();
            GameManager.Instance.AudioManager.PlaySound("TrinketPickup");
            Destroy(gameObject);
        }
    }
}
