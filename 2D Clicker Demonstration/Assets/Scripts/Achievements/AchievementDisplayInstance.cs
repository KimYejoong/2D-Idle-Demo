using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AchievementDisplayInstance : MonoBehaviour
{
    [SerializeField]
    private Text textTitle;
    [SerializeField]
    private Text textDescription;
    [SerializeField]
    private Button getRewardButton;
    [SerializeField]
    private Slider slider;

    private Achievement _myAchievement;

    public Achievement MyAchievement => _myAchievement;

    private float _progressPercent;
    private bool _unlocked;

    public void Init(Achievement achievement)
    {
        _myAchievement = achievement;
        textTitle.text = _myAchievement.GetName();
        textDescription.text = _myAchievement.GetDescription();
        _progressPercent = _myAchievement.GetProgressInPercent();
        _unlocked = _myAchievement.GetUnlocked();

        _myAchievement.ChangeInProgress += UpdateUI;
        
        UpdateUI(_progressPercent, _unlocked);
    }
    
    private void UpdateUI(float updatedProgressInPercent, bool updatedUnlocked)
    {
        _progressPercent = updatedProgressInPercent;
        _unlocked = updatedUnlocked;
        getRewardButton.interactable = _unlocked;
        slider.value = (_unlocked == true ? 1 : _progressPercent);
    }
}
