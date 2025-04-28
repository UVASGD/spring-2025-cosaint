using UnityEngine;

// Assumes ProtopyeSlowProjectile exists and has Initialize(float time, float magnitude) or setters

public class PrototypeSlowAbility : AbilityBase
{
    private GameObject projectilePrefab;

    public PrototypeSlowAbility(ShopAbilitySO shopData, GameObject prefab)
        : base(shopData, KeyCode.U, 2f, AbilityFireType.TAP) // Pass SO, Key, Cooldown, Type
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

        ProtopyeSlowProjectile projectileComponent = projectile.GetComponent<ProtopyeSlowProjectile>();
        if (projectileComponent != null)
        {
            float baseSlowTime = 3.5f;
            float timeMultiplierPerLevel = 1.15f;
            float baseSlowMagnitude = 0.6f;
            float magnitudeMultiplierPerLevel = 0.9f;

            float currentSlowTime = baseSlowTime * Mathf.Pow(timeMultiplierPerLevel, CurrentLevel - 1);
            float currentSlowMagnitude = baseSlowMagnitude * Mathf.Pow(magnitudeMultiplierPerLevel, CurrentLevel - 1);
            currentSlowMagnitude = Mathf.Max(currentSlowMagnitude, 0.2f); // Prevent excessive slow

            projectileComponent.SlowTime = currentSlowTime;
            projectileComponent.SlowMagnitude = currentSlowMagnitude;
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