﻿using System.Collections;
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

    private ListPanelController _listPanelController;

    private void Awake()
    {
        DataController.LoadUpgradeButton(this);
        _listPanelController = GetComponentInParent<ListPanelController>();
    }

    private void Start()
    {        
        UpdateUI();
    }

    public void PurchaseUpgrade()
    {
        if (!(DataController.Gold >= currentCost))
            return;
        
        DataController.Gold -= currentCost;
        level++;
        DataController.Instance.GoldPerClick += goldByUpgrade;

        UpdateUpgrade();
        UpdateUI();

        _listPanelController.TryUpdateSortContents(); // 이미 내용물 정렬 상태일 때에 한해서 갱신된 정보 가지고 재정렬 시도
        DataController.SaveUpgradeButton(this);
    }

    private void UpdateUpgrade()
    {
        goldByUpgrade = initialGoldByUpgrade * (int)Mathf.Pow(upgradePower, level);
        currentCost = initialCurrentCost * (int)Mathf.Pow(costPower, level);
    }

    private void UpdateUI()
    {
        upgradeDisplayText.text = upgradeName + "\nCost: " + currentCost + "\nLevel: " + level + "\nNext New GoldPerClick : " + goldByUpgrade;
    }

    public int GetCost()
    {
        return currentCost;
    }

}
