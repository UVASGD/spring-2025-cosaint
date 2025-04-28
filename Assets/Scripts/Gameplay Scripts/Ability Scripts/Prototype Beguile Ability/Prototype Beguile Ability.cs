using UnityEngine;

// Assumes ProtopyeBeguileProjectile exists and has public properties or setters
// like 'Duration' and 'Strength' or an Initialize method.

public class PrototypeBeguileAbility : AbilityBase
{
    private GameObject projectilePrefab;

    public PrototypeBeguileAbility(ShopAbilitySO shopData, GameObject prefab)
        : base(shopData, KeyCode.B, 2f, AbilityFireType.TAP) // Pass SO, Key, Cooldown, Type
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

        // Debug.Log($"{abilityName} activated! Level: {CurrentLevel}"); // Optional debug

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

        // Apply level-scaled beguile effect stats to the instance
        ProtopyeBeguileProjectile projectileComponent = projectile.GetComponent<ProtopyeBeguileProjectile>();
        if (projectileComponent != null)
        {
            // --- Define Base Values and Scaling (Example) ---
            float baseDuration = 7.0f;      // Level 1 duration
            float durationIncreasePerLevel = 1.2f; 

            float baseStrength = 0.5f;      // Level 1 effect strength (e.g., 50%)
            float strengthIncreasePerLevel = 0.1f; // +10% strength per level > 1

            // --- Calculate current stats based on level ---
            float currentDuration = baseDuration + (CurrentLevel - 1) * durationIncreasePerLevel;
            float currentStrength = baseStrength + (CurrentLevel - 1) * strengthIncreasePerLevel;
            currentStrength = Mathf.Clamp01(currentStrength); // Ensure strength stays between 0 and 1 if it's a percentage chance/factor

            // --- Pass stats to the projectile instance ---
            projectileComponent.setBeguileTime(currentDuration);
            projectileComponent.setBeguileDamge(currentStrength);
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