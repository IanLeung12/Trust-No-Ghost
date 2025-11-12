using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // For UI references
using TMPro;           // If using TextMeshPro

public class PlayerInteraction : MonoBehaviour
{
    [Header("Player States")]
    public bool holdingCandy = false;
    public bool hasWeapon = false;
    public bool weaponEquipped = false;
    public int weaponUses = 0;
    public float weaponPushForce = 10f;
    public float weaponRange = 8f;

    [Header("Weapon View Model")]
    public GameObject weaponViewModel;
    private Animator weaponAnimator;

    [Header("UI Elements")]
    public RawImage candyIcon;      // Assign candy icon image in inspector
    public RawImage axeIcon;        // Assign axe icon image in inspector
    public TextMeshProUGUI axeUsesText; // Assign text for axe uses

    void Start()
    {
        if (weaponViewModel != null)
        {
            weaponAnimator = weaponViewModel.GetComponent<Animator>();
            weaponViewModel.SetActive(false);
        }

        // Hide UI icons at start
        if (candyIcon != null)
            candyIcon.enabled = false;
        if (axeIcon != null)
            axeIcon.enabled = false;
        if (axeUsesText != null)
            axeUsesText.text = "";

        UpdateInventoryUI();
    }


    void Update()
    {
        // Equip/Unequip weapon
        if (hasWeapon && Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponEquipped = !weaponEquipped;
            if (weaponViewModel != null)
                weaponViewModel.SetActive(weaponEquipped);

            Debug.Log(weaponEquipped ? "Weapon equipped!" : "Weapon unequipped!");
        }

        // Use weapon on left click if equipped
        if (weaponEquipped && Input.GetMouseButtonDown(0))
        {
            UseWeapon();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // PICK UP CANDY
        if (other.CompareTag("Candy"))
        {
            holdingCandy = true;
            Destroy(other.gameObject);
            Debug.Log("Picked up candy!");
            UpdateInventoryUI();
        }

        // PICK UP WEAPON
        else if (other.CompareTag("Weapon"))
        {
            hasWeapon = true;
            weaponUses = 20;
            Destroy(other.gameObject);
            Debug.Log("Picked up weapon!");
            weaponEquipped = false;
            UpdateInventoryUI();
        }

        // DANGEROUS MONSTER
        else if (other.CompareTag("Dangerous"))
        {
            Debug.Log("Touched dangerous monster! Game over.");
            EndHandler endHandler = FindFirstObjectByType<EndHandler>();
            endHandler?.OnDeath();
        }

        // FRIENDLY MONSTER
        else if (other.CompareTag("Friendly"))
        {
            if (holdingCandy)
            {
                EndHandler endHandler = FindFirstObjectByType<EndHandler>();
                endHandler?.OnWin();
            }
            else
            {
                EndHandler endHandler = FindFirstObjectByType<EndHandler>();
                endHandler?.OnDeath();
            }
        }
    }

    void UseWeapon()
    {
        if (!hasWeapon || weaponUses <= 0)
        {
            Debug.Log("Weapon out of uses!");
            hasWeapon = false;
            weaponEquipped = false;
            if (weaponViewModel != null)
                weaponViewModel.SetActive(false);
            UpdateInventoryUI();
            return;
        }

        weaponUses--;
        Debug.Log($"Weapon used! Remaining uses: {weaponUses}");
        UpdateInventoryUI();

        if (weaponAnimator != null)
        {
            weaponAnimator.SetTrigger("Attack");
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, weaponRange);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Friendly") || hit.CompareTag("Dangerous"))
            {
                

                if (hit.CompareTag("Dangerous"))
                {
                    Destroy(hit.gameObject);
                    Debug.Log("Dangerous monster killed!");
                }
                else if (hit.CompareTag("Friendly"))
                {
                    Debug.Log("Friendly monster hit! Game over!");
                    SceneManager.LoadScene("Start Scene");
                    return;
                }
            }
        }

        if (weaponUses <= 0)
        {
            hasWeapon = false;
            weaponEquipped = false;
            if (weaponViewModel != null)
                weaponViewModel.SetActive(false);
            Debug.Log("Weapon depleted!");
            UpdateInventoryUI();
        }
    }

    void UpdateInventoryUI()
    {
        // Candy icon active if player has candy
        if (candyIcon != null)
            candyIcon.enabled = holdingCandy;

        // Axe icon active if player has weapon
        if (axeIcon != null)
            axeIcon.enabled = hasWeapon;

        // Show remaining uses
        if (axeUsesText != null)
            axeUsesText.text = hasWeapon ? weaponUses.ToString() : "";
    }
}
