using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class AchievementMenuOpenButton : MonoBehaviour
{
    [SerializeField] 
    private AchievementPanelController achievementPanel;

    public void OnClick()
    {
        achievementPanel.Open();
    }
}
