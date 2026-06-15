using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunnerHUD : MonoBehaviour
{
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private TMP_Text distanceLabel;

    [SerializeField] private RollerController roller;
    [SerializeField] private Image doubleJumpIcon;
    [SerializeField] private Color iconReadyColor = Color.white;
    [SerializeField] private Color iconGreyedColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private void Update()
    {
        if (trackManager != null && distanceLabel != null)
            distanceLabel.text = $"{trackManager.DistanceTravelled:0} m";

        if (doubleJumpIcon != null && roller != null)
            doubleJumpIcon.color = roller.HasDoubleJump ? iconReadyColor : iconGreyedColor;
    }
}
