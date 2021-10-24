using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickButton : MonoBehaviour
{

    public GameObject effect;

    private float _nextAction = 0f;
    private float _InputActionsPerSecond = 8f; // 초당 최대 입력 가능 횟수 

    private void ComputeNextAction()
    {
        _nextAction = Time.unscaledTime + (1f / _InputActionsPerSecond);
    }

    private void OnEventClick()
    {
        DataController.Gold += DataController.Instance.GoldPerClick;

        var newEffect = EffectManager.Instance.GetObject();
        newEffect.Initialize();
        if (!(Camera.main is null))
        {
            Vector2 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newEffect.transform.position = temp;
        }

        // 클릭에 따른 재화 증가를 인스턴트 텍스트로 표시(화면 좌하단)
        var newInstantText = UIManager.Instance.GenerateInstantText("+"+ DataController.Instance.GoldPerClick.ToCurrencyString(), 
            UIManager.Instance.goldDisplayText.transform, 
            Vector2.right * (UIManager.Instance.goldDisplayText.preferredWidth) + Vector2.up * (UIManager.Instance.goldDisplayText.preferredHeight + 4f)
            , 0.3f, TextAnchor.UpperRight, true);

        newInstantText.myText.fontSize = UIManager.Instance.goldDisplayText.fontSize;
        newInstantText.myText.color = UIManager.Instance.goldDisplayText.color;
        
        // 클릭에 따른 재화 증가를 인스턴트 텍스트로 표시(하단 메뉴 좌상단)
        UIManager.Instance.GenerateInstantText("+"+ DataController.Instance.GoldPerClick.ToCurrencyString(), 
            UIManager.Instance.goldDisplayTextInLowerPanel.transform, 
            Vector2.right * (UIManager.Instance.goldDisplayTextInLowerPanel.preferredWidth) + Vector2.up * (UIManager.Instance.goldDisplayTextInLowerPanel.preferredHeight + 4f)
             , 0.3f, TextAnchor.UpperRight);
        // Prefab의 Text의 할당 사이즈가 반드시 가로 세로 0이어야 상하좌우 정렬에 따라 어긋나지 않음

        // 애니메이션 추가
    }

    public void OnClick()
    {
        if (_nextAction != 0f && Time.unscaledTime < _nextAction) // 초당 클릭 가능 횟수에 상한선 걸어둠
        {
            // Debug.Log("초당 입력 제한 도달");
            return;
        }

        OnEventClick();
        ComputeNextAction();
    }
}
