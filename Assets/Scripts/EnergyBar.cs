using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [Header("UI References")]
    public Image energyFillLeftImage;
    public Image energyFillRightImage;
    public Image energyBackgroundImage;
    
    [Header("Visual Settings")]
    public Color fullEnergyColor = Color.green;
    public Color lowEnergyColor = Color.red;
    public Color emptyEnergyColor = Color.gray;
    [Range(0f, 1f)]
    public float lowEnergyThreshold = 0.3f;
    
    [Header("Animation Settings")]
    public bool smoothTransition = true;
    public float transitionSpeed = 5f;
    
    private PlayerControllerInputSystem playerController;
    private float targetFillAmount = 1f;
    private float currentFillAmount = 1f;
    private CanvasGroup canvasGroup;
    public float invisibleThreshold = 0.02f; // 2% energy
    public float fadeOutAlpha = 0.001f; // More transparent when faded
    public float fadeInAlpha = 1f;
    public float fadeSpeed = 2f;

    void Start()
    {
        // Find the player controller in the scene
        playerController = FindFirstObjectByType<PlayerControllerInputSystem>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        if (playerController == null)
        {
            Debug.LogError("EnergyBar: Could not find PlayerControllerInputSystem in the scene!");
            return;
        }
        
        if (energyFillLeftImage == null || energyFillRightImage == null)
        {
            Debug.LogError("EnergyBar: Energy Fill Images are not assigned!");
            return;
        }
        
        // Initialize the energy bar
        UpdateEnergyBar();
    }

    void Update()
    {
        if (playerController != null && energyFillLeftImage != null && energyFillRightImage != null)
        {
            UpdateEnergyBar();
            HandleFade();
        }
    }

    void UpdateEnergyBar()
    {
        // Get energy percentage from player controller
        targetFillAmount = playerController.GetEnergyPercentage();
        // Smooth transition or instant update
        if (smoothTransition)
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, transitionSpeed * Time.deltaTime);
        }
        else
        {
            currentFillAmount = targetFillAmount;
        }
        // Update fill amount for centered bar
        if (energyFillLeftImage != null && energyFillRightImage != null)
        {
            float halfFill = currentFillAmount * 0.5f;
            energyFillLeftImage.fillAmount = halfFill;
            energyFillRightImage.fillAmount = halfFill;
        }
        // Update color based on energy level
        UpdateEnergyColor();
    }
    
    void UpdateEnergyColor()
    {
        Color targetColor;
        
        if (currentFillAmount <= 0f)
        {
            targetColor = emptyEnergyColor;
        }
        else if (currentFillAmount <= lowEnergyThreshold)
        {
            // Interpolate between empty and low energy colors
            float t = currentFillAmount / lowEnergyThreshold;
            targetColor = Color.Lerp(emptyEnergyColor, lowEnergyColor, t);
        }
        else
        {
            // Interpolate between low and full energy colors
            float t = (currentFillAmount - lowEnergyThreshold) / (1f - lowEnergyThreshold);
            targetColor = Color.Lerp(lowEnergyColor, fullEnergyColor, t);
        }
        
        // Update color for both fill images
        if (energyFillLeftImage != null) energyFillLeftImage.color = targetColor;
        if (energyFillRightImage != null) energyFillRightImage.color = targetColor;
    }

    void HandleFade()
    {
        float energyPercent = playerController.GetEnergyPercentage();
        if (energyPercent <= invisibleThreshold)
        {
            canvasGroup.alpha = 0f;
            return;
        }
        float targetAlpha = (energyPercent >= 0.99f) ? fadeOutAlpha : fadeInAlpha;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }
    
    // Optional: Method to manually set player controller reference
    public void SetPlayerController(PlayerControllerInputSystem controller)
    {
        playerController = controller;
    }
}