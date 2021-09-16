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
    public void Update()
    {
        goldDisplayText.text = "GOLD : " + DataController.Gold.ToCurrencyString();        
        goldPerClickDisplayText.text = "GOLD PER CLICK: " + DataController.GoldPerClick;
        goldPerSecDisplayText.text = "GOLD PER SECOND : " + DataController.Instance.GetGoldPerSecond();

        goldDisplayTextInLowerPanel.text = "GOLD : " + DataController.Gold.ToCurrencyString();
    }
}

