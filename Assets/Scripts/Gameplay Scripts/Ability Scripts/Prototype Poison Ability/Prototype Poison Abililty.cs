using UnityEngine;

// Assumes ProtopyePoisonProjectile exists and has public properties or setters
// like 'PoisonTime' and 'PoisonWeakness' or an Initialize method.

public class PrototypePoisonAbility : AbilityBase
{
    private GameObject projectilePrefab;

    public PrototypePoisonAbility(ShopAbilitySO shopData, GameObject prefab)
        : base(shopData, KeyCode.T, 2f, AbilityFireType.TAP)
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

        Transform cameraTransform = Camera.main.transform;
        GameObject projectile = GameObject.Instantiate(
            projectilePrefab,
            cameraTransform.position + cameraTransform.forward,
            Quaternion.LookRotation(cameraTransform.forward)
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.useGravity = false;
            rb.linearVelocity = cameraTransform.forward * 10f;
        }

        ProtopyePoisonProjectile projectileComponent = projectile.GetComponent<ProtopyePoisonProjectile>();
        if (projectileComponent != null)
        {
            // Define base values for Level 1
            float basePoisonTime = 5.0f;
            float basePoisonWeakness = 1.0f; // Assume 1.0 means normal damage taken

            // Define how much stats change per level upgrade
            float timeMultiplierPerLevel = 1.1f; // 10% increase per level
            float weaknessMultiplierPerLevel = 0.9f; // 10% decrease per level (stronger effect)

            // Calculate the actual values for the CurrentLevel
            float currentPoisonTime = basePoisonTime * Mathf.Pow(timeMultiplierPerLevel, CurrentLevel - 1);
            float currentPoisonWeakness = basePoisonWeakness * Mathf.Pow(weaknessMultiplierPerLevel, CurrentLevel - 1);

            // Add a safety floor for weakness
            currentPoisonWeakness = Mathf.Max(currentPoisonWeakness, 0.1f);

            projectileComponent.PoisonTime = currentPoisonTime;
            projectileComponent.PoisonWeakness = currentPoisonWeakness;
        }
    }

    protected override void ApplyLevelBasedStats()
    {

    }

    protected override void HoldExecute(float holdTime, Vector3 targetPos)
    {
        throw new System.NotImplementedException();
    }
}