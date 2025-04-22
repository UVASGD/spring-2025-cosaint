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
        if (currentAbilitySO == null || player == null) return; // Not initialized yet

        int currentWisdom = player.GetWisdomPoints();
        bool isOwned = ownedAbilityInstance != null && ownedAbilityInstance.CurrentLevel > 0;
        int currentLevel = isOwned ? ownedAbilityInstance.CurrentLevel : 0;

        // --- Update Level Text ---
        if (currentAbilitySO.isOneTimePurchase)
        {
            levelText.text = "One-Time";
        }
        else if (isOwned)
        {
            if (currentLevel >= currentAbilitySO.maxLevel)
            {
                levelText.text = $"Level: MAX ({currentLevel})";
            }
            else
            {
                levelText.text = $"Level: {currentLevel}";
            }
        }
        else
        {
            levelText.text = "Level: 0"; // Not Owned
        }

        // --- Determine Costs ---
        int buyCost = currentAbilitySO.GetCostForLevel(1);
        int upgradeCost = isOwned ? ownedAbilityInstance.GetNextUpgradeCost() : -1; // -1 if not upgradeable
        int sellValue = isOwned ? ownedAbilityInstance.TotalWisdomInvested : 0; // Sell for total invested

        // --- Update Button Visibility and Interactivity ---

        // BUY Button
        buyButton.gameObject.SetActive(!isOwned); // Show if not owned
        if (!isOwned)
        {
            buyButton.interactable = currentWisdom >= buyCost;
            costText.text = $"Cost: {buyCost} WP";
            costText.gameObject.SetActive(true);
        }

        // UPGRADE Button
        bool canUpgrade = isOwned && !currentAbilitySO.isOneTimePurchase && currentLevel < currentAbilitySO.maxLevel;
        upgradeButton.gameObject.SetActive(canUpgrade); // Show if owned, not one-time, and not max level
        if (canUpgrade)
        {
            upgradeButton.interactable = currentWisdom >= upgradeCost;
            costText.text = $"Upgrade Cost: {upgradeCost} WP";
            costText.gameObject.SetActive(true);
        }

        // SELL Button
        bool canSell = isOwned && !currentAbilitySO.isOneTimePurchase; // Can't sell one-time items (or adjust if needed)
        sellButton.gameObject.SetActive(canSell); // Show if owned and not one-time
        if (canSell)
        {
            sellButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Sell ({sellValue} WP)"; // Update sell button text
                                                                                                  // No cost text needed when only sell is visible, hide it unless buy/upgrade are also shown
            if (!buyButton.gameObject.activeSelf && !upgradeButton.gameObject.activeSelf)
            {
                costText.gameObject.SetActive(false);
            }
        }

        // If nothing else sets the cost text, hide it
        if (!buyButton.gameObject.activeSelf && !upgradeButton.gameObject.activeSelf && !sellButton.gameObject.activeSelf)
        {
            costText.gameObject.SetActive(false);
        }
        // Handle the case where only the sell button is active
        else if (!buyButton.gameObject.activeSelf && !upgradeButton.gameObject.activeSelf && sellButton.gameObject.activeSelf)
        {
            costText.gameObject.SetActive(false); // Selling shows value on button
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