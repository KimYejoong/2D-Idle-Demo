using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, Purchasable
{
    public Text skillDisplayText;
    public CanvasGroup canvasGroup;
    public Slider slider;

    public Color colorAvailable = Color.green;
    public Color colorUnavailable = Color.red;

    public Image colorImage;

    public string skillName;
    public int level;

    [HideInInspector]
    public int currentCost;
    public int initialCurrentCost = 1;

    [HideInInspector]
    public float goldMultiplier;
    public float initialGoldMultiplier = 2f;    

    public float upgradePower = 1.05f;
    public float costPower = 4.1f;

    public float duration = 20f; // 스킬 최대 지속 시간
    public float cooldownDuration = 60f; // 스킬 최대 재사용 대기 시간
    [HideInInspector]
    public float remaining; // 스킬 잔여 지속 시간
    [HideInInspector]
    public float cooldownRemaining; // 스킬 잔여 재사용 대기 시간

    [HideInInspector]
    public bool isActivated = false; // 스킬 사용중인지 여부

    [HideInInspector]
    public bool isPurchased = false;

    private ListPanelController listPanelController;

    private void Awake()
    {
        listPanelController = GetComponentInParent<ListPanelController>();
        DataController.Instance.LoadSkillButton(this);
    }

    private void Start()
    {
        if (isActivated)
            BuffDisplayManager.Instance.AddBuffDisplay(this); // DataController도 Start에서 각 SkillButton의 remaining을 차감 처리하기 때문에 로드 시 순간적으로 잔여 시간 표시될 수 있음

        UpdateUI();
        StartCoroutine(AutoSaveSkillStatus());        
    }

    public void PurchaseSkill()
    {
        if (DataController.Instance.gold >= currentCost)
        {
            isPurchased = true;
            DataController.Instance.gold -= currentCost;
            level++;

            UpdateSkill();
            UpdateUI();

            listPanelController.TryUpdateSortContents(); // 이미 내용물 정렬 상태일 때에 한해서 갱신된 정보 가지고 재정렬 시도
            DataController.Instance.SaveSkillButton(this);
        }
    }

    private void OnApplicationQuit()
    {
        DataController.Instance.SaveSkillButton(this);
    }

    IEnumerator AutoSaveSkillStatus()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            DataController.Instance.SaveSkillButton(this);
        }
    }

    public void UpdateSkill()
    {
        goldMultiplier = initialGoldMultiplier * (int)Mathf.Pow(upgradePower, level);
        currentCost = initialCurrentCost * (int)Mathf.Pow(costPower, level);
    }

    public void UpdateUI()
    {
        skillDisplayText.text = skillName + "\nLevel: " + level + "\nCost : " + currentCost + "\nGold Multiplier: " + goldMultiplier
            + "\nCooldown(remaining/total) : " + cooldownRemaining + "/" + cooldownDuration + "\nDuration(remaining/total)" + remaining + "/" + duration;
        

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
        UpdateSkillTime();
    }

    public void UseSkill()
    {
        if (!isPurchased || isActivated || cooldownRemaining > 0) // 구매하지 않았거나, 이미 사용 중이거나, 재사용 대기 시간이 남아있으면 사용 불가
            return;

        isActivated = true;

        remaining = duration;
        cooldownRemaining = cooldownDuration;
        BuffDisplayManager.Instance.AddBuffDisplay(this);
    }

    void UpdateSkillTime()
    {
        remaining = Mathf.Clamp(remaining - Time.deltaTime, 0, duration);
        cooldownRemaining = Mathf.Clamp(cooldownRemaining - Time.deltaTime, 0, cooldownDuration);

        if (remaining == 0)
            isActivated = false;
    }

    public int GetCost()
    {
        return currentCost;
    }
}
