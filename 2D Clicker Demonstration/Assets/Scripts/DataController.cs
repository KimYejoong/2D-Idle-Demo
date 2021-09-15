using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Numerics;
using System.Linq;

public class DataController : MonoBehaviour
{
    #region Singleton
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
    #endregion

    private CharacterButton[] characterButtons; // 부재시 쌓이는 재화 체크 시 자동 재화 생산 담당하는 캐릭터 버튼을 순회하기 위해 배열로 관리
    private SkillButton[] skillButtons;

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

    public double gold
    {
        get
        {
            if (!PlayerPrefs.HasKey("Gold"))
            {
                return 0;
            }

            string tempGold = PlayerPrefs.GetString("Gold");
            return double.Parse(tempGold);
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
        skillButtons = FindObjectsOfType<SkillButton>();
    }

    private void Start()
    {

        GetGoldMultiplierDuringNotPlaying();
        
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

    public void LoadCharacterButton(CharacterButton characterButton)
    {
        string key = characterButton.characterName;
        characterButton.level = PlayerPrefs.GetInt(key + "_level", 0);
        characterButton.currentCost = PlayerPrefs.GetInt(key + "_cost", characterButton.initialCurrentCost);
        characterButton.goldPerSec = PlayerPrefs.GetInt(key + "_goldPerSec");
        characterButton.isPurchased = (PlayerPrefs.GetInt(key + "_isPurchased") == 1);        
    }

    public void SaveCharacterButton(CharacterButton characterButton)
    {
        string key = characterButton.characterName;
        PlayerPrefs.SetInt(key + "_level", characterButton.level);
        PlayerPrefs.SetInt(key + "_cost", characterButton.currentCost);
        PlayerPrefs.SetInt(key + "_goldPerSec", characterButton.goldPerSec);

        if (characterButton.isPurchased)
            PlayerPrefs.SetInt(key + "_isPurchased", 1);
        else
            PlayerPrefs.SetInt(key + "_isPurchased", 0);
    }


    public void LoadSkillButton(SkillButton skillButton)
    {
        string key = skillButton.skillName;
        skillButton.level = PlayerPrefs.GetInt(key + "_level", 0);
        skillButton.currentCost = PlayerPrefs.GetInt(key + "_cost", skillButton.initialCurrentCost);
        skillButton.goldMultiplier = PlayerPrefs.GetFloat(key + "_goldMultiplier");
        skillButton.remaining = PlayerPrefs.GetFloat(key + "_remaining", 0);        
        skillButton.cooldownRemaining = PlayerPrefs.GetFloat(key + "_cooldownRemaining", 0);        
        skillButton.isPurchased = (PlayerPrefs.GetInt(key + "_isPurchased") == 1);
        skillButton.isActivated = (PlayerPrefs.GetInt(key + "_isActivated") == 1);
    }

    public void SaveSkillButton(SkillButton skillButton)
    {
        string key = skillButton.skillName;
        PlayerPrefs.SetInt(key + "_level", skillButton.level);
        PlayerPrefs.SetInt(key + "_cost", skillButton.currentCost);
        PlayerPrefs.SetFloat(key + "_goldMultiplier", skillButton.goldMultiplier);
        PlayerPrefs.SetFloat(key + "_remaining", skillButton.remaining);        
        PlayerPrefs.SetFloat(key + "_cooldownRemaining", skillButton.cooldownRemaining);        

        if (skillButton.isPurchased)
            PlayerPrefs.SetInt(key + "_isPurchased", 1);
        else
            PlayerPrefs.SetInt(key + "_isPurchased", 0);

        if (skillButton.isActivated)
            PlayerPrefs.SetInt(key + "_isActivated", 1);
        else
            PlayerPrefs.SetInt(key + "_isActivated", 0);
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

    public float GetGoldMultiplier()
    {
        float goldMultiplier = 1;

        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (skillButtons[i].isPurchased && skillButtons[i].isActivated) // 해당 스킬을 구매했고, 현재 사용했을 경우에만 고려                
                goldMultiplier *= skillButtons[i].goldMultiplier;
        }

        return goldMultiplier;
    }

    public void GetGoldMultiplierDuringNotPlaying()
    {
        List<SkillData> goldMultipliers = new List<SkillData>();

        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (skillButtons[i].isPurchased) {
                if (skillButtons[i].isActivated)
                {
                    goldMultipliers.Add(new SkillData(skillButtons[i].remaining, skillButtons[i].goldMultiplier)); // 잔여 시간 정보는 필요하니 SkillData로 넘기고
                    skillButtons[i].remaining = Mathf.Max(skillButtons[i].remaining - timeAfterLastPlay, 0); // 지난 시간만큼 해당 스킬 버튼의 잔여 시간, 잔여 재사용 대기 시간을 감소시킴
                }
                skillButtons[i].cooldownRemaining = Mathf.Max(skillButtons[i].cooldownRemaining - timeAfterLastPlay, 0);
                // Debug.Log("cooldown Adust = " + skillButtons[i].cooldownRemaining + ", timeAfterLastPlay = " + timeAfterLastPlay);
            }
        }        

        if (goldMultipliers.Count == 0) // 적용되는 스킬 버프가 따로 없었으면 그냥 지난 시간만큼 바로 처리하고,
        {
            gold += GetGoldPerSecond() * timeAfterLastPlay;
        }
        else // 스킬 버프 적용 중에 종료했다 다시 켠 경우 버프를 고려하여 획득 재화를 계산함
        {               
            float prevRemaining = 0f;

            goldMultipliers = goldMultipliers.OrderBy(skill => skill.remaining).ToList(); // remaining 오름차순으로 정렬하여 버프가 가장 많이 겹치는 구간 ~ 적게 겹치는 구간 순으로 처리

            for (int i = 0; i < goldMultipliers.Count; i++)
            {
                float tempMultiplier = 1f; // 기본 배수를 일단 1배율로 잡아주고,
                float tempRemaining = goldMultipliers[i].remaining - prevRemaining; // i번째 구간만의 remaining을 구함 (이전 구간분 차감)

                for (int j = i; j < goldMultipliers.Count; j++)
                {
                    tempMultiplier *= goldMultipliers[j].multiplier; // 이전 구간을 제외하고(j = i ~ Count), 적용 가능한 버프 계수를 누적해서 곱해줌
                }

                prevRemaining = tempRemaining;
                tempRemaining = Mathf.Min(goldMultipliers[i].remaining, timeAfterLastPlay); // 게임을 껐다 켠지 얼마 안돼서 잔여 시간이 더 긴 경우, 종료 기간분만큼만 계산해줌. 단, 실제 지속 시간은 이미 위에서 처리해줬음

                Debug.Log("Idle time[" + i  + "] = " + tempRemaining + ", tempMultiplier = " + tempMultiplier);               
                Debug.Log("Idle Bonus = " + GetGoldPerSecond() * tempMultiplier * tempRemaining);
                gold += GetGoldPerSecond() * tempMultiplier * tempRemaining;
            }
        }
    }

    class SkillData
    {
        public float remaining;
        public float multiplier;

        public SkillData(float time, float multi)
        {
            remaining = time;
            multiplier = multi;
        }
    }      


}
