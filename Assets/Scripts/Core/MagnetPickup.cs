using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MagnetPickup : MonoBehaviour
{
    [SerializeField] private float duration = 8f;
    [SerializeField] private float hoverAmplitude = 0.2f;
    [SerializeField] private float hoverSpeed = 1.5f;
    [SerializeField] private float spinSpeed = 90f;
    [SerializeField] private AudioClip collectSound;

    private float baseY;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Start()
    {
        baseY = transform.position.y;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y = baseY + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.position = pos;

        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        MagnetEffect magnet = other.GetComponent<MagnetEffect>();
        if (magnet == null) return;

        magnet.Activate(duration);
        if (collectSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFXWithPitchVariation(collectSound);
        Destroy(gameObject);
    }
}
