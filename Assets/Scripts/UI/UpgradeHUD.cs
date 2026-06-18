using UnityEngine;
using UnityEngine.UI;

public class UpgradeHUD : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image headStartSticker;
    [SerializeField] private Image hpBoostSticker;
    [SerializeField] private Image coinBoostSticker;
    [SerializeField] private Image magnetBoostSticker;
    [SerializeField] private Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private void Update()
    {
        Set(headStartSticker,   UpgradeManager.HeadStartLevel > 0);
        Set(coinBoostSticker,   UpgradeManager.CoinBoosterLevel > 0);
        Set(hpBoostSticker,     UpgradeManager.HPBoostLevel > 0 && !UpgradeManager.HPBoostUsedThisRun);
        Set(magnetBoostSticker, UpgradeManager.MagnetBoostLevel > 0);
    }

    private void Set(Image img, bool active)
    {
        if (img != null) img.color = active ? Color.white : inactiveColor;
    }
}
