using UnityEngine;

public class Player : MonoBehaviour
{
    public AbilityManager abilityManager { get; private set; }

    [SerializeField] private int wisdomPoints = 50;
    [SerializeField] private const int MAX_SLOTS = 10;


    void Awake()
    {
        if (abilityManager == null)
        {
            abilityManager = FindFirstObjectByType<AbilityManager>();
            if (abilityManager == null)
            {
                Debug.LogError("Player could not find AbilityManager!");
            }
        }
    }

    public void AwardWisdomPoints(int points)
    {
        if (points > 0)
        {
            wisdomPoints += points;
            Debug.Log($"Awarded {points} WP. Total: {wisdomPoints}");
            FindFirstObjectByType<ShopManager>()?.RefreshShopItemStates();
            FindFirstObjectByType<LibraryUI>()?.UpdateDisplays();
        }
    }

    public bool CanAfford(int cost)
    {
        return wisdomPoints >= cost;
    }

    public bool SpendWisdomPoints(int amount)
    {
        if (amount <= 0) return false;

        if (wisdomPoints >= amount)
        {
            wisdomPoints -= amount;
            Debug.Log($"Spent {amount} WP. Remaining: {wisdomPoints}");

            FindFirstObjectByType<ShopManager>()?.RefreshShopItemStates();
            FindFirstObjectByType<LibraryUI>()?.UpdateDisplays();
            return true;
        }
        else
        {
            Debug.Log($"Cannot spend {amount} WP. Only have {wisdomPoints}");
            return false;
        }
    }

    public void RefundWisdomPoints(int amount)
    {
        if (amount > 0)
        {
            wisdomPoints += amount;
            Debug.Log($"Refunded {amount} WP. Total: {wisdomPoints}");
            FindFirstObjectByType<ShopManager>()?.RefreshShopItemStates();
            FindFirstObjectByType<LibraryUI>()?.UpdateDisplays();
        }
    }

    public int GetWisdomPoints() => wisdomPoints;

    public bool FreeSlots()
    {
        return abilityManager.OwnedAbilityCount < GetMaxSlots();
    }

    public int GetSlotsUsed()
    {
        return abilityManager.OwnedAbilityCount;
    }

    public int GetMaxSlots()
    {
        return MAX_SLOTS;
    }
}
