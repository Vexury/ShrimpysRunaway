using UnityEngine;

public class ShieldPickup : PickupBase
{
    protected override bool TryPickup(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return false;
        int amount = UpgradeManager.LastResortLevel > 0 && health.CurrentHP == 1 ? 2 : 1;
        health.Heal(amount);
        return true;
    }
}
