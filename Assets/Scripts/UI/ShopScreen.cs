using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text walletLabel;
    [SerializeField] private UpgradeEntry[] upgrades;

    public Action OnPurchase;

    [Serializable]
    private struct UpgradeEntry
    {
        public UpgradeType type;
        public TMP_Text levelLabel;
        public TMP_Text costLabel;
        public Button buyButton;
    }

    private void Start()
    {
        Refresh();
    }

    public void Show()
    {
        panel.SetActive(true);
        Refresh();
    }

    public void Hide() => panel.SetActive(false);

    public void OnBuyClicked(int typeIndex)
    {
        var type = (UpgradeType)typeIndex;
        bool success = UpgradeManager.Purchase(type);
        if (success) OnPurchase?.Invoke();
        Refresh();
    }

    private void Refresh()
    {
        walletLabel.text = $"{CoinWallet.Total}";
        foreach (var u in upgrades)
        {
            int level = UpgradeManager.GetLevel(u.type);
            int cost  = UpgradeManager.NextCost(u.type);
            u.levelLabel.text        = $"Lv {level}";
            u.costLabel.text         = cost >= 0 ? cost.ToString() : "MAX";
            u.buyButton.interactable = UpgradeManager.CanPurchase(u.type);
        }
    }
}
