using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerMenuOpenButton : MonoBehaviour
{
    [SerializeField]
    private LowerMenu lowerMenuPanel;

    [SerializeField]
    private GameObject mainButtons;

    public void OpenMenu()
    {
        // lowerMenuPanel.SetActive(true);
        lowerMenuPanel.Open();
        mainButtons.SetActive(false);
    }
}
