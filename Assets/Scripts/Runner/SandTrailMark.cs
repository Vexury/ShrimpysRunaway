using UnityEngine;

public class SandTrailMark : MonoBehaviour
{
    private TrackManager trackManager;
    private Renderer rend;
    private MaterialPropertyBlock block;
    private Color baseColor;

    public void Init(TrackManager tm)
    {
        trackManager = tm;
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
        baseColor = rend.sharedMaterial.GetColor("_BaseColor");
    }

    public void SetAlpha(float t)
    {
        rend.GetPropertyBlock(block);
        block.SetColor("_BaseColor", new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * t));
        rend.SetPropertyBlock(block);
    }

    private void Update()
    {
        transform.position += Vector3.back * trackManager.WorldSpeed * Time.deltaTime;
    }
}
