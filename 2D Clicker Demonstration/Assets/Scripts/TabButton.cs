using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabButton : MonoBehaviour
{
    private LowerMenu lowerMenu;

    [SerializeField]
    private ListPanelController listPanelController;

    private void Awake()
    {
        lowerMenu = GetComponentInParent<LowerMenu>();
    }

    public void OnClick()
    {
        lowerMenu.CloseAllTabs();
        listPanelController.OpenTab();
    }
}
