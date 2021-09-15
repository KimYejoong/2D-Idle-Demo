using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDisplayManager : MonoBehaviour
{
    #region Singleton
    private static BuffDisplayManager instance;

    public static BuffDisplayManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BuffDisplayManager>();

                if (instance == null)
                {
                    GameObject container = new GameObject("BuffDisplayManager");
                    instance = container.AddComponent<BuffDisplayManager>();
                }
            }

            return instance;
        }
    }
    #endregion

    [SerializeField]
    private GameObject buffDisplayText; // Prefab
    [SerializeField]
    private int spacing = 3;

    private List<BuffDisplayText> list;

    private void Awake()
    {
        list = new List<BuffDisplayText>();
    }

    public void AddBuffDisplay(SkillButton skill)
    {
        GameObject newBuffDisplayGo = Instantiate(buffDisplayText, transform);
        BuffDisplayText newBuffDisplay = newBuffDisplayGo.GetComponent<BuffDisplayText>();
        newBuffDisplay.Initialize(this, skill);
        newBuffDisplay.transform.position = transform.position + Vector3.down * (newBuffDisplay.displayText.preferredHeight + spacing) * list.Count;

        list.Add(newBuffDisplay);
    }

    public void UpdateBuffDisplayList(BuffDisplayText buffDisplayText)
    {
        list.Remove(buffDisplayText);

        for (int i = 0; i < list.Count; i++)
        {
            list[i].transform.position = transform.position + Vector3.down * (list[i].displayText.preferredHeight + spacing) * i;
        }
    }
    
}
