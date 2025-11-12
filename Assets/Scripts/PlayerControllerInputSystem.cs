using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerInputSystem : MonoBehaviour
{
    public Camera cam;
    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float lookSpeed = 0.5f;
    public float jumpForce = 4f;
    public float gravity = -40f;
    
    [Header("Energy System")]
    public float maxEnergy = 100f;
    public float energyDrainRate = 25f;  // Energy consumed per second while sprinting
    public float maxEnergyRechargeRate = 15f;  // Energy recharged per second when not sprinting
    public float minEnergyToSprint = 10f;  // Minimum energy required to start sprinting
    private float energyRechargeRate = 0f;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float rotX = 0f;
    private Vector3 velocity;
    private bool isSprinting = false;
    private bool sprintInput = false;
    private float currentEnergy;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();
        currentEnergy = maxEnergy;  // Start with full energy

        controls.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Movement.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Sprint.performed += ctx => sprintInput = true;
        controls.Player.Sprint.canceled += ctx => sprintInput = false;
    }

    void OnEnable() { controls.Enable(); }
    void OnDisable() { controls.Disable(); }

    void Update()
    {
        HandleSprinting();
        HandleEnergyRecharge();
        
        // Movement with sprint speed
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float currentSpeed = isSprinting ? sprintSpeed : speed;
        
        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        velocity.y += gravity * Time.deltaTime;
        
        // Combine horizontal movement with vertical velocity
        Vector3 finalMovement = move * currentSpeed + velocity;
        controller.Move(finalMovement * Time.deltaTime);

        // Look
        float mouseX = lookInput.x * lookSpeed;
        float mouseY = lookInput.y * lookSpeed;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -80f, 80f);
        cam.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            velocity.y = jumpForce;
        }
    }
    
    void HandleSprinting()
    {
        bool isMoving = moveInput.magnitude > 0.1f;
        bool canSprint = currentEnergy >= minEnergyToSprint;
        
        // Start sprinting if conditions are met
        if (sprintInput && isMoving && canSprint && !isSprinting)
        {
            isSprinting = true;
        }
        
        // Stop sprinting if conditions are no longer met
        if (isSprinting && (!sprintInput || !isMoving || currentEnergy <= 0))
        {
            isSprinting = false;
        }
        
        // Consume energy while sprinting
        if (isSprinting)
        {
            currentEnergy -= energyDrainRate * Time.deltaTime;
            currentEnergy = Mathf.Max(0, currentEnergy);
        }
    }
    
    void HandleEnergyRecharge()
    {
        // Recharge energy when not sprinting
        if (!isSprinting && currentEnergy < maxEnergy)
        {
            currentEnergy += energyRechargeRate * Time.deltaTime;
            currentEnergy = Mathf.Min(maxEnergy, currentEnergy);
            energyRechargeRate = Mathf.Min(energyRechargeRate + Time.deltaTime * 10f, maxEnergyRechargeRate);
        }
        else if (isSprinting)
        {
            energyRechargeRate = 0f; // Reset recharge rate when sprinting
        }
    }
    
    // Public method to get current energy (useful for UI)
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }
    
    // Public method to get energy percentage (useful for UI)
    public float GetEnergyPercentage()
    {
        return currentEnergy / maxEnergy;
    }
}
