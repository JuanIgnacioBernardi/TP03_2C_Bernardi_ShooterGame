using UnityEngine;
public class WeaponCameraSetup : MonoBehaviour
{
    [Header("Camera references")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera weaponCamera;

    [Header("Configuration")]
    [SerializeField] private string weaponLayerName = "Weapon";
    [SerializeField] private float weaponFOV = 60f;
    [SerializeField] private float weaponNearClip = 0.01f;
    private void Awake()
    {
        int weaponLayer = LayerMask.NameToLayer(weaponLayerName);

        if (weaponLayer == -1) return;

        if (mainCamera == null || weaponCamera == null) return;

        SetupMainCamera(weaponLayer);
        SetupWeaponCamera(weaponLayer);
    }
    private void SetupMainCamera(int weaponLayer)
    {
        mainCamera.cullingMask &= ~(1 << weaponLayer);
    }
    private void SetupWeaponCamera(int weaponLayer)
    {
        weaponCamera.clearFlags = CameraClearFlags.Depth;

        weaponCamera.cullingMask = 1 << weaponLayer;
        weaponCamera.fieldOfView = weaponFOV;
        weaponCamera.nearClipPlane = weaponNearClip;
        weaponCamera.farClipPlane = 50f;
        weaponCamera.depth = mainCamera.depth + 1;
        weaponCamera.allowHDR = mainCamera.allowHDR;
        weaponCamera.allowMSAA = mainCamera.allowMSAA;
    }
    public void SetWeaponFOV(float fov)
    {
        weaponFOV = fov;
        if (weaponCamera != null)
            weaponCamera.fieldOfView = fov;
    }
}