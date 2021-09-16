using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickButton : MonoBehaviour
{

    public GameObject effect;

    private float m_fNextAction = 0f;
    private float m_fInputActionsPerSecond = 8f; // 초당 최대 입력 가능 횟수 

    private void ComputeNextAction()
    {
        m_fNextAction = Time.unscaledTime + (1f / m_fInputActionsPerSecond);
    }

    private void OnEventClick()
    {
        DataController.Gold += DataController.GoldPerClick;

        var newEffect = EffectManager.Instance.GetObject();
        newEffect.Initialize();
        Vector2 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newEffect.transform.position = temp;
    }

    public void OnClick()
    {
        if (m_fNextAction != 0f && Time.unscaledTime < m_fNextAction) // 초당 클릭 가능 횟수에 상한선 걸어둠
        {
            // Debug.Log("[Skip Click Event] Max Action Per Second");
            return;
        }

        OnEventClick();
        ComputeNextAction();
    }
}
