using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListPanelSortButton : MonoBehaviour
{
    [SerializeField]
    private ListPanelController listPanelController;
    
    private Text buttonText;

    private void Awake()
    {
        buttonText = GetComponentInChildren<Text>();
        UpdateText();
    }

    public void OnClick()
    {
        if (!listPanelController.isSorted) {
            listPanelController.SortContents();
            listPanelController.isSorted = true;
        }
        else
        {
            listPanelController.UndoSortContents();
            listPanelController.isSorted = false;
        }

        UpdateText();
    }

    private void UpdateText()
    {
        if (!listPanelController.isSorted)
            buttonText.text = "Sort";
        else
            buttonText.text = "Undo";
    }
}
