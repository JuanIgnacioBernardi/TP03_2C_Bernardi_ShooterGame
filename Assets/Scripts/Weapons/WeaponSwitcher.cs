using UnityEngine;
using UnityEngine.InputSystem;
public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private WeaponBase[] weapons;
    private int currentIndex = 0;
    private void Start()
    {
        EquipWeapon(currentIndex);
    }
    private void Update()
    {
        HandleScrollSwitch();
        HandleKeySwitch();
    }
    private void HandleScrollSwitch()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0f)
            EquipWeapon((currentIndex - 1 + weapons.Length) % weapons.Length);
        else if (scroll < 0f)
            EquipWeapon((currentIndex + 1) % weapons.Length);
    }
    private void HandleKeySwitch()
    {
        Keyboard kb = Keyboard.current;

        if (kb.digit1Key.wasPressedThisFrame) EquipWeapon(0);
        if (kb.digit2Key.wasPressedThisFrame) EquipWeapon(1);
        if (kb.digit3Key.wasPressedThisFrame) EquipWeapon(2);
    }
    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        if (index == currentIndex && weapons[index].gameObject.activeSelf) return;

        weapons[currentIndex].gameObject.SetActive(false);
        currentIndex = index;
        weapons[currentIndex].gameObject.SetActive(true);
    }
    public WeaponBase CurrentWeapon => weapons[currentIndex];
}