using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GhostAI : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Assign your player in Inspector

    [Header("AI Settings")]
    public float wanderRadius = 10f;       // How far ghosts wander randomly
    public float wanderInterval = 5f;      // How often to pick a new wander spot
    public float detectionRadius = 10f;    // How close player must be to chase
    public float loseRadius = 15f;         // How far player must be to stop chase
    public float ghostSpeed = 3f;          // Normal movement speed
    public float chaseSpeed = 6f;          // Speed while chasing player

    [Header("Heartbeat Audio")]
    public AudioSource heartbeatAudioSource;
    public AudioClip heartbeatClip;
    public float maxHeartbeatDistance = 15f;  // Max distance for heartbeat
    public float minHeartbeatInterval = 0.3f; // Fastest heartbeat (very close)
    public float maxHeartbeatInterval = 2f;   // Slowest heartbeat (far)
    public float minVolume = 0.1f;            // Volume when far
    public float maxVolume = 0.8f;            // Volume when close

    private NavMeshAgent agent;
    private float wanderTimer;
    private bool isChasing = false;
    private float heartbeatTimer = 0f;
    private static GhostAI closestGhost; // Track which ghost is closest to player

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Automatically find the player if not assigned
        if (!player)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
                player = p.transform;
            else
                Debug.LogWarning("No GameObject with tag 'Player' found in scene!");
        }

        agent.speed = ghostSpeed;
        wanderTimer = wanderInterval;
        
        // Setup heartbeat audio source if not assigned
        if (heartbeatAudioSource == null)
        {
            heartbeatAudioSource = gameObject.AddComponent<AudioSource>();
            heartbeatAudioSource.loop = false;
            heartbeatAudioSource.playOnAwake = false;
            heartbeatAudioSource.spatialBlend = 0f; // 2D sound (not positional)
        }
    }


    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.position);
        
        // Check if this is the closest ghost to the player
        UpdateClosestGhost(distance);
        
        // Only the closest ghost controls heartbeat
        if (closestGhost == this)
        {
            UpdateHeartbeat(distance);
        }

        // --- Chase logic ---
        if (!isChasing && distance <= detectionRadius)
        {
            // Start chasing
            isChasing = true;
            agent.speed = chaseSpeed;
        }
        else if (isChasing && distance > loseRadius)
        {
            // Lost the player, return to wandering
            isChasing = false;
            agent.speed = ghostSpeed;
            wanderTimer = 0f;
        }

        if (isChasing)
        {
            // Constantly update path to player
            agent.SetDestination(player.position);
        }
        else
        {
            // Wander behavior
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval)
            {
                Vector3 newPos = GetRandomNavPosition(transform.position, wanderRadius);
                agent.SetDestination(newPos);
                wanderTimer = 0f;
            }
        }
    }

    private Vector3 GetRandomNavPosition(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, distance, NavMesh.AllAreas))
            return navHit.position;

        return transform.position;
    }

    void UpdateClosestGhost(float distance)
    {
        // If no closest ghost set, or this ghost is closer, make this the closest
        if (closestGhost == null || 
            (player != null && Vector3.Distance(closestGhost.transform.position, player.position) > distance))
        {
            closestGhost = this;
        }
    }

    void UpdateHeartbeat(float distance)
    {
        // Only play heartbeat if within max distance and have audio clip
        if (distance > maxHeartbeatDistance || heartbeatClip == null || heartbeatAudioSource == null)
            return;

        // Calculate heartbeat interval based on distance (closer = faster)
        float normalizedDistance = Mathf.Clamp01(distance / maxHeartbeatDistance);
        float heartbeatInterval = Mathf.Lerp(minHeartbeatInterval, maxHeartbeatInterval, normalizedDistance);
        
        // Calculate volume based on distance (closer = louder)
        float volume = Mathf.Lerp(maxVolume, minVolume, normalizedDistance);

        // Update timer and play heartbeat
        heartbeatTimer += Time.deltaTime;
        if (heartbeatTimer >= heartbeatInterval)
        {
            heartbeatAudioSource.pitch = Random.Range(0.9f, 1.1f); // Slight pitch variation
            heartbeatAudioSource.volume = volume;
            heartbeatAudioSource.PlayOneShot(heartbeatClip);
            heartbeatTimer = 0f;
        }
    }

    void OnDestroy()
    {
        // Clear reference if this ghost was the closest
        if (closestGhost == this)
        {
            closestGhost = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRadius);
    }
}
