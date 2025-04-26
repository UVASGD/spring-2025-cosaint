using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use TextMeshPro for better text rendering

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    public ShopAbilitySO AbilitySO => currentAbilitySO;


    private ShopAbilitySO currentAbilitySO;
    private ShopManager shopManager;
    private Player player; // To check current wisdom points for enabling buttons
    private AbilityBase ownedAbilityInstance; // Reference to the owned instance, if exists

    // Call this to set up the UI element with data
    public void Setup(ShopAbilitySO abilitySO, ShopManager manager, Player playerRef, AbilityBase ownedInstance)
    {
        currentAbilitySO = abilitySO;
        shopManager = manager;
        player = playerRef;
        ownedAbilityInstance = ownedInstance; // Can be null if not owned

        // --- Populate Static Info ---
        nameText.text = currentAbilitySO.abilityName;
        descriptionText.text = currentAbilitySO.description;
        iconImage.sprite = currentAbilitySO.icon;
        // Enable/disable sprite renderer if needed
        iconImage.enabled = (currentAbilitySO.icon != null);

        // --- Add Button Listeners ---
        // Remove previous listeners to prevent duplicates if the UI is refreshed
        buyButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();

        // Add new listeners that call the ShopManager
        buyButton.onClick.AddListener(OnBuyClicked);
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        sellButton.onClick.AddListener(OnSellClicked);

        // --- Update Dynamic Info & Button States ---
        UpdateUI();
    }

    // Updates level, cost, and button visibility/interactivity
    public void UpdateUI()
    {
        if (currentAbilitySO == null || player == null) return;

        int currentWisdom = player.GetWisdomPoints();
        bool isOwned = ownedAbilityInstance != null && ownedAbilityInstance.CurrentLevel > 0;
        int currentLevel = isOwned ? ownedAbilityInstance.CurrentLevel : 0;
        bool slotsAvailable = player.FreeSlots(); // ✅ NEW: Check if slot is available

        // --- Level Text ---
        if (currentAbilitySO.isOneTimePurchase)
        {
            levelText.text = "One-Time";
        }
        else if (isOwned)
        {
            levelText.text = currentLevel >= currentAbilitySO.maxLevel
                ? $"Level: MAX ({currentLevel})"
                : $"Level: {currentLevel}";
        }
        else
        {
            levelText.text = "Level: 0";
        }

        // --- Costs ---
        int buyCost = currentAbilitySO.GetCostForLevel(1);
        int upgradeCost = isOwned ? ownedAbilityInstance.GetNextUpgradeCost() : -1;
        int sellValue = isOwned ? ownedAbilityInstance.TotalWisdomInvested : 0;

        // --- BUY Button ---
        buyButton.gameObject.SetActive(!isOwned);
        if (!isOwned)
        {
            buyButton.interactable = currentWisdom >= buyCost && slotsAvailable;

            // ✅ NEW: Slot limit warning
            costText.text = slotsAvailable
                ? $"Cost: {buyCost} WP"
                : "Max slots reached";

            costText.gameObject.SetActive(true);
        }

        // --- UPGRADE Button ---
        bool canUpgrade = isOwned && !currentAbilitySO.isOneTimePurchase && currentLevel < currentAbilitySO.maxLevel;
        upgradeButton.gameObject.SetActive(canUpgrade);
        if (canUpgrade)
        {
            upgradeButton.interactable = currentWisdom >= upgradeCost;
            costText.text = $"Upgrade Cost: {upgradeCost} WP";
            costText.gameObject.SetActive(true);
        }

        // --- SELL Button ---
        bool canSell = isOwned && !currentAbilitySO.isOneTimePurchase;
        sellButton.gameObject.SetActive(canSell);
        if (canSell)
        {
            sellButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Sell ({sellValue} WP)";
            if (!buyButton.gameObject.activeSelf && !upgradeButton.gameObject.activeSelf)
            {
                costText.gameObject.SetActive(false);
            }
        }

        // --- Final fallback: hide cost text if nothing should show it
        if (!buyButton.gameObject.activeSelf && !upgradeButton.gameObject.activeSelf && !sellButton.gameObject.activeSelf)
        {
            costText.gameObject.SetActive(false);
        }
    }


    // --- Button Click Handlers ---
    private void OnBuyClicked()
    {
        if (shopManager != null && currentAbilitySO != null)
        {
            shopManager.TryBuyAbility(currentAbilitySO, this);
        }
    }

    private void OnUpgradeClicked()
    {
        if (shopManager != null && ownedAbilityInstance != null) // Need the instance to upgrade
        {
            shopManager.TryUpgradeAbility(ownedAbilityInstance, this);
        }
    }

    private void OnSellClicked()
    {
        if (shopManager != null && ownedAbilityInstance != null) // Need the instance to sell
        {
            shopManager.TrySellAbility(ownedAbilityInstance, this);
        }
    }

    // Allows ShopManager to update the reference if the ability is bought/sold
    public void SetOwnedAbilityInstance(AbilityBase instance)
    {
        ownedAbilityInstance = instance;
        UpdateUI(); // Refresh UI after change
    }
}