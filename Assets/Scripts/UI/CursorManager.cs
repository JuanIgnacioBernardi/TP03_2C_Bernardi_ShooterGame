using UnityEngine;
public class CursorManager : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Vector2 defaultHotspot = Vector2.zero;
    private void Awake()
    {
        SetDefaultCursor();
    }
    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, defaultHotspot, CursorMode.ForceSoftware);
    }
    public void SetCursor(Texture2D cursor, Vector2 hotspot)
    {
        Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
    }
}