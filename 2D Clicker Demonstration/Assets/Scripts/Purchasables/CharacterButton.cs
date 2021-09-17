using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Purchasables
{
    public class CharacterButton : MonoBehaviour, Purchasable
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
        public double goldPerSec;
        public int initialGoldPerSec = 1;

        public float upgradePower = 1.05f;
        public float costPower = 4.1f;

        [HideInInspector]
        public bool isPurchased = false;

        private ListPanelController _listPanelController;

        private void Awake()
        {
            DataController.LoadCharacterButton(this);
            _listPanelController = GetComponentInParent<ListPanelController>();
        }

        private void Start()
        {        
            StartCoroutine(AddGoldLoop());
            UpdateUI();
        }

        public void PurchaseCharacter()
        {
            if (!(DataController.Gold >= currentCost))
                return;
            
            isPurchased = true;
            DataController.Gold -= currentCost;
            level++;

            UpdateCharacter();
            UpdateUI();

            _listPanelController.TryUpdateSortContents(); // 이미 내용물 정렬 상태일 때에 한해서 갱신된 정보 가지고 재정렬 시도
            DataController.SaveCharacterButton(this);
        }

        private IEnumerator AddGoldLoop()
        {
            while (true)
            {
                if (isPurchased)
                {
                    DataController.Gold += goldPerSec * DataController.Instance.GetGoldMultiplier();
                }

                yield return new WaitForSeconds(1f);
            }
        }

        private void UpdateCharacter()
        {
            goldPerSec = goldPerSec + initialGoldPerSec * (int)Mathf.Pow(upgradePower, level);
            currentCost = initialCurrentCost * (int)Mathf.Pow(costPower, level);
        }

        private void UpdateUI()
        {
            characterDisplayText.text = characterName + "\nLevel: " + level + "\nCost : " + currentCost + "\nGold Per Second : " + goldPerSec;

            //slider.minValue = 0;
            //slider.maxValue = currentCost;

            slider.value = (float)(DataController.Gold / currentCost);

            canvasGroup.alpha = isPurchased ? 1.0f : 0.3f;
            colorImage.color = DataController.Gold >= currentCost ? colorAvailable : colorUnavailable;

        }

        private void Update()
        {
            UpdateUI();
        }

        public int GetCost()
        {
            return currentCost;
        }
    }
}
