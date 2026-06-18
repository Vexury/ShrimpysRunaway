using System;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType { Coin, Sandwich }

public class Collectible : PickupBase
{
    [SerializeField] private CollectibleType collectibleType = CollectibleType.Coin;
    [SerializeField] private bool magnetable = true;
    [SerializeField] private GameObject yellowCoin;
    [SerializeField] private GameObject redCoin;

    public static event Action<CollectibleType, int> OnCollected;
    private static readonly Dictionary<CollectibleType, int> counts = new();

    public static int GetCount(CollectibleType type) => counts.TryGetValue(type, out int n) ? n : 0;
    public static void ResetCounts() => counts.Clear();

    [SerializeField] private int redCoinValue = 3;

    private int coinValue = 1;

    public bool Magnetable   => magnetable;
    public bool MagnetPulled { get; set; }

    protected override bool CanHover => !MagnetPulled;

    public void SetRed(bool red)
    {
        coinValue = red ? redCoinValue : 1;
        if (yellowCoin != null) yellowCoin.SetActive(!red);
        if (redCoin != null) redCoin.SetActive(red);
    }

    protected override bool TryPickup(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null && other.GetComponent<RollerController>() == null) return false;

        counts.TryGetValue(collectibleType, out int current);
        counts[collectibleType] = current + (collectibleType == CollectibleType.Coin ? coinValue : 1);
        OnCollected?.Invoke(collectibleType, counts[collectibleType]);
        return true;
    }
}
