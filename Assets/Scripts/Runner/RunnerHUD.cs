using TMPro;
using UnityEngine;

public class RunnerHUD : MonoBehaviour
{
    [SerializeField] private RollerController roller;
    [SerializeField] private TMP_Text speedLabel;
    [SerializeField] private string format = "{0:0} km/h";

    private void Update()
    {
        speedLabel.text = string.Format(format, roller.ForwardSpeed * 3.6f);
    }
}
