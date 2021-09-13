using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text goldDisplayText;
    public Text goldPerClickDisplayText;
    public Text goldPerSecDisplayText;
    public Text goldDisplayTextInLowerPanel;

    // Update is called once per frame
    void Update()
    {
        goldDisplayText.text = "GOLD : " + DataController.Instance.gold;        
        goldPerClickDisplayText.text = "GOLD PER CLICK: " + DataController.Instance.goldPerClick;
        goldPerSecDisplayText.text = "GOLD PER SECOND : " + DataController.Instance.GetGoldPerSecond();

        goldDisplayTextInLowerPanel.text = "GOLD : " + DataController.Instance.gold;
    }
}

