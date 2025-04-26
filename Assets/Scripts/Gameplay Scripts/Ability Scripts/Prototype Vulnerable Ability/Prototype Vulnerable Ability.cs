using UnityEngine;

// Assumes ProtopyeVulnerableProjectile exists and has SetDuration(float) or Initialize(float)

public class PrototypeVulnerableAbility : AbilityBase
{
    private GameObject projectilePrefab;

    // Constructor
    public PrototypeVulnerableAbility(ShopAbilitySO shopData, GameObject prefab)
        : base(shopData, KeyCode.H, 2f, AbilityFireType.TAP) // Pass SO, Key, Cooldown, Type
    {
        if (prefab == null) { Debug.LogError($"Prefab is null for ability {shopData?.abilityName ?? "Ability"}!"); }
        this.projectilePrefab = prefab;
    }

    // Called every activation
    protected override void Execute()
    {
                PlayCastSound();
        if (projectilePrefab == null) {
            Debug.LogError($"{abilityName}: projectilePrefab is null!");
            return;
        }

         Debug.Log($"{abilityName} activated! Level: {CurrentLevel}"); 

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

        // Apply level-scaled duration to the instance
        ProtopyeVulnerableProjectile projectileComponent = projectile.GetComponent<ProtopyeVulnerableProjectile>();
        if (projectileComponent != null)
        {
            float baseDuration = 3.0f; // Level 1 duration
            float durationIncreasePerLevel = 0.5f; // Bonus per level > 1
            float currentDuration = baseDuration + (CurrentLevel - 1) * durationIncreasePerLevel;


            projectileComponent.Duration = currentDuration;
        }
    }

    // Called on purchase / level up
    protected override void ApplyLevelBasedStats()
    {
        // Scaling for projectile duration is handled in Execute.
        // Only modify ability-instance stats here if needed (e.g., cooldown).
    }

    // Required by base class for TAP ability
    protected override void HoldExecute(float holdTime, Vector3 targetPos)
    {
        throw new System.NotImplementedException();
    }
}