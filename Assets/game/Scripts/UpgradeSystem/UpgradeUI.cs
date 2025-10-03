using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Engine UI")]
    [SerializeField] private Slider engineSlider;
    [SerializeField] private Text engineCostText;
    [SerializeField] private Button engineUpgradeButton;

    [Header("Suspension UI")]
    [SerializeField] private Slider suspensionSlider;
    [SerializeField] private Text suspensionCostText;
    [SerializeField] private Button suspensionUpgradeButton;

    [Header("Tire UI")]
    [SerializeField] private Slider tireSlider;
    [SerializeField] private Text tireCostText;
    [SerializeField] private Button tireUpgradeButton;

    [Header("4WD UI")]
    [SerializeField] private Slider driveSlider;
    [SerializeField] private Text driveCostText;
    [SerializeField] private Button driveUpgradeButton;

    private CarConfigStats config;

    void Start()
    {
        config = upgradeManager.GetComponent<UpgradeManager>().GetComponent<CarConfigStats>(); // simplified in your project
        SetupUI();
    }

    private void SetupUI()
    {
        // Hook up button events
        engineUpgradeButton.onClick.AddListener(() => TryUpgrade(config.accelerationTorque));
        suspensionUpgradeButton.onClick.AddListener(() => TryUpgrade(config.suspensionFrequency));
        tireUpgradeButton.onClick.AddListener(() => TryUpgrade(config.tireGrip));
        driveUpgradeButton.onClick.AddListener(() => TryUpgrade(config.powerDistribution));

        RefreshUI();
    }

    private void TryUpgrade(UpgradeableStat stat)
    {
        if (upgradeManager.TryUpgrade(stat))
            RefreshUI();
    }

    private void RefreshUI()
    {
        // Update sliders
        engineSlider.value = config.accelerationTorque.level;
        suspensionSlider.value = config.suspensionFrequency.level;
        tireSlider.value = config.tireGrip.level;
        driveSlider.value = config.powerDistribution.level;

        // Update cost texts
        engineCostText.text = config.accelerationTorque.CanUpgrade ? $"${config.accelerationTorque.NextCost}" : "MAX";
        suspensionCostText.text = config.suspensionFrequency.CanUpgrade ? $"${config.suspensionFrequency.NextCost}" : "MAX";
        tireCostText.text = config.tireGrip.CanUpgrade ? $"${config.tireGrip.NextCost}" : "MAX";
        driveCostText.text = config.powerDistribution.CanUpgrade ? $"${config.powerDistribution.NextCost}" : "MAX";
    }
}
