using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffDisplayText : MonoBehaviour
{
    private BuffDisplayManager buffDisplayManager;
    public SkillButton skill;
    public Text displayText;

    public void Initialize(BuffDisplayManager manager, SkillButton skill)
    {
        buffDisplayManager = manager;
        this.skill = skill;
        displayText = GetComponent<Text>();

        UpdateDisplayText();

    }

    public void UpdateDisplayText()
    {
        if (skill.remaining > 0)
        {
            int hour = (int)(skill.remaining / 3600);
            int min = (int)(skill.remaining - hour * 3600) / 60;
            int sec = (int)(skill.remaining % 60);

            string timeText;
            if (hour > 0)
                timeText = string.Format("{0:00}", hour) + " : " + string.Format("{0:00}", min) + " : " + string.Format("{0:00}", sec);
            else
                timeText = string.Format("{0:00}", min) + " : " + string.Format("{0:00}", sec);

            displayText.text = skill.skillName + " : " + "지속 시간 동안 재화 획득량 " + skill.goldMultiplier + "배(" + timeText + ")";
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        buffDisplayManager.UpdateBuffDisplayList(this);        
    }


}
