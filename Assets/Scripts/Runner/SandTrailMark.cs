using UnityEngine;

public class SandTrailMark : MonoBehaviour
{
    private TrackManager trackManager;

    public void Init(TrackManager tm)
    {
        trackManager = tm;
    }

    private void Update()
    {
        transform.position += Vector3.back * trackManager.WorldSpeed * Time.deltaTime;
    }
}
