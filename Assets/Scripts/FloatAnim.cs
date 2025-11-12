using UnityEngine;

public class FloatAnim : MonoBehaviour
{
    [Header("Spin Settings")]
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Degrees per second

    [Header("Hover Settings")]
    public float hoverAmplitude = 0.5f; // Max height offset
    public float hoverFrequency = 1f;   // Hover speed
    public float lightGrowthRate = 0.1f;
    public Light objlight;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Remember original position
        // Find or add a Light component
        if (objlight == null)
        {
            objlight = GetComponentInChildren<Light>();
            if (objlight == null)
            {
                GameObject lightObj = new GameObject("PointLight");
                objlight = lightObj.AddComponent<Light>();
            }
        }
    }

    void Update()
    {
        // Spin around local Y axis (respects manual rotation)
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);

        // Hover up and down
        float hoverY = startPos.y + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = new Vector3(startPos.x, hoverY, startPos.z);
        
        // Use the objlight variable instead of GetComponent
        if (objlight != null)
        {
            objlight.range += Time.deltaTime * lightGrowthRate;
        }
    }
}
