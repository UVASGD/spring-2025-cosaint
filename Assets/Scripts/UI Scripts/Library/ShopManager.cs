using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq; // Keep for LINQ

public class ShopManager : MonoBehaviour
{
    [Header("Shop Configuration")]
    [SerializeField] private List<ShopAbilitySO> availableAbilities;
    [SerializeField] private GameObject shopItemUIPrefab;
    [SerializeField] private Transform scrollContentParent;

    [Header("System References")]
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private Player player;
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private AbilityManagerUI abilityManagerUI;

    private List<ShopItemUI> spawnedUIItems = new List<ShopItemUI>();
    [SerializeField] private ShopAbilitySO defaultStartingAbility;

    void Start()
    {
        // Ensure references are set
        if (!abilityManager)
        {
            abilityManager = FindFirstObjectByType<AbilityManager>(); // Use the new API
            if (!abilityManager) Debug.LogError("ShopManager could not find AbilityManager!");
        }
        if (!player)
        {
            player = FindFirstObjectByType<Player>(); // Use the new API
            if (!player) Debug.LogError("ShopManager could not find Player!");
        }
        if (!abilityManagerUI)
        {
            abilityManagerUI = FindFirstObjectByType<AbilityManagerUI>();
            if (!abilityManagerUI) Debug.LogError("ShopManager could not find Ability Manager UI!");

        }

        PopulateShop();

        if (shopPanel && !shopPanel.activeSelf) { }
        else if (!shopPanel) Debug.LogWarning("Shop Panel reference not set in ShopManager.");


        // --- Auto-buy default ability without cost ---
        if (defaultStartingAbility != null && abilityManager.GetAbilityByShopData(defaultStartingAbility) == null)
        {
            AbilityBase newAbility = CreateAbilityInstance(defaultStartingAbility);
            if (newAbility != null)
            {
                newAbility.Purchase(0); // Mark as purchased for 0 WP
                abilityManager.AddAbility(newAbility);

                // Set in ShopItemUI (ensures it's shown as owned and sellable)
                ShopItemUI uiItem = spawnedUIItems.FirstOrDefault(item => item != null && item.AbilitySO == defaultStartingAbility);
                if (uiItem != null)
                {
                    uiItem.SetOwnedAbilityInstance(newAbility); // This will call UpdateUI
                }
                else
                {
                    Debug.LogWarning($"Could not find UI item for default ability '{defaultStartingAbility.abilityName}'.");
                }

                RefreshShopItemStates(); // Make sure UI is synced
                FindFirstObjectByType<LibraryUI>()?.UpdateDisplays();
                Debug.Log($"Granted default ability '{defaultStartingAbility.abilityName}' for free.");
            }
            else
            {
                Debug.LogError($"Failed to create default ability: {defaultStartingAbility.abilityName}");
            }
        }


    }

    public void PopulateShop()
    {
        // ... (Clear existing items) ...
        Debug.Log($"Found {availableAbilities?.Count ?? 0} abilities to process."); // How many SOs?

        foreach (ShopAbilitySO abilitySO in availableAbilities)
        {
            // --- Add Log 1 ---
            Debug.Log($"Processing Ability SO: {abilitySO?.name ?? "NULL SO"}");
            if (abilitySO == null) continue; // Skip if SO is somehow null

            GameObject newItemGO = Instantiate(shopItemUIPrefab, scrollContentParent);
            ShopItemUI shopItemUI = newItemGO.GetComponent<ShopItemUI>();

            if (shopItemUI != null)
            {
                // --- Add Log 2 ---
                Debug.Log($"Got ShopItemUI component for {abilitySO.abilityName}. Finding owned instance...");
                AbilityBase ownedInstance = abilityManager.GetAbilityByShopData(abilitySO);
                Debug.Log($"Owned instance found: {ownedInstance != null}"); // Does player own it?

                // --- Add Log 3 ---
                Debug.Log($"Calling Setup for {abilitySO.abilityName}...");
                shopItemUI.Setup(abilitySO, this, player, ownedInstance);
                // --- Add Log 4 ---
                Debug.Log($"Setup CALLED for {abilitySO.abilityName}.");

                spawnedUIItems.Add(shopItemUI);
            }
            else // Should not happen if prefab is correct
            {
                Debug.LogError($"ShopItemUI Prefab is missing the ShopItemUI script on its root!");
                Destroy(newItemGO);
            }
        }
    }

    public void RefreshShopItemStates()
    {
        foreach (ShopItemUI itemUI in spawnedUIItems) itemUI.UpdateUI();
        abilityManagerUI.RefreshUI();
    }

    public void TryBuyAbility(ShopAbilitySO abilitySO, ShopItemUI requestingUI)
    {
        if (abilitySO == null || player == null || abilityManager == null) return;
        if (abilityManager.GetAbilityByShopData(abilitySO) != null) return;

        // Limit amount of abilities at one time
        if (!player.FreeSlots())
        {
            Debug.Log("Limiting amount of abilities at once");
            return;
        }

        int cost = abilitySO.GetCostForLevel(1);

        if (player.CanAfford(cost))
        {
            if (abilitySO.isOneTimePurchase)
            {
                player.SpendWisdomPoints(cost);
                Debug.Log($"Bought one-time item: {abilitySO.abilityName} for {cost} WP.");
                // Trigger one-time effect here
                requestingUI.gameObject.SetActive(false);
                RefreshShopItemStates();
                // --- UPDATED API CALL ---
                // FindObjectOfType<LibraryUI>()?.UpdateWisdomPointsDisplay(); // Obsolete
                FindFirstObjectByType<LibraryUI>()?.UpdateDisplays(); // Use new API
                return;
            }

            AbilityBase newAbilityInstance = CreateAbilityInstance(abilitySO); // Ensure this method is correctly implemented

            if (newAbilityInstance != null)
            {
                player.SpendWisdomPoints(cost);
                newAbilityInstance.Purchase(cost);
                abilityManager.AddAbility(newAbilityInstance);
                requestingUI.SetOwnedAbilityInstance(newAbilityInstance);
                RefreshShopItemStates();
                // --- UPDATED API CALL ---
                // FindObjectOfType<LibraryUI>()?.UpdateWisdomPointsDisplay(); // Obsolete
                FindFirstObjectByType<LibraryUI>()?.UpdateDisplays(); // Use new API
            }
            else Debug.LogError($"Failed to create instance of {abilitySO.abilityClassName}.");
        }
        else Debug.Log($"Not enough WP to buy {abilitySO.abilityName}.");
    }

    public void TryUpgradeAbility(AbilityBase abilityInstance, ShopItemUI requestingUI)
    {
        if (abilityInstance == null || player == null || abilityInstance.ShopData == null) return;
        if (abilityInstance.CurrentLevel >= abilityInstance.ShopData.maxLevel) return;

        int upgradeCost = abilityInstance.GetNextUpgradeCost();
        if (upgradeCost < 0) return;

        if (player.CanAfford(upgradeCost))
        {
            player.SpendWisdomPoints(upgradeCost);
            bool success = abilityInstance.TryUpgrade(upgradeCost);

            if (success)
            {
                requestingUI.UpdateUI();
                RefreshShopItemStates();
                // --- UPDATED API CALL ---
                // FindObjectOfType<LibraryUI>()?.UpdateWisdomPointsDisplay(); // Obsolete
                FindFirstObjectByType<LibraryUI>()?.UpdateDisplays(); // Use new API
            }
            else
            {
                player.RefundWisdomPoints(upgradeCost);
                Debug.LogError($"Upgrade failed unexpectedly for {abilityInstance.abilityName}.");
            }
        }
        else Debug.Log($"Not enough WP to upgrade {abilityInstance.abilityName}.");
    }

    public void TrySellAbility(AbilityBase abilityInstance, ShopItemUI requestingUI)
    {
        if (abilityInstance == null || player == null || abilityManager == null) return;
        if (abilityInstance.ShopData.isOneTimePurchase) return;

        int sellValue = abilityInstance.TotalWisdomInvested;

        player.RefundWisdomPoints(sellValue);
        Debug.Log($"Sold {abilityInstance.abilityName} for {sellValue} WP.");

        abilityManager.RemoveAbility(abilityInstance); // This calls abilityInstance.Sell() internally now

        requestingUI.SetOwnedAbilityInstance(null);
        RefreshShopItemStates();
        FindFirstObjectByType<LibraryUI>()?.UpdateDisplays(); // Use new API
    }

    // IMPORTANT: Ensure this method correctly matches your AbilityBase/derived constructors
    private AbilityBase CreateAbilityInstance(ShopAbilitySO abilitySO)
    {
        // Keep the implementation from the previous response, ensuring it works for your setup.
        // Example assuming Constructor(ShopAbilitySO, GameObject):
        try
        {
            System.Type abilityType = System.Type.GetType(abilitySO.abilityClassName);
            if (abilityType == null || !typeof(AbilityBase).IsAssignableFrom(abilityType))
            {
                Debug.LogError($"Invalid class name or type for {abilitySO.abilityClassName}");
                return null;
            }

            ConstructorInfo constructor = abilityType.GetConstructor(new System.Type[] { typeof(ShopAbilitySO), typeof(GameObject) });

            if (constructor != null && abilitySO.abilityPrefab != null)
            {
                object instance = constructor.Invoke(new object[] { abilitySO, abilitySO.abilityPrefab });
                return instance as AbilityBase;
            }
            else
            {
                Debug.LogError($"Could not find matching constructor(ShopAbilitySO, GameObject) for {abilitySO.abilityClassName} or prefab is null.");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception creating ability instance for {abilitySO.abilityName}: {e.Message}");
            return null;
        }
    }
}