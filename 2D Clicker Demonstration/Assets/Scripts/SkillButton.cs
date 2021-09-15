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
        if (isActivated && (remaining - DataController.Instance.timeAfterLastPlay) > 1f) // 게임 끄기 전에 활성화 되어있었을 경우, 다시 켜고도 잔여 시간이 1초를 넘을 경우에 표시 추가함
            StartCoroutine(AddBuffWhenLoaded());
        // Start에서 바로 버프 표시 오브젝트를 생성할 경우, 유휴 시간에 의한 remaining 차감이 DataController의 Start()에서 이뤄지므로 게임 끄기 전의 잔여 시간 정보를 표시하게 됨
        // 따라서 여기서 바로 실행하지 않고, 코루틴으로 Fixed Update 시점까지 강제로 대기한 후 버프 표시 오브젝트를 생성하여 바로 최신화 된 정보를 보여줄 수 있도록 함

        UpdateUI();
        StartCoroutine(AutoSaveSkillStatus());        
    }

    IEnumerator AddBuffWhenLoaded()
    {
        yield return new WaitForFixedUpdate();
        BuffDisplayManager.Instance.AddBuffDisplay(this);
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
