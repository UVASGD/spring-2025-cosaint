using UnityEngine;

// Assuming these enums exist somewhere
public enum AbilityFireType { TAP, HOLD }
public enum AbilityHoldAction { HOLD_START, HOLD_CONTINUE, HOLD_END }

// Abstract base class for all abilities
public abstract class AbilityBase
{
    // --- Core Ability Properties ---
    public string abilityName { get; protected set; }
    public KeyCode activationKey { get; protected set; }
    public float cooldown { get; protected set; }
    public AbilityFireType abilityFireType { get; protected set; }
    protected float lastActivationTime = -Mathf.Infinity; // Time since last use
    protected LineRenderer lineRenderer; // For hold abilities, potentially

    // --- Shop & Progression Properties ---
    public ShopAbilitySO ShopData { get; private set; } // Reference back to the SO
    public int CurrentLevel { get; protected set; } = 0; // 0 means not owned/learned yet
    public int TotalWisdomInvested { get; protected set; } = 0; // Tracks total cost spent
    protected ShopAbilitySO shopData;
    public Sprite AbilityIcon { get; protected set; } = null; // Icon for the ability


    // --- Constructor ---
    // We now also pass the ShopAbilitySO reference
    protected AbilityBase(ShopAbilitySO shopData, KeyCode key, float cool, AbilityFireType fireType)
    {
        this.ShopData = shopData; // Store the reference
        this.abilityName = shopData.abilityName; // Get name from SO
        this.activationKey = key;
        this.cooldown = cool;
        this.abilityFireType = fireType;
        this.CurrentLevel = 0; // Start at level 0 (not owned)
        this.TotalWisdomInvested = 0;
        this.AbilityIcon = shopData.icon; // Get icon from SO
    }

    // --- Core Activation Logic ---
    public virtual bool CanActivate()
    {
        return Time.time >= lastActivationTime + cooldown && CurrentLevel > 0; // Must be owned
    }

    public virtual void Activate()
    {
        if (CanActivate())
        {
            lastActivationTime = Time.time;
            Execute();
        }
        else
        {
            Debug.Log($"{abilityName} is on cooldown or not learned!");
        }
    }

    // Method to be implemented by specific abilities for TAP activation
    protected abstract void Execute();

    // Method to be implemented by specific abilities for HOLD activation
    protected abstract void HoldExecute(float holdTime, Vector3 targetPos);

    // Method to handle HOLD input state changes (called by AbilityManager)
    public virtual void ToggleHold(AbilityHoldAction action)
    {
        // Basic implementation idea - needs more logic for tracking hold time etc.
        if (abilityFireType != AbilityFireType.HOLD || !CanActivate()) return;

        if (action == AbilityHoldAction.HOLD_START)
        {
            Debug.Log($"Started Holding {abilityName}");
            // Start visualizing, maybe charge up?
        }
        else if (action == AbilityHoldAction.HOLD_END)
        {
             if (CanActivate()) // Check cooldown again on release
             {
                lastActivationTime = Time.time;
                Debug.Log($"Released {abilityName} - Executing");
                // Execute the hold ability, potentially based on charge time
                // HoldExecute(calculatedHoldTime, calculatedTargetPosition);
             }
        }
    }

    // --- Shop & Upgrade Logic ---

    // Call this when the ability is first purchased
    public virtual void Purchase(int cost)
    {
        if(CurrentLevel == 0)
        {
            CurrentLevel = 1;
            TotalWisdomInvested = cost;
            Debug.Log($"{abilityName} purchased for {cost} Wisdom. Current Level: {CurrentLevel}");
            // Apply any base stats for level 1 if needed
            ApplyLevelBasedStats();
        }
    }

    // Call this to attempt an upgrade
    public virtual bool TryUpgrade(int cost)
    {
        if (ShopData == null)
        {
            Debug.LogError($"{abilityName} is missing ShopData reference!");
            return false;
        }

        if (CurrentLevel > 0 && CurrentLevel < ShopData.maxLevel)
        {
            CurrentLevel++;
            TotalWisdomInvested += cost;
            Debug.Log($"{abilityName} upgraded to Level {CurrentLevel} for {cost} Wisdom. Total Invested: {TotalWisdomInvested}");
            // Apply the stat changes for the new level
            ApplyLevelBasedStats();
            return true;
        }
        else if (CurrentLevel >= ShopData.maxLevel)
        {
             Debug.Log($"{abilityName} is already at max level ({CurrentLevel}).");
             return false;
        }
         else // CurrentLevel is 0
        {
            Debug.LogError($"Cannot upgrade {abilityName} before purchasing it.");
            return false;
        }
    }

    // Method to be overridden by derived classes to apply stats based on CurrentLevel
    // This is where you'd modify projectile damage, duration, cooldown, etc.
    protected virtual void ApplyLevelBasedStats()
    {
        Debug.Log($"Applying stats for {abilityName} Level {CurrentLevel}. Base implementation does nothing.");
        // Example: Maybe reduce cooldown slightly per level
        // float baseCooldown = 2.0f; // Store base cooldown elsewhere if needed
        // cooldown = baseCooldown - (0.1f * (CurrentLevel - 1));
    }

    // Called when the ability is sold
    public virtual void Sell()
    {
        // Resetting level but keeping track of investment is handled by AbilityManager refunding.
        // We just reset the level here. AbilityManager removes it from the list.
        Debug.Log($"Sold {abilityName}. Level reset.");
        CurrentLevel = 0;
        // TotalWisdomInvested is used by AbilityManager *before* calling Sell, then the object is removed.
    }


    // --- Utility ---
    public virtual void SetLineRenderer(LineRenderer lr)
    {
        lineRenderer = lr;
    }

    // Helper to calculate the cost required to reach the *next* level
    public int GetNextUpgradeCost()
    {
        if (ShopData == null || CurrentLevel <= 0 || CurrentLevel >= ShopData.maxLevel)
        {
            return -1; // Indicates not upgradeable or not owned
        }
        // Cost to reach the *next* level (CurrentLevel + 1)
        return ShopData.GetCostForLevel(CurrentLevel + 1);
    }
        public float LastActivationTime { // Use PascalCase for public properties
        get { return lastActivationTime; }
    }
}