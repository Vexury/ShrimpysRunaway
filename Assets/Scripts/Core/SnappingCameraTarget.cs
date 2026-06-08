using UnityEngine;

public class SnappingCameraTarget : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void LateUpdate()
    {
        transform.position = player.position;

        transform.rotation = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
    }
}
