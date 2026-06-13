using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private float hoverAmplitude = 0.2f;
    [SerializeField] private float hoverSpeed = 1.5f;
    [SerializeField] private float spinSpeed = 90f;

    public static event Action<int> OnCollected;
    public static int CollectedCount { get; private set; }

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

        CollectedCount++;
        OnCollected?.Invoke(CollectedCount);
        Destroy(gameObject);
    }

}
