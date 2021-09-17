using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdleBonusPopupController : MonoBehaviour
{
    [SerializeField]
    private Text idleBonusDisplay;

    void Start()
    {
        var idleTime = DataController.Instance.TimeAfterLastPlay;

        var hour = (int)(idleTime / 3600);
        var min = (int)(idleTime - hour * 3600) / 60;
        var sec = (int)(idleTime % 60);

        string timeText;
        if (hour > 0)
            timeText = hour + "시간 " + min + "분 " + sec + "초";
        else
            timeText = min + "분 " + sec + "초";
        
        idleBonusDisplay.text = "자리를 비운 " +  timeText + " 동안 획득한 재화 : " +
                                DataController.Instance.GetGoldEarnedFromIdle().ToCurrencyString();
    }
}
