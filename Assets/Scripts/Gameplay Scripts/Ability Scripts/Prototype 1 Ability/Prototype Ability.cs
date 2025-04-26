using UnityEngine;

// Assumes ProtopyeProjectile exists and has a method like setDamage(float damage)
// or a public property 'Damage', or an Initialize(float damage) method.

public class PrototypeAbility : AbilityBase
{
    private GameObject projectilePrefab;

    public PrototypeAbility(ShopAbilitySO shopData, GameObject prefab)
        : base(shopData, KeyCode.E, 3f, AbilityFireType.TAP) // Pass SO, Key, Cooldown, Type
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

        // Apply level-scaled damage stats to the instance
        ProtopyeProjectile projectileComponent = projectile.GetComponent<ProtopyeProjectile>();
        if (projectileComponent != null)
        {
            // --- Define Base Values and Scaling ---
            float baseDamage = 10.0f;    // Level 1 damage
            float damageMultiplierPerLevel = 1.1f; // 10% damage increase per level

            // --- Calculate current stats based on level ---
            float currentDamage = baseDamage * Mathf.Pow(damageMultiplierPerLevel, CurrentLevel - 1);

            projectileComponent.setDamage(currentDamage);
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

    // --- REMOVED UpgradeAbility() METHOD ---
}