using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Text;
using System.Numerics;
using System.Linq;
using Purchasables;

public class DataController : MonoBehaviour
{
    #region Singleton
    private static DataController _instance;

    public static DataController Instance
    {
        get
        {
            if (_instance != null) 
                return _instance;
            
            _instance = FindObjectOfType<DataController>();

            if (_instance != null)
                return _instance;
            
            var container = new GameObject("DataController");
            _instance = container.AddComponent<DataController>();

            return _instance;
        }
    }
    #endregion

    private CharacterButton[] _characterButtons; // 부재시 쌓이는 재화 체크 시 자동 재화 생산 담당하는 캐릭터 버튼을 순회하기 위해 배열로 관리
    private SkillButton[] _skillButtons;

    private DateTime GetLastPlayDate()
    {
        if (!PlayerPrefs.HasKey("Time")) {
            return DateTime.Now;
        }

        var timeBinaryInString = PlayerPrefs.GetString("Time");
        var timeBinaryInLong = Convert.ToInt64(timeBinaryInString);

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

    public static double Gold
    {
        get
        {
            if (!PlayerPrefs.HasKey("Gold"))
            {
                return 0;
            }

            var tempGold = PlayerPrefs.GetString("Gold");
            return double.Parse(tempGold);
        }
        set => PlayerPrefs.SetString("Gold", value.ToString(CultureInfo.InvariantCulture));
    }
    
    public static int GoldPerClick
    {
        get => PlayerPrefs.GetInt("GoldPerClick", 1);
        set => PlayerPrefs.SetInt("GoldPerClick", value);
    }

    public int TimeAfterLastPlay
    {
        get
        {
            var currentTime = DateTime.Now;
            var lastPlayDate = GetLastPlayDate();

            return (int)currentTime.Subtract(lastPlayDate).TotalSeconds; // 현재 접속 시점 ~ 최종 접속 시점의 시간 간격(초)
        }
    }


    private void Awake()
    {        
        _characterButtons = FindObjectsOfType<CharacterButton>();
        _skillButtons = FindObjectsOfType<SkillButton>();
    }

    private void Start()
    {
        GetGoldMultiplierDuringNotPlaying(); // Awake에서 각 스킬 버튼의 remaining 등의 정보를 로드한 후, DataController의 Start에서 미접속 기간 동안 획득한 재화를 처리하면서 remaining 차감 처리함
        
        InvokeRepeating(nameof(UpdateLastPlayDate), 0f, 5f); // 매 5초마다 최종 접속 시각 기록하도록 함
    }

    public static void LoadUpgradeButton(UpgradeButton upgradeButton)
    {
        var key = upgradeButton.upgradeName;
        upgradeButton.level = PlayerPrefs.GetInt(key + "_level", 1);
        upgradeButton.goldByUpgrade = PlayerPrefs.GetInt(key + "_goldByUpgrade", upgradeButton.initialGoldByUpgrade);
        upgradeButton.currentCost = PlayerPrefs.GetInt(key + "_cost", upgradeButton.initialCurrentCost);
    }

    public static void SaveUpgradeButton(UpgradeButton upgradeButton)
    {
        var key = upgradeButton.upgradeName;
        PlayerPrefs.SetInt(key + "_level", upgradeButton.level);
        PlayerPrefs.SetInt(key + "_goldByUpgrade", upgradeButton.goldByUpgrade);
        PlayerPrefs.SetInt(key + "_cost", upgradeButton.currentCost);
    }

    public static void LoadCharacterButton(CharacterButton characterButton)
    {
        var key = characterButton.characterName;
        characterButton.level = PlayerPrefs.GetInt(key + "_level", 0);
        characterButton.currentCost = PlayerPrefs.GetInt(key + "_cost", characterButton.initialCurrentCost);
        characterButton.goldPerSec = PlayerPrefs.GetInt(key + "_goldPerSec");
        characterButton.isPurchased = (PlayerPrefs.GetInt(key + "_isPurchased") == 1);        
    }

    public static void SaveCharacterButton(CharacterButton characterButton)
    {
        var key = characterButton.characterName;
        PlayerPrefs.SetInt(key + "_level", characterButton.level);
        PlayerPrefs.SetInt(key + "_cost", characterButton.currentCost);
        PlayerPrefs.SetInt(key + "_goldPerSec", characterButton.goldPerSec);

        PlayerPrefs.SetInt(key + "_isPurchased", characterButton.isPurchased ? 1 : 0);
    }


    public static void LoadSkillButton(SkillButton skillButton)
    {
        var key = skillButton.skillName;
        skillButton.level = PlayerPrefs.GetInt(key + "_level", 0);
        skillButton.currentCost = PlayerPrefs.GetInt(key + "_cost", skillButton.initialCurrentCost);
        skillButton.goldMultiplier = PlayerPrefs.GetFloat(key + "_goldMultiplier");
        skillButton.remaining = PlayerPrefs.GetFloat(key + "_remaining", 0);        
        skillButton.cooldownRemaining = PlayerPrefs.GetFloat(key + "_cooldownRemaining", 0);        
        skillButton.isPurchased = (PlayerPrefs.GetInt(key + "_isPurchased") == 1);
        skillButton.isActivated = (PlayerPrefs.GetInt(key + "_isActivated") == 1);
    }

    public static void SaveSkillButton(SkillButton skillButton)
    {
        var key = skillButton.skillName;
        PlayerPrefs.SetInt(key + "_level", skillButton.level);
        PlayerPrefs.SetInt(key + "_cost", skillButton.currentCost);
        PlayerPrefs.SetFloat(key + "_goldMultiplier", skillButton.goldMultiplier);
        PlayerPrefs.SetFloat(key + "_remaining", skillButton.remaining);        
        PlayerPrefs.SetFloat(key + "_cooldownRemaining", skillButton.cooldownRemaining);

        PlayerPrefs.SetInt(key + "_isPurchased", skillButton.isPurchased ? 1 : 0);
        PlayerPrefs.SetInt(key + "_isActivated", skillButton.isActivated ? 1 : 0);
    }


    public int GetGoldPerSecond()
    {
        var goldPerSec = 0;

        for (int i = 0; i < _characterButtons.Length; i++)
        {
            if (_characterButtons[i].isPurchased) // 구매한 아이템일 경우에만 계산에 고려함
                goldPerSec += _characterButtons[i].goldPerSec;

        }

        return goldPerSec;
    }

    public float GetGoldMultiplier()
    {
        float goldMultiplier = 1;

        for (var i = 0; i < _skillButtons.Length; i++)
        {
            if (_skillButtons[i].isPurchased && _skillButtons[i].isActivated) // 해당 스킬을 구매했고, 현재 사용했을 경우에만 고려                
                goldMultiplier *= _skillButtons[i].goldMultiplier;
        }

        return goldMultiplier;
    }

    private void GetGoldMultiplierDuringNotPlaying()
    {
        var goldMultipliers = new List<SkillData>();

        for (var i = 0; i < _skillButtons.Length; i++)
        {
            if (!_skillButtons[i].isPurchased)
                continue;
            
            if (_skillButtons[i].isActivated)
            {
                goldMultipliers.Add(new SkillData(_skillButtons[i].remaining, _skillButtons[i].goldMultiplier)); // 잔여 시간 정보는 필요하니 SkillData로 넘기고
                _skillButtons[i].remaining = Mathf.Max(_skillButtons[i].remaining - TimeAfterLastPlay, 0); // 지난 시간만큼 해당 스킬 버튼의 잔여 시간, 잔여 재사용 대기 시간을 감소시킴
            }
            _skillButtons[i].cooldownRemaining = Mathf.Max(_skillButtons[i].cooldownRemaining - TimeAfterLastPlay, 0);
        }        

        if (goldMultipliers.Count == 0) // 적용되는 스킬 버프가 따로 없었으면 그냥 지난 시간만큼 바로 처리하고,
        {
            Gold += GetGoldPerSecond() * TimeAfterLastPlay;
        }
        else // 스킬 버프 적용 중에 종료했다 다시 켠 경우 버프를 고려하여 획득 재화를 계산함
        {               
            var prevRemaining = 0f;

            goldMultipliers = goldMultipliers.OrderBy(skill => skill.remaining).ToList(); // remaining 오름차순으로 정렬하여 버프가 가장 많이 겹치는 구간 ~ 적게 겹치는 구간 순으로 처리

            for (var i = 0; i < goldMultipliers.Count; i++)
            {
                var tempMultiplier = 1f; // 기본 배수를 일단 1배율로 잡아주고,
                var tempRemaining = goldMultipliers[i].remaining - prevRemaining; // i번째 구간만의 remaining을 구함 (이전 구간분 차감)

                for (var j = i; j < goldMultipliers.Count; j++)
                {
                    tempMultiplier *= goldMultipliers[j].multiplier; // 이전 구간을 제외하고(j = i ~ Count), 적용 가능한 버프 계수를 누적해서 곱해줌
                }

                prevRemaining = tempRemaining;
                tempRemaining = Mathf.Min(goldMultipliers[i].remaining, TimeAfterLastPlay); // 게임을 껐다 켠지 얼마 안돼서 잔여 시간이 더 긴 경우, 종료 기간분만큼만 계산해줌. 단, 실제 지속 시간은 이미 위에서 처리해줬음

                //Debug.Log("Idle time[" + i + "] = " + tempRemaining + ", tempMultiplier = " + tempMultiplier);
                //Debug.Log("Idle Bonus = " + GetGoldPerSecond() * tempMultiplier * tempRemaining);
                Gold += GetGoldPerSecond() * tempMultiplier * tempRemaining;
            }
        }
    }

    private class SkillData
    {
        public readonly float remaining;
        public readonly float multiplier;

        public SkillData(float time, float multi)
        {
            remaining = time;
            multiplier = multi;
        }
    }      


}
