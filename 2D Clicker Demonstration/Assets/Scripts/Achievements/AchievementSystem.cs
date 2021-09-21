using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
    #region Singleton
    private static AchievementSystem _instance;

    public static AchievementSystem Instance
    {
        get
        {
            if (_instance != null) 
                return _instance;
            
            _instance = FindObjectOfType<AchievementSystem>();

            if (_instance != null)
                return _instance;
            
            var container = new GameObject("AchievementSystem");
            _instance = container.AddComponent<AchievementSystem>();

            return _instance;
        }
    }
    #endregion

    private GameObject achievementButton;
    private Dictionary<string, Achievement> _achievements = new Dictionary<string, Achievement>();

    private void Awake()
    {
        InitializeAchievements();
        
        DataController.EarnGold += AchieveEarnGold;
        DataController.EarningGoldPerSec += AchieveEarningGoldPerSec;
    }

    private void LoadAllAchievements()
    {
        foreach (var keys in _achievements.Keys)
        {
            _achievements[keys].LoadAchievement();
        }
    }
    
    private void SaveAllAchievements()
    {
        foreach (var keys in _achievements.Keys)
        {
            _achievements[keys].SaveAchievement();
        }
    }
    
    private void OnApplicationQuit()
    {
        SaveAllAchievements();
    }
    
    void InitializeAchievements()
    {
        _achievements.Add("십원", new Achievement("Earned_10_gold", 0, 10, false));
        _achievements.Add("백원", new Achievement("Earned_100_gold", 0, 100, false));
        _achievements.Add("오백원", new Achievement("Earned_500_gold", 0, 500, false));
        
        _achievements.Add("불로소득", new Achievement("Earning_10_gold_per_sec", 0, 10, false));
        _achievements.Add("숨쉬고 돈벌기", new Achievement("Earning_50_gold_per_sec", 0, 50, false));
        _achievements.Add("티끌모아 태산", new Achievement("Earning_150_gold_per_sec", 0, 150, false));
        
        LoadAllAchievements();
    }
    
    private void AchieveEarnGold(double amount)
    {
        if (amount >= 10)
            _achievements["십원"].TryAchieve(amount);
        
        if(amount>= 100)
            _achievements["백원"].TryAchieve(amount);
        
        if(amount>= 500)
            _achievements["오백원"].TryAchieve(amount);
    }

    private void AchieveEarningGoldPerSec(double amount)
    {
        if (amount >= 10)
            _achievements["불로소득"].TryAchieve(amount);
        
        if (amount >= 50)
            _achievements["숨쉬고 돈벌기"].TryAchieve(amount);
        
        if (amount >= 150)
            _achievements["티끌모아 태산"].TryAchieve(amount);
    }
}

public class Achievement
{
    private string _name;
    private double _progress;
    private double _goal;
    private bool _unlocked;

    public Achievement(string name, double progress, double goal, bool unlocked)
    {
        _name = name;
        _progress = progress;
        _goal = goal;
        _unlocked = unlocked;
    }

    public double GetProgress()
    {
        return _progress;
    }
    
    public void TryAchieve(double progress)
    {
        _progress = progress;
        
        if (_unlocked)
            return;

        if (_progress >= _goal)
        {
            _unlocked = true;
            SaveAchievement();
            Debug.Log("Unlocked Achievement = " + _name);
        }
    }
    
    public void LoadAchievement()
    {
        string key = _name;

        _progress = PlayerPrefsExtended.GetDouble(key + "_progress");
        _goal = PlayerPrefsExtended.GetDouble(key + "_goal");
        _unlocked = PlayerPrefs.GetInt(key + "_unlocked") == 1;
    }

    public void SaveAchievement()
    {
        string key = _name;
        
        PlayerPrefsExtended.SetDouble(key + "_progress", _progress);
        PlayerPrefsExtended.SetDouble(key + "_goal", _goal);
        PlayerPrefs.SetInt(key + "_unlocked", _unlocked ? 1 : 0);
    }
    
}