using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public Text characterDisplayText;
    public CanvasGroup canvasGroup;
    public Slider slider;

    public Color colorAvailable = Color.green;
    public Color colorUnavailable = Color.red;

    public Image colorImage;

    public string characterName;
    public int level;

    [HideInInspector]
    public int currentCost;
    public int initialCurrentCost = 1;    

    [HideInInspector]
    public int goldPerSec;
    public int initialGoldPerSec = 1;

    public float upgradePower = 1.05f;
    public float costPower = 4.1f;

    [HideInInspector]
    public bool isPurchased = false;


    private void Start()
    {
        DataController.Instance.LoadCharacterButton(this);
        StartCoroutine(AddGoldLoop());
        UpdateUI();
    }

    public void PurchaseCharacter()
    {
        if (DataController.Instance.gold >= currentCost)
        {
            isPurchased = true;
            DataController.Instance.gold -= currentCost;
            level++;

            UpdateCharacter();
            UpdateUI();
            DataController.Instance.SaveCharacterButton(this);
        }
    }

    IEnumerator AddGoldLoop()
    {
        while (true)
        {
            if (isPurchased)
            {
                DataController.Instance.gold += goldPerSec * DataController.Instance.GetGoldMultiplier();                
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    public void UpdateCharacter()
    {
        goldPerSec = goldPerSec + initialGoldPerSec * (int)Mathf.Pow(upgradePower, level);
        currentCost = initialCurrentCost * (int)Mathf.Pow(costPower, level);
    }

    public void UpdateUI()
    {
        characterDisplayText.text = characterName + "\nLevel: " + level + "\nCost : " + currentCost + "\nGold Per Second : " + goldPerSec;

        //slider.minValue = 0;
        //slider.maxValue = currentCost;

        slider.value = (float)(DataController.Instance.gold / currentCost);

        if (isPurchased)
            canvasGroup.alpha = 1.0f;
        else
            canvasGroup.alpha = 0.3f;

        if (DataController.Instance.gold >= currentCost)
            colorImage.color = colorAvailable;        
        else
            colorImage.color = colorUnavailable;

    }

    private void Update()
    {
        UpdateUI();
    }
}
