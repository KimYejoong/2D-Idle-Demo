using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Numerics;

public class DataController : MonoBehaviour
{
    private static DataController instance;

    public static DataController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataController>();

                if (instance == null)
                {
                    GameObject container = new GameObject("DataController");
                    instance = container.AddComponent<DataController>();
                }
            }

            return instance;
        }
    }

    private CharacterButton[] characterButtons;

    DateTime GetLastPlayDate()
    {
        if (!PlayerPrefs.HasKey("Time")) {
            return DateTime.Now;
        }

        string timeBinaryInString = PlayerPrefs.GetString("Time");
        long timeBinaryInLong = Convert.ToInt64(timeBinaryInString);

        return DateTime.FromBinary(timeBinaryInLong);
    }

    private void UpdateLastPlayDate()
    {
        PlayerPrefs.SetString("Time", DateTime.Now.ToBinary().ToString());
    }

    private void OnApplicationQuit()
    {
        UpdateLastPlayDate();
    }

    public BigInteger gold
    {
        get
        {
            if (!PlayerPrefs.HasKey("Gold"))
            {
                return 0;
            }

            string tempGold = PlayerPrefs.GetString("Gold");
            return long.Parse(tempGold);
        }
        set
        {
            PlayerPrefs.SetString("Gold", value.ToString());
        }
    }
    
    public int goldPerClick
    {
        get
        {
            return PlayerPrefs.GetInt("GoldPerClick", 1);
        }
        set
        {
            PlayerPrefs.SetInt("GoldPerClick", value);
        }
    }

    public int timeAfterLastPlay
    {
        get
        {
            DateTime currentTime = DateTime.Now;
            DateTime lastPlayDate = GetLastPlayDate();

            return (int)currentTime.Subtract(lastPlayDate).TotalSeconds; // 현재 접속 시점 ~ 최종 접속 시점의 시간 간격(초)
        }
    }


    private void Awake()
    {        
        characterButtons = FindObjectsOfType<CharacterButton>();        
    }

    private void Start()
    {
        gold += GetGoldPerSecond() * timeAfterLastPlay;
        InvokeRepeating("UpdateLastPlayDate", 0f, 5f);
    }

    public void LoadUpgradeButton(UpgradeButton upgradeButton)
    {
        string key = upgradeButton.upgradeName;
        upgradeButton.level = PlayerPrefs.GetInt(key + "_level", 1);
        upgradeButton.goldByUpgrade = PlayerPrefs.GetInt(key + "_goldByUpgrade", upgradeButton.initialGoldByUpgrade);
        upgradeButton.currentCost = PlayerPrefs.GetInt(key + "_cost", upgradeButton.initialCurrentCost);
    }

    public void SaveUpgradeButton(UpgradeButton upgradeButton)
    {
        string key = upgradeButton.upgradeName;
        PlayerPrefs.SetInt(key + "_level", upgradeButton.level);
        PlayerPrefs.SetInt(key + "_goldByUpgrade", upgradeButton.goldByUpgrade);
        PlayerPrefs.SetInt(key + "_cost", upgradeButton.currentCost);
    }

    public void LoadCharacterButton(CharacterButton itemButton)
    {
        string key = itemButton.characterName;
        itemButton.level = PlayerPrefs.GetInt(key + "_level", 0);
        itemButton.currentCost = PlayerPrefs.GetInt(key + "_cost", itemButton.initialCurrentCost);
        itemButton.goldPerSec = PlayerPrefs.GetInt(key + "_goldPerSec");
        itemButton.isPurchased = (PlayerPrefs.GetInt(key + "_isPurchased") == 1);        
    }

    public void SaveCharacterButton(CharacterButton itemButton)
    {
        string key = itemButton.characterName;
        PlayerPrefs.SetInt(key + "_level", itemButton.level);
        PlayerPrefs.SetInt(key + "_cost", itemButton.currentCost);
        PlayerPrefs.SetInt(key + "_goldPerSec", itemButton.goldPerSec);

        if (itemButton.isPurchased)
            PlayerPrefs.SetInt(key + "_isPurchased", 1);
        else
            PlayerPrefs.SetInt(key + "_isPurchased", 0);
    }

    public int GetGoldPerSecond()
    {
        int goldPerSec = 0;

        for (int i = 0; i < characterButtons.Length; i++)
        {
            if (characterButtons[i].isPurchased) // 구매한 아이템일 경우에만 계산에 고려함
                goldPerSec += characterButtons[i].goldPerSec;

        }

        return goldPerSec;
    }
}
