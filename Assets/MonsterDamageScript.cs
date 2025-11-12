using UnityEngine;

public class DangerousMonster : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        CheckPlayerCollision(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckPlayerCollision(other.gameObject);
    }

    void CheckPlayerCollision(GameObject obj)
    {
        // Only trigger if this monster is tagged as "Dangerous"
        if (tag == "Dangerous" && obj.CompareTag("Player"))
        {
            // Find and trigger the EndHandler death screen
            EndHandler endHandler = FindFirstObjectByType<EndHandler>();
            if (endHandler != null)
            {
                endHandler.OnDeath();
            }
        }
    }
}
