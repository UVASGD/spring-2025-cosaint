using UnityEngine;

public class PrototypeAbility : AbilityBase
{
    private GameObject projectilePrefab; // Reference to the prefab for the projectile
    private Transform orientationTransform;  // Reference to the player's orientation

    // Constructor
    public PrototypeAbility(GameObject prefab, Transform player) 
        : base("Prototype Ability", KeyCode.E, 3f)
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
        this.level++;
        Debug.Log("Ability upgraded to level " + this.level);
        ProtopyeProjectile projectile = projectilePrefab.GetComponent<ProtopyeProjectile>();
        projectile.setDamage(projectile.getDamage() * 1.1f);
    }
}