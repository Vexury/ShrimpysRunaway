using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Texture2D idleCursor;
    [SerializeField] private Texture2D clickCursor;

    private static readonly Vector2 Hotspot = Vector2.zero;

    protected override void Awake()
    {
        base.Awake();
        Cursor.SetCursor(idleCursor, Hotspot, CursorMode.Auto);
    }

    private void Update()
    {
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
            Cursor.SetCursor(clickCursor, Hotspot, CursorMode.Auto);
        else if (mouse.leftButton.wasReleasedThisFrame)
            Cursor.SetCursor(idleCursor, Hotspot, CursorMode.Auto);
    }
}
