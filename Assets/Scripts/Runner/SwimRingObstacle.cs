using System;
using UnityEngine;

public class SwimRingObstacle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Hole Trigger")]
    [SerializeField] private Vector3 holeTriggerCenter = new Vector3(0f, -1.25f, 0f);
    [SerializeField] private Vector3 holeTriggerSize   = new Vector3(5f, 7.5f, 1f);

    public static event Action OnPassedThrough;

    private static readonly int ColorB      = Shader.PropertyToID("_ColorB");
    private static readonly int Offset      = Shader.PropertyToID("_Offset");
    private static readonly int StripeCount = Shader.PropertyToID("_StripeCount");

    private void Awake()
    {
        float stripeCount = meshRenderer.sharedMaterial.GetFloat(StripeCount);
        MaterialPropertyBlock mpb = new();
        meshRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(ColorB, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f));
        mpb.SetFloat(Offset, UnityEngine.Random.Range(0f, 1f / stripeCount));
        meshRenderer.SetPropertyBlock(mpb);

        var hole = new GameObject("HoleTrigger");
        hole.transform.SetParent(transform, false);
        hole.transform.localPosition = holeTriggerCenter;
        var col = hole.AddComponent<BoxCollider>();
        col.size = holeTriggerSize;
        col.isTrigger = true;
        hole.AddComponent<SwimRingHoleTrigger>().Init(this);
    }

    public void NotifyPassedThrough() => OnPassedThrough?.Invoke();

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0.4f, 0.35f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(holeTriggerCenter, holeTriggerSize);
        Gizmos.color = new Color(0f, 1f, 0.4f, 0.9f);
        Gizmos.DrawWireCube(holeTriggerCenter, holeTriggerSize);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
