using UnityEngine;

public class MagnetEffect : MonoBehaviour
{
    [SerializeField] private float radius = 8f;
    [SerializeField] private float pullSpeed = 12f;

    private float remainingTime;

    public bool IsActive => remainingTime > 0f;

    public void Activate(float duration)
    {
        remainingTime = Mathf.Max(remainingTime, duration);
    }

    private void Update()
    {
        if (remainingTime <= 0f) return;

        remainingTime -= Time.deltaTime;

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hits)
        {
            Collectible c = hit.GetComponent<Collectible>();
            if (c == null) continue;

            c.MagnetPulled = true;
            c.transform.position = Vector3.MoveTowards(
                c.transform.position,
                transform.position,
                pullSpeed * Time.deltaTime
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
