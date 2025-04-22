using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting; // Needed for LINQ methods like FirstOrDefault

public class AbilityManager : MonoBehaviour
{
    // This list now holds ALL OWNED abilities and their current state (level)
    [SerializeField] private List<AbilityBase> ownedAbilities = new List<AbilityBase>();
    [SerializeField] private LineRenderer lineRenderer; // Keep for hold abilities if needed

    void Awake()
    {
        // Keep LineRenderer setup if you use it
        if (lineRenderer != null)
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.useWorldSpace = true;
        }
        else
        {
            Debug.LogWarning("LineRenderer not assigned in AbilityManager.");
        }

    }

    // Input handling remains largely the same, but iterates over OWNED abilities
    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Check input for all owned abilities
        foreach (var ability in ownedAbilities)
        {
            // Only process input if the ability is actually learned (Level > 0)
            // This check might be redundant if ownedAbilities only contains level > 0 items,
            // but it's safe to keep. AbilityBase.CanActivate() also checks level.
            if (ability.CurrentLevel <= 0) continue;

            if (ability.abilityFireType == AbilityFireType.TAP)
            {
                if (Input.GetKeyDown(ability.activationKey))
                {
                    ability.Activate();
                }
            }
            else if (ability.abilityFireType == AbilityFireType.HOLD)
            {
                // Note: Basic hold logic, might need refinement
                if (Input.GetKeyDown(ability.activationKey)) // Key Down starts the hold attempt
                {
                    ability.ToggleHold(AbilityHoldAction.HOLD_START);
                }
                else if (Input.GetKey(ability.activationKey)) // Key Held continues (optional action)
                {
                    // ability.ToggleHold(AbilityHoldAction.HOLD_CONTINUE); // If needed
                }
                else if (Input.GetKeyUp(ability.activationKey)) // Key Up ends the hold
                {
                    ability.ToggleHold(AbilityHoldAction.HOLD_END);
                }
            }
        }
    }

    // --- Modified Methods for Shop Interaction ---

    // Find an owned ability by its name
    public AbilityBase GetAbilityByName(string name)
    {
        // Use LINQ FirstOrDefault to find the ability or return null
        return ownedAbilities.FirstOrDefault(ability => ability.abilityName == name);
    }

    // Find an owned ability using the ScriptableObject reference
    public AbilityBase GetAbilityByShopData(ShopAbilitySO shopData)
    {
        if (shopData == null) return null;
        return ownedAbilities.FirstOrDefault(ability => ability.ShopData == shopData);
    }


    // Add a NEWLY PURCHASED ability instance
    public void AddAbility(AbilityBase ability)
    {
        // Double-check it's not already added (ShopManager should prevent this)
        if (GetAbilityByShopData(ability.ShopData) == null) // Check using SO reference
        {
            ownedAbilities.Add(ability);
            // Pass LineRenderer if needed by the ability
            if (ability.abilityFireType == AbilityFireType.HOLD && lineRenderer != null)
            {
                ability.SetLineRenderer(lineRenderer);
            }

            Debug.Log($"Ability '{ability.abilityName}' added to AbilityManager (Level {ability.CurrentLevel}).");
        }
        else
        {
            Debug.LogWarning($"Ability {ability.abilityName} is already managed. Purchase logic might be flawed.");
        }
    }

    // Remove an ability when SOLD
    public void RemoveAbility(AbilityBase ability)
    {
        if (ownedAbilities.Contains(ability))
        {
            // Call the ability's Sell method *before* removing, if it needs cleanup
            ability.Sell();

            ownedAbilities.Remove(ability);
            Debug.Log($"Ability '{ability.abilityName}' removed from AbilityManager (Sold).");
        }
        else
        {
            Debug.LogWarning($"Ability {ability.abilityName} not found in the owned list for removal.");
        }
    }

    // Get a copy of the list of owned abilities
    public List<AbilityBase> GetOwnedAbilities()
    {
        return new List<AbilityBase>(ownedAbilities); // Return a copy
    }
}