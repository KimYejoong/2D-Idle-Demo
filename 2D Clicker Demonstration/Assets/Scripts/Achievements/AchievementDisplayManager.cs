using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AchievementDisplayManager : MonoBehaviour
{
    [SerializeField] private RectTransform scrollContent;

    [SerializeField] private Transform contentsPanel;

    [SerializeField] private GameObject contents;

    public bool isSorted;
    private List<Transform> _originalListInOrder;
    private Vector2 _originalLocation;

    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _originalLocation = _rect.anchoredPosition;

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        _originalListInOrder = new List<Transform>();
        
        foreach (var key in AchievementSystem.Instance._achievements.Keys)
        {
            var achievementGO = Instantiate(contents, contentsPanel);
            var achievementDisplayHandle = achievementGO.GetComponent<AchievementDisplayInstance>();
            var achievement = AchievementSystem.Instance._achievements[key];
            achievementDisplayHandle.Init(achievement);
            
            _originalListInOrder.Add(achievementGO.transform);
        }
    }

    public void SortContents()
    {
        var sortedListInOrder =
            _originalListInOrder.OrderBy(item => item.GetComponent<AchievementDisplayInstance>()).ToList();

        for (var i = 0; i < sortedListInOrder.Count; i++) sortedListInOrder[i].SetSiblingIndex(i);

        isSorted = true;
    }

    public void UndoSortContents()
    {
        for (var i = 0; i < _originalListInOrder.Count; i++) _originalListInOrder[i].SetSiblingIndex(i);

        isSorted = false;
    }

    public void TryUpdateSortContents()
    {
        if (isSorted) SortContents();
    }
}
