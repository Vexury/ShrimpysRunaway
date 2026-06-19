using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text coinLabel;

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

    private void OnCollected(CollectibleType type, int count)
    {
        if (type == CollectibleType.Coin && coinLabel != null)
            coinLabel.text = (CoinWallet.Total + count).ToString();
    }

    private void OnWalletChanged(int total)
    {
        if (coinLabel != null) coinLabel.text = (total + Collectible.GetCount(CollectibleType.Coin)).ToString();
    }
}
