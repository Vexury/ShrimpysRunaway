using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text coinLabel;
    [SerializeField] private TMP_Text distanceLabel;
    [SerializeField] private TrackManager trackManager;

    private void OnEnable()
    {
        Collectible.OnCollected += OnCollected;
        CoinWallet.OnTotalChanged += OnWalletChanged;
    }

    private void OnDisable()
    {
        Collectible.OnCollected -= OnCollected;
        CoinWallet.OnTotalChanged -= OnWalletChanged;
    }

    private void Start()
    {
        if (coinLabel != null) coinLabel.text = CoinWallet.Total.ToString();
    }

    private void Update()
    {
        if (trackManager != null && distanceLabel != null)
            distanceLabel.text = $"{trackManager.DistanceTravelled:0} m";
    }

    private void OnCollected(CollectibleType type, int count)
    {
        if (type == CollectibleType.Coin && coinLabel != null)
            coinLabel.text = (CoinWallet.Total + count).ToString();
    }

    private void OnWalletChanged(int total)
    {
        if (coinLabel != null) coinLabel.text = total.ToString();
    }
}
