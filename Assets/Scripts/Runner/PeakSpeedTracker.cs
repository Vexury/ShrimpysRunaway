using UnityEngine;

public class PeakSpeedTracker : MonoBehaviour
{
    [SerializeField] private TrackManager trackManager;

    public float PeakSpeedKmh { get; private set; }

    private void Awake()
    {
        if (trackManager == null)
            trackManager = GetComponent<TrackManager>();
    }

    private void Update()
    {
        if (trackManager == null) return;
        float kmh = trackManager.WorldSpeed * 3.6f;
        if (kmh > PeakSpeedKmh)
            PeakSpeedKmh = kmh;
    }
}
