using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffDisplayText : MonoBehaviour
{
    private BuffDisplayManager buffDisplayManager;
    private SkillButton skill;
    public Text displayText;

    public void Initialize(BuffDisplayManager manager, SkillButton skill)
    {
        buffDisplayManager = manager;
        this.skill = skill;
        displayText = GetComponent<Text>();

        UpdateDisplayText();

        StartCoroutine(CorutineUpdate());
    }

    IEnumerator CorutineUpdate() // 최소 1초 단위만 표시하기 때문에, Update 대신 0.5초 간격 코루틴 사용. 1초 간격도 지장은 없지만 로드 시 remaining 0초 되더라도 좀 늦게 사라져서 0.5초로 수정
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f); 
            UpdateDisplayText();
        }
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
