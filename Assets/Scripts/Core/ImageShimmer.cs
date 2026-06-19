using UnityEngine;
using UnityEngine.UI;

public class ImageShimmer : MonoBehaviour
{
    [SerializeField] private Color baseColor = Color.white;
    [ColorUsage(true, true)]
    [SerializeField] private Color shimmerColor = new Color(2f, 1.6f, 0.1f, 1f);
    [SerializeField] private float shimmerSpeed = 2f;

    private Material mat;

    void Awake()
    {
        var image = GetComponent<Image>();
        mat = new Material(image.material);
        image.material = mat;
    }

    void Update()
    {
        float t = Mathf.Sin(Time.unscaledTime * shimmerSpeed) * 0.5f + 0.5f;
        // LerpUnclamped preserves HDR values above 1; SetColor bypasses the [0,1] clamp on image.color
        mat.SetColor("_Color", Color.LerpUnclamped(baseColor, shimmerColor, t));
    }

    void OnDestroy() => Destroy(mat);
}
