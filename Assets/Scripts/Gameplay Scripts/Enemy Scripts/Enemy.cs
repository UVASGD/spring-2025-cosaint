using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float speed = 2f;

    private float damage = 5.00f;
    private Transform target;

    private bool isFrozen = false;
    private float freezeTimer = 0f;

    private bool isDoingDamage = false;

    private GameObject townHall;
    private Townhall townHallScript;

    private void Start() 
    {
        townHall = GameObject.Find("Townhall");
        townHallScript = townHall.GetComponent<Townhall>();
        target = townHall.transform;
    }

    private void Update()
    {
        if (isFrozen) HandleFreeze();
        
        Move();
    }

    // Basic movement logic shared by all enemies
    protected virtual void Move()
    {
        if (target == null)
        {
            Debug.LogWarning("Target not set for " + name);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    // Generic method to take damage
    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    public void ApplyFreeze(float freezeTime)
    {
        if (!isFrozen)
        {
            isFrozen = true;
            freezeTimer = freezeTime;
            speed = 0f; 
        }
    }

    private void HandleFreeze()
    {
        freezeTimer -= Time.deltaTime;
        
        if (freezeTimer <= 0)
        {
            isFrozen = false;
            ResetSpeed();
        }
    }

    public void SetDoingDamage()
    {
        this.isDoingDamage = true;
    }

    private void ResetSpeed()
    {
        speed = 2f;
    }

    // Death logic
    protected virtual void Die()
    {
        // Ensures out of townhall range do not effect enemiesInRange set
        if (isDoingDamage)
        {
            townHallScript.RemoveFromEnemiesList(this);
        }
        Debug.Log($"{name} has died!");
        Destroy(gameObject);
    }

    // Placeholder for abilities (to be overridden by child or component scripts)
    public virtual void UseAbility()
    {
        Debug.Log($"{name} has no special ability!");
    }

    public float GetDamage() => damage;
}