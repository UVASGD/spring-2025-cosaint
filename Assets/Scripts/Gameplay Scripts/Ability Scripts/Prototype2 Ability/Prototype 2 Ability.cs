using UnityEngine;

public class Prototype2Ability : AbilityBase
{
    private GameObject projectilePrefab; // Reference to the prefab for the projectile
    private Transform orientationTransform;  // Reference to the player's orientation

    public Prototype2Ability(ShopAbilitySO shopData, GameObject prefab) // Added shopData param
        : base(shopData, KeyCode.F, 2f, AbilityFireType.TAP) // Pass shopData to base, provide specific Key/Cool/Type
    {
        projectilePrefab = prefab;
    }

    protected override void Execute()
    {
        PlayCastSound();
        Debug.Log($"{abilityName} activated!");


        Transform cameraTransform = Camera.main.transform;
        // Instantiate the projectile
        GameObject projectile = GameObject.Instantiate(
            projectilePrefab,
            cameraTransform.position + cameraTransform.forward, // Spawn in front of player
            Quaternion.LookRotation(cameraTransform.forward)    // Orient forward
        );

        // Add velocity to the projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = cameraTransform.forward * 10f;

        Protopye2Projectile projectileComponent = projectile.GetComponent<Protopye2Projectile>();
        if (projectileComponent != null)
        {
            float baseFreezeTime = 5f;
            float currentFreezeTime = baseFreezeTime * (1 + (CurrentLevel - 1) * 0.1f);

            // Use the setter method if it exists on Protopye2Projectile
            projectileComponent.setFreezeTime(currentFreezeTime); // Assumes this method exists
        }
        else
        {
            Debug.LogWarning($"{abilityName}: Instantiated projectile '{projectile.name}' missing Protopye2Projectile script!");
        }

    }

    protected override void ApplyLevelBasedStats()
    {
        Debug.Log($"{abilityName} ApplyLevelBasedStats called for Level {CurrentLevel}.");
    }
    protected override void HoldExecute(float holdTime, Vector3 targetPos)
    {
        throw new System.NotImplementedException();
    }
}