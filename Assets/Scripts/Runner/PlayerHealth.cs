using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 4;
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip deathClip;

    [Header("Armor")]
    [SerializeField] private GameObject[] armorPieces;
    [SerializeField] private Transform armorParent;
    [SerializeField] private float armorLaunchForce = 5f;
    [SerializeField] private float armorDestroyDelay = 3f;

    public static event Action OnDeath;
    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    private int currentHP;
    private float lastDamageTime = float.MinValue;
    private int armorIndex;
    private Vector3[] armorLocalPos;
    private Quaternion[] armorLocalRot;
    private Vector3[] armorLocalScale;

    private void Awake()
    {
        currentHP = maxHP;

        if (armorPieces != null)
        {
            armorLocalPos   = new Vector3[armorPieces.Length];
            armorLocalRot   = new Quaternion[armorPieces.Length];
            armorLocalScale = new Vector3[armorPieces.Length];
            for (int i = 0; i < armorPieces.Length; i++)
            {
                if (armorPieces[i] == null) continue;
                armorLocalPos[i]   = armorPieces[i].transform.localPosition;
                armorLocalRot[i]   = armorPieces[i].transform.localRotation;
                armorLocalScale[i] = armorPieces[i].transform.localScale;
            }
        }
    }

    public void Heal(int amount)
    {
        int prev = currentHP;
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        int healed = currentHP - prev;
        for (int i = 0; i < healed; i++)
            ReattachArmor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Obstacle")) return;
        if (Time.time - lastDamageTime <= damageCooldown) return;

        lastDamageTime = Time.time;
        currentHP--;

        if (AudioManager.Instance != null)
        {
            AudioClip clip = currentHP <= 0 ? deathClip : damageClip;
            if (clip != null) AudioManager.Instance.PlaySFX(clip);
        }

        if (armorPieces != null && armorIndex < armorPieces.Length)
            LaunchArmor(armorPieces[armorIndex++]);

        if (currentHP <= 0)
            OnDeath?.Invoke();
    }

    private void LaunchArmor(GameObject piece)
    {
        if (piece == null) return;
        piece.transform.SetParent(null, true);
        Rigidbody rb = piece.AddComponent<Rigidbody>();
        Vector3 dir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        rb.AddForce(dir * armorLaunchForce, ForceMode.Impulse);
        StartCoroutine(DeactivateAfterDelay(piece, armorDestroyDelay));
    }

    private void ReattachArmor()
    {
        if (armorIndex <= 0) return;
        armorIndex--;
        GameObject piece = armorPieces[armorIndex];
        if (piece == null) return;

        Rigidbody rb = piece.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);

        piece.transform.SetParent(armorParent != null ? armorParent : transform, false);
        piece.transform.localPosition = armorLocalPos[armorIndex];
        piece.transform.localRotation = armorLocalRot[armorIndex];
        piece.transform.localScale    = armorLocalScale[armorIndex];
        piece.SetActive(true);
    }

    private IEnumerator DeactivateAfterDelay(GameObject piece, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (piece != null) piece.SetActive(false);
    }
}
