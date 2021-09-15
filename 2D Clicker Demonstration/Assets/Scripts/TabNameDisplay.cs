using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabNameDisplay : MonoBehaviour
{
    private Text tabNameDisplayText;

    // Start is called before the first frame update
    private void Awake()
    {
        tabNameDisplayText = GetComponentInChildren<Text>();
    }

    public void UpdateTabName(string tabName)
    {
        tabNameDisplayText.text = tabName;
    }

}
