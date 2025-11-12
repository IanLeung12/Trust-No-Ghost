using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class MonsterPhysicsHandler : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    private GhostAI ghostAI;
    private bool isFlung = false;

    [Header("Recovery Settings")]
    public float recoveryDelay = 5f; // seconds before AI regains control
    public float recoverySnapRange = 100f; // how far to search for NavMesh to snap back to

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        ghostAI = GetComponent<GhostAI>();

        // Important: Rigidbody should be non-kinematic for physics to work
        rb.isKinematic = false;
    }

    public void Fling(Vector3 explosionOrigin, float explosionForce, float explosionRadius)
    {
        if (isFlung) return;

        isFlung = true;

        // Stop AI and NavMesh temporarily
        if (agent.enabled)
            agent.enabled = false;

        // Apply explosion force
        rb.AddExplosionForce(explosionForce, explosionOrigin, explosionRadius, 2f, ForceMode.Impulse);

        // Start recovery coroutine
        StartCoroutine(RecoverAfterDelay());
    }

    private IEnumerator RecoverAfterDelay()
    {
        yield return new WaitForSeconds(recoveryDelay);

        // Stop physics motion before re-enabling AI
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Try to place the monster back on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, recoverySnapRange, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        else
        {
            Debug.LogWarning($"{name} couldn't find NavMesh nearby; keeping position.");
        }

        // Re-enable NavMeshAgent
        agent.enabled = true;

        // Let the AI chase again
        isFlung = false;
    }
}
