using UnityEngine;

public class MagnetPickup : PickupBase
{
    [SerializeField] private float duration = 8f;

    protected override bool TryPickup(Collider other)
    {
        MagnetEffect magnet = other.GetComponent<MagnetEffect>();
        if (magnet == null) return false;
        magnet.Activate(duration * UpgradeManager.MagnetDurationMultiplier);
        return true;
    }
}
