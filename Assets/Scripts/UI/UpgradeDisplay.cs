using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradeDisplay : MonoBehaviour
{
    [SerializeField]
    Enumes.UpgradeType upgradeType;

    [SerializeField]
    IntVariable chloroplastCountVar;

    [SerializeField]
    IntVariable chlorplastProductionVar;

    [SerializeField]
    IntVariable weedCostVar;

    [SerializeField]
    FloatVariable poisonResistanceVar;

    [SerializeField]
    FloatVariable spreadChanceVar;

    [SerializeField]
    TextMeshProUGUI currentValueText;

    [SerializeField]
    TextMeshProUGUI costText;

    [SerializeField]
    TextMeshProUGUI upgradeAmountText;

    [SerializeField]
    Image button;

    [SerializeField]
    Sprite greenButton;

    [SerializeField]
    Sprite redButton;

    [SerializeField]
    Color greenColor;

    [SerializeField]
    int upgradeCost;

    [SerializeField]
    float upgradeAmount;

    [SerializeField]
    int numberOfUpgrades = 1;

    int currentUpgradeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateDisplay()
    {
        costText.text = "Cost: " + upgradeCost;

        switch (upgradeType)
        {
            case Enumes.UpgradeType.ChloroplastProduction:
                currentValueText.text = "Current: " + chlorplastProductionVar.GetValue().ToString();
                upgradeAmountText.text = upgradeAmount.ToString();
                break;

            case Enumes.UpgradeType.PoisonResistance:
                currentValueText.text = "Current: " + Mathf.RoundToInt(poisonResistanceVar.GetValue() * 100).ToString() + "%";
                upgradeAmountText.text = Mathf.RoundToInt((upgradeAmount * 100)).ToString() + "%";
                break;

            case Enumes.UpgradeType.spreadChance:
                currentValueText.text = "Current: " + Mathf.RoundToInt(spreadChanceVar.GetValue() * 100).ToString() + "%";
                upgradeAmountText.text = Mathf.RoundToInt((upgradeAmount * 100)).ToString() + "%";
                break;

            case Enumes.UpgradeType.weedCost:
                currentValueText.text = "Current: " + weedCostVar.GetValue().ToString();
                upgradeAmountText.text = upgradeAmount.ToString();
                break;
        }


        if (currentUpgradeCount >= numberOfUpgrades)
        {
            upgradeAmountText.text = "MAXED!";
            button.gameObject.SetActive(false);
            costText.gameObject.SetActive(false);
            return;
        }


        if (chloroplastCountVar.GetValue() < upgradeCost)
        {
            button.sprite = redButton;
            button.color = Color.white;
        }
        else
        {
            button.sprite = greenButton;
            button.color = greenColor;
        }
    }

    public void PurchaseUpgrade()
    {
        if (currentUpgradeCount >= numberOfUpgrades) return;
        if (chloroplastCountVar.GetValue() >= upgradeCost)
        {
            chloroplastCountVar.DecrementValue(upgradeCost);
            currentUpgradeCount++;

            switch (upgradeType)
            {
                case Enumes.UpgradeType.ChloroplastProduction:
                    chlorplastProductionVar.IncrementValue((int)upgradeAmount);
                    break;

                case Enumes.UpgradeType.PoisonResistance:
                    poisonResistanceVar.IncrementValue(upgradeAmount);
                    break;

                case Enumes.UpgradeType.spreadChance:
                    spreadChanceVar.IncrementValue(upgradeAmount);
                    break;

                case Enumes.UpgradeType.weedCost:
                    weedCostVar.DecrementValue((int)upgradeAmount);
                    break;
            }
        }

        AudioManager.instance.PlayClip(Enumes.AudioClips.beep);
        UpdateDisplay();
    }
}
