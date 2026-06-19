using UnityEngine;

public class CursorTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        Vector2 mouse = Input.mousePosition;
        float y = Screen.height - mouse.y; // flip Y
        GUI.DrawTexture(new Rect(mouse.x - 2, y - 2, 4, 4), Texture2D.whiteTexture);
    }
}
