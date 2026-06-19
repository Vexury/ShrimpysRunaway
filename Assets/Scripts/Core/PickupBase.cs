using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class PickupBase : MonoBehaviour
{
    [SerializeField] private float hoverAmplitude = 0.2f;
    [SerializeField] private float hoverSpeed = 1.5f;
    [SerializeField] private float spinSpeed = 90f;
    [SerializeField] protected AudioClip collectSound;

    protected float baseY;

    protected virtual bool CanHover => true;
    protected virtual bool UsePitchVariation => false;

    protected virtual void Awake() => GetComponent<Collider>().isTrigger = true;
    protected virtual void Start() => baseY = transform.position.y;

    protected virtual void Update()
    {
        if (CanHover)
        {
            Vector3 pos = transform.position;
            pos.y = baseY + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
            transform.position = pos;
        }
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
    }

    protected abstract bool TryPickup(Collider other);

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!TryPickup(other)) return;
        if (collectSound != null && AudioManager.Instance != null)
        {
            if (UsePitchVariation)
                AudioManager.Instance.PlaySFXWithPitchVariation(collectSound);
            else
                AudioManager.Instance.PlaySFX(collectSound);
        }
        Destroy(gameObject);
    }
}
