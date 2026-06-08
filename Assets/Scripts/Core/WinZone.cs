using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WinZone : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
            GameTimer.Instance?.Stop();
    }
}
