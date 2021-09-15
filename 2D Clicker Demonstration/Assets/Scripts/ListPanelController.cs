using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class SiblingInfo
{
    public Transform handle;
    public int siblingIndex;

    public SiblingInfo(Transform trans, int index)
    {
        handle = trans;
        siblingIndex = index;
    }
}

public class ListPanelController : MonoBehaviour
{
    [SerializeField]
    private RectTransform scrollContent;

    private RectTransform rect;
    private Vector2 originalLocation;

    [SerializeField]
    private Transform parentPanel;
    [SerializeField]
    private GameObject contents;

    public bool isSorted = false;
    private List<SiblingInfo> originalListInOrder;    

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalLocation = rect.anchoredPosition;

        originalListInOrder = new List<SiblingInfo>();

        foreach (Transform child in parentPanel)
        {
            originalListInOrder.Add(new SiblingInfo(child, child.GetSiblingIndex()));            
        }       
    }

    private void Start()
    {
        // SortContents();
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

    public void SortContents()
    {
        
        var sortedListInOrder = originalListInOrder.OrderBy(item => item.handle.GetComponent<Purchasable>().GetCost()).ToList();

        for (int i = 0; i < sortedListInOrder.Count; i++)
        {
            sortedListInOrder[i].handle.SetSiblingIndex(i);                        
        }

        isSorted = true;
    }

    public void UndoSortContents()
    {
        for (int i = 0; i < originalListInOrder.Count; i++)
        {
            originalListInOrder[i].handle.SetSiblingIndex(i);
        }

        isSorted = false;
    }

    public void TryUpdateSortContents()
    {
        if (isSorted)
        {
            SortContents();
        }
    }
}
