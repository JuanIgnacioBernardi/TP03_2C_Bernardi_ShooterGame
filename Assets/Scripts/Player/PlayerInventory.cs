using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInventory : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private HealthKitDataSO data;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip healSound;

    private HealthSystem healthSystem;
    private int kitCount;
    private bool isHealing;
    private Coroutine healCoroutine;

    public event Action<int> onKitCountChanged;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }
    private void Update()
    {
        if (kitCount <= 0 || isHealing) return;
        if (!Keyboard.current.qKey.wasPressedThisFrame) return;

        UseHealthKit();
    }
    public void AddHealthKit()
    {
        kitCount++;
        onKitCountChanged?.Invoke(kitCount);
    }
    private void UseHealthKit()
    {
        kitCount--;
        onKitCountChanged?.Invoke(kitCount);

        if (healCoroutine != null) StopCoroutine(healCoroutine);
        healCoroutine = StartCoroutine(HealRoutine());
    }
    private IEnumerator HealRoutine()
    {
        isHealing = true;
        audioSource?.PlayOneShot(healSound);
        float elapsed = 0f;

        while (elapsed < data.healDuration)
        {
            elapsed += Time.deltaTime;
            float healAmount = data.healPerSecond * Time.deltaTime;
            healthSystem.Heal(healAmount);
            yield return null;
        }

        isHealing = false;
        healCoroutine = null;
    }
}