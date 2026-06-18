using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Game/Upgrade Config")]
public class UpgradeConfig : ScriptableObject
{
    public int[] headStartCosts    = { 2000 };
    public int[] hpBoostCosts      = { 3000 };
    public int[] coinBoosterCosts  = { 1500 };
    public int[] magnetBoostCosts   = { 100, 200, 400, 600 };
    public int[] lastResortCosts    = { 1000 };
    public int[] fakeUpgradeCosts   = { 1000, 2000, 3000, 4000, 5000 };
}
