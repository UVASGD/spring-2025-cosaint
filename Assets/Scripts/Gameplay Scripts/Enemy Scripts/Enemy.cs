using System;
using NUnit.Framework.Internal;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float health = 100f;
    private const float DEFAULT_SPEED = 5f;
    private const float BASE_DAMAGE = 10f;
    private float speed = DEFAULT_SPEED;
    private float damage;
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
        SetRoundDamage();
    }

    private void Update()
    {
        if (isFrozen)
        {
            HandleFreeze();
        }
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

    private void SetRoundDamage()
    {
        int currentRound = RoundManager.Instance.GetCurrentRound();
        float damageMultiplier;

        if (currentRound >= 20)
        {
            damageMultiplier = (float) (Math.Log10(currentRound - 19) + 4);
        }
        else if (currentRound >= 10)
        {
            damageMultiplier = (float)(Math.Log10(currentRound - 9) + 2.5);
        }
        else
        {
            damageMultiplier = (float) (Math.Log10(currentRound + 1) + .7);
        }
        
        damage = BASE_DAMAGE * damageMultiplier;
        Debug.Log($"Doing {damage} damage");
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
            isDoingDamage = false;
            freezeTimer = freezeTime;
            speed = 0f;
            townHallScript.RemoveFromEnemiesList(this);
        }
    }

    private void HandleFreeze()
    {
        freezeTimer -= Time.deltaTime;
        
        if (freezeTimer <= 0)
        {
            isFrozen = false;
            isDoingDamage = true;
            ResetSpeed();
            townHallScript.AddToEnemiesList(this);
        }
    }

    public void SetDoingDamage(bool doingDamage)
    {
        isDoingDamage = doingDamage;
    }

    public bool GetDoingDamage() => isDoingDamage;

    private void ResetSpeed()
    {
        speed = DEFAULT_SPEED;
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