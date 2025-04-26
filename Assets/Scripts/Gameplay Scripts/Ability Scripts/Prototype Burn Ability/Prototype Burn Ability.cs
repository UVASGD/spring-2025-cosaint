using UnityEngine;


public class PrototypeBurnAbility : AbilityBase
{
    private GameObject projectilePrefab;

    public PrototypeBurnAbility(ShopAbilitySO shopData, GameObject prefab)
        : base(shopData, KeyCode.G, 2f, AbilityFireType.TAP) // Pass SO, Key, Cooldown, Type
    {
        if (prefab == null) { Debug.LogError($"Prefab is null for ability {shopData?.abilityName ?? "Ability"}!"); }
        this.projectilePrefab = prefab;
    }

    protected override void Execute()
    {
                PlayCastSound();
        if (projectilePrefab == null) {
            Debug.LogError($"{abilityName}: projectilePrefab is null!");
            return;
        }

         Debug.Log($"{abilityName} activated! Level: {CurrentLevel}"); // Optional debug

        Transform cameraTransform = Camera.main.transform;
        GameObject projectile = GameObject.Instantiate(
            projectilePrefab,
            cameraTransform.position + cameraTransform.forward,
            Quaternion.LookRotation(cameraTransform.forward)
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.useGravity = false;
            rb.linearVelocity = cameraTransform.forward * 10f; // Fixed speed
        }

        // Apply level-scaled burn effect stats to the instance
        ProtopyeBurnProjectile projectileComponent = projectile.GetComponent<ProtopyeBurnProjectile>();
        if (projectileComponent != null)
        {
            // --- Define Base Values and Scaling ---
            float baseTickDuration = 1.0f;   // Level 1 time between ticks
            float durationMultiplierPerLevel = 0.9f; // Ticks get faster

            float baseBurnDamage = 5.0f;     // Level 1 damage per tick
            float damageMultiplierPerLevel = 1.1f; // Damage increases

            int baseNumTicks = 3;          // Level 1 number of ticks
            int ticksIncreasePerLevel = 1;     // Linear increase in tick count

            // --- Calculate current stats based on level ---
            float currentTickDuration = baseTickDuration * Mathf.Pow(durationMultiplierPerLevel, CurrentLevel - 1);
            currentTickDuration = Mathf.Max(currentTickDuration, 0.1f); // Prevent too-fast ticks

            float currentBurnDamage = baseBurnDamage * Mathf.Pow(damageMultiplierPerLevel, CurrentLevel - 1);

            int currentNumTicks = baseNumTicks + (CurrentLevel - 1) * ticksIncreasePerLevel;

            // --- Pass stats to the projectile instance ---
            // Assumes public properties or setters exist on projectile script
            projectileComponent.TickDuration = currentTickDuration;
            projectileComponent.BurnDamage = currentBurnDamage;
            projectileComponent.NumTicks = currentNumTicks;
            // Or: projectileComponent.Initialize(currentTickDuration, currentBurnDamage, currentNumTicks);
        }
    }

    protected override void ApplyLevelBasedStats()
    {
        // Intentionally left empty unless ability-instance stats need changes on level up.
    }

    protected override void HoldExecute(float holdTime, Vector3 targetPos)
    {
        throw new System.NotImplementedException();
    }

}