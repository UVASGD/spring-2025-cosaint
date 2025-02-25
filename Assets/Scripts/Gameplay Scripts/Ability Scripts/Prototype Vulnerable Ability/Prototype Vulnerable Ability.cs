using UnityEngine;

public class PrototypeVulnerableAbility : AbilityBase
{
    private GameObject projectilePrefab; // Reference to the prefab for the projectile
    private Transform orientationTransform;  // Reference to the player's orientation

    // Constructor
    public PrototypeVulnerableAbility(GameObject prefab, Transform player) 
        : base("Prototype Vulnerable Ability", KeyCode.H, 2f)
    {
        projectilePrefab = prefab;
        orientationTransform = player;
    }

    protected override void Execute()
    {
        Debug.Log($"{abilityName} activated!");


        // Instantiate the projectile
        GameObject projectile = GameObject.Instantiate(
            projectilePrefab, 
            orientationTransform.position + orientationTransform.forward, // Spawn in front of player
            Quaternion.LookRotation(orientationTransform.forward)    // Orient forward
        );

        // Add velocity to the projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = orientationTransform.forward * 10f;
        
    }

    public override void UpgradeAbility()
    {

    }
}