using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListPanelController : MonoBehaviour
{
    [SerializeField]
    private RectTransform scrollContent;

    private RectTransform rect;
    private Vector2 originalLocation;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalLocation = rect.anchoredPosition;
    }

    public void OpenTab()
    {
        rect.anchoredPosition = originalLocation;
        scrollContent.anchoredPosition = new Vector3(0, 0, 0);
    }

    public void CloseTab()
    {
        rect.anchoredPosition = Vector2.down * 1000f;        
    }
}
