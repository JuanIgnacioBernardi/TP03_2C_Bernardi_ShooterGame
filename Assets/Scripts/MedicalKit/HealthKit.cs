using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class HealthKit : MonoBehaviour
{
    [SerializeField] private HealthKitDataSO data;
    [SerializeField] private Transform playerTransform;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    [Header("World UI")]
    [SerializeField] private TextMeshPro medicalText;

    private bool collected;
    private bool playerInRange;
    private void Start()
    {
        if (medicalText != null)
            medicalText.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (collected || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool inRange = distance <= data.pickupRange;

        // Show/hide text when entering/exiting range
        if (inRange != playerInRange)
        {
            playerInRange = inRange;
            if (medicalText != null)
                medicalText.gameObject.SetActive(playerInRange);
        }

        // The text always looks at the camera
        if (playerInRange && medicalText != null)
        {
            Vector3 dirToPlayer = playerTransform.position - medicalText.transform.position;
            medicalText.transform.rotation = Quaternion.LookRotation(-dirToPlayer);
        }
        if (!playerInRange) return;
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;

        Collect();
    }
    private void Collect()
    {
        collected = true;

        if (medicalText != null)
            medicalText.gameObject.SetActive(false);

        if (pickupSound != null)
            AudioEvents.RaisePlaySFX(pickupSound);

        PlayerInventory inventory = playerTransform.GetComponent<PlayerInventory>();
        inventory?.AddHealthKit();

        // Wait for the sound to finish before deactivating
        float delay = pickupSound != null ? pickupSound.length : 0f;
        Invoke(nameof(Deactivate), delay);
    }
    private void Deactivate() => gameObject.SetActive(false);
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, data.pickupRange);
    }
}