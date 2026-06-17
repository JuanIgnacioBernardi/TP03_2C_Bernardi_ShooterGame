using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HealthKit : MonoBehaviour
{
    [SerializeField] private HealthKitDataSO data;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    [Header("World UI")]
    [SerializeField] private TextMeshPro medicalText;

    private bool collected;
    private bool playerInRange;
    private Transform playerTransform;
    private int playerLayer;
    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }
    private void Start()
    {
        if (medicalText != null)
            medicalText.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (collected || !playerInRange || playerTransform == null) return;

        // Text always faces the player
        if (medicalText != null)
        {
            Vector3 dirToPlayer = playerTransform.position - medicalText.transform.position;
            medicalText.transform.rotation = Quaternion.LookRotation(-dirToPlayer);
        }

        if (!Keyboard.current.eKey.wasPressedThisFrame) return;
        Collect();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (collected || other.gameObject.layer != playerLayer) return;
        playerTransform = other.transform;
        playerInRange = true;
        if (medicalText != null)
            medicalText.gameObject.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != playerLayer) return;
        playerInRange = false;
        playerTransform = null;
        if (medicalText != null)
            medicalText.gameObject.SetActive(false);
    }
    private void Collect()
    {
        collected = true;

        if (medicalText != null)
            medicalText.gameObject.SetActive(false);

        if (pickupSound != null)
            AudioEvents.RaisePlaySFX(pickupSound);

        PlayerInventory inventory = playerTransform?.GetComponent<PlayerInventory>();
        inventory?.AddHealthKit();

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