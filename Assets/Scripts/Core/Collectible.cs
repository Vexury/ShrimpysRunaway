using System;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType { Coin, Sandwich }

[RequireComponent(typeof(Collider))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private float hoverAmplitude = 0.2f;
    [SerializeField] private float hoverSpeed = 1.5f;
    [SerializeField] private float spinSpeed = 90f;
    [SerializeField] private CollectibleType collectibleType = CollectibleType.Coin;
    [SerializeField] private AudioClip collectSound;

    public static event Action<CollectibleType, int> OnCollected;
    private static readonly Dictionary<CollectibleType, int> counts = new();

    public static int GetCount(CollectibleType type) => counts.TryGetValue(type, out int n) ? n : 0;
    public static void ResetCounts() => counts.Clear();

    private float baseY;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Start()
    {
        baseY = transform.position.y;
    }

    public bool MagnetPulled { get; set; }

    private void Update()
    {
        if (!MagnetPulled)
        {
            Vector3 pos = transform.position;
            pos.y = baseY + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
            transform.position = pos;
        }

        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null && other.GetComponent<RollerController>() == null) return;

        counts.TryGetValue(collectibleType, out int current);
        counts[collectibleType] = current + 1;
        OnCollected?.Invoke(collectibleType, counts[collectibleType]);
        AudioManager.Instance.PlaySFXWithPitchVariation(collectSound);
        Destroy(gameObject);
    }
}
