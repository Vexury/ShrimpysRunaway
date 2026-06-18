using UnityEngine;
using UnityEngine.UI;

public class ColorTargetSelector : MonoBehaviour
{
    [SerializeField] private Button bodyButton;
    [SerializeField] private Button armorButton;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color deselectedColor = new Color(1f, 1f, 1f, 0.4f);

    private void Start() => Refresh();

    public void SelectBody()
    {
        PlayerConfig.ActiveTarget = PlayerConfig.ColorTarget.Body;
        Refresh();
    }

    public void SelectArmor()
    {
        PlayerConfig.ActiveTarget = PlayerConfig.ColorTarget.Armor;
        Refresh();
    }

    private void Refresh()
    {
        bool bodyActive = PlayerConfig.ActiveTarget == PlayerConfig.ColorTarget.Body;
        bodyButton.image.color  = bodyActive ? selectedColor : deselectedColor;
        armorButton.image.color = bodyActive ? deselectedColor : selectedColor;
    }
}
