using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour, Purchasable
{
    public Text upgradeDisplayText;

    public string upgradeName;    

    [HideInInspector]
    public int goldByUpgrade;
    public int initialGoldByUpgrade = 1;

    [HideInInspector]
    public int currentCost = 1;
    public int initialCurrentCost = 1;

    [HideInInspector]
    public int level = 1;

    public float upgradePower = 1.05f;
    public float costPower = 1.2f;

    private void Start()
    {
        DataController.Instance.LoadUpgradeButton(this);
        UpdateUI();
    }

    public void PurchaseUpgrade()
    {
        if (DataController.Instance.gold >= currentCost)
        {
            DataController.Instance.gold -= currentCost;
            level++;
            DataController.Instance.goldPerClick += goldByUpgrade;

            UpdateUpgrade();
            UpdateUI();
            DataController.Instance.SaveUpgradeButton(this);
        }
    }

    public void UpdateUpgrade()
    {
        goldByUpgrade = initialGoldByUpgrade * (int)Mathf.Pow(upgradePower, level);
        currentCost = initialCurrentCost * (int)Mathf.Pow(costPower, level);
    }

    public void UpdateUI()
    {
        upgradeDisplayText.text = upgradeName + "\nCost: " + currentCost + "\nLevel: " + level + "\nNext New GoldPerClick : " + goldByUpgrade;
    }

    public int GetCost()
    {
        return currentCost;
    }

}
