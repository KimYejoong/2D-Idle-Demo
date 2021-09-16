using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickButton : MonoBehaviour
{
    public GameObject effect;

    public void OnClick()
    {        
        DataController.Instance.gold += DataController.Instance.goldPerClick;
        
        var effect = EffectManager.Instance.GetObject();
        effect.Initialize();
        Vector2 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        effect.transform.position = temp;

    }
}
