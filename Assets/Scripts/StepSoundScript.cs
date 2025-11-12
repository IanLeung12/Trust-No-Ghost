using UnityEngine;

public class StepSoundScript : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip step;
    public AudioClip jump;
    public AudioClip land;

    [Header("Footstep Settings")]
    public float referenceSpeed = 5f; // speed that maps to baseStepInterval
    public float baseStepInterval = 0.4f; // decreased from 0.6f to increase frequency
    public float minSpeedForSteps = 0.2f; // ignore tiny movement
    public float stepRateMultiplier = 1.3f; // new multiplier to make steps even more frequent
    public float volume = 1f;
    public Vector2 pitchRange = new Vector2(0.9f, 1.15f);

    private CharacterController controller;
    private float stepProgress = 0f;
    private bool wasGrounded = true;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = GetComponentInParent<CharacterController>();
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = GetComponentInChildren<AudioSource>();
            }
        }
    }

    void Update()
    {
        if (controller == null || audioSource == null) return;

        bool grounded = controller.isGrounded;
        Vector3 vel = controller.velocity;
        float horizontalSpeed = new Vector2(vel.x, vel.z).magnitude;

        // Handle jump/land sounds
        if (wasGrounded && !grounded)
        {
            // Leaving ground (jump) – play only if moving upward and a clip exists
            if (jump != null && vel.y > 0.05f)
            {
                PlayOneShot(jump, 1f, 1f);
            }
        }
        else if (!wasGrounded && grounded)
        {
            // Landed
            if (land != null)
            {
                PlayOneShot(land, 1f, 1f);
            }
        }
        wasGrounded = grounded;

        // Footsteps only when grounded and moving enough
        if (grounded && horizontalSpeed > minSpeedForSteps && step != null)
        {
            // Speed factor relative to reference speed
            float speedFactor = Mathf.Clamp(horizontalSpeed / Mathf.Max(0.01f, referenceSpeed), 0.1f, 3f) * stepRateMultiplier;

            // Accumulate progress; faster speed => faster accumulation => more frequent steps
            stepProgress += Time.deltaTime * speedFactor;

            if (stepProgress >= baseStepInterval)
            {
                // Scale volume/pitch slightly with speed
                float pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, Mathf.InverseLerp(0.1f, 2f, speedFactor));
                float vol = volume * Mathf.Clamp01(0.5f + 0.5f * Mathf.InverseLerp(0.1f, 2f, speedFactor));
                PlayOneShot(step, vol, pitch);
                stepProgress = 0f;
            }
        }
        else
        {
            // Not moving or airborne – slowly decay progress to avoid burst on resume
            stepProgress = Mathf.Max(0f, stepProgress - Time.deltaTime * 0.25f);
        }
    }

    private void PlayOneShot(AudioClip clip, float vol, float pitch)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, vol);
    }
}
