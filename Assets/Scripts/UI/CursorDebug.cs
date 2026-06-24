using UnityEngine;
using UnityEngine.InputSystem;
public class CursorDebug : MonoBehaviour
{
    private void OnGUI()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        float guiY = Screen.height - mousePos.y;
        GUI.color = Color.red;
        GUI.DrawTexture(new Rect(mousePos.x - 4, guiY - 4, 8, 8), Texture2D.whiteTexture);
    }
}