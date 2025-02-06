using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Townhall : MonoBehaviour
{
    private const float STARTING_HEALTH = 100f;
    private float health;
    private RoundManager RoundManager;
    private HashSet<Enemy> enemiesDamaging = new HashSet<Enemy>();
    private float damageTimer = 0f;
    private float currentDamage = 0f;

    private void Start()
    {
        health = STARTING_HEALTH;
        RoundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();
    }

    private void Update() 
    {
        Debug.Log("Enemies in range: " + enemiesDamaging.Count);
        if (enemiesDamaging.Count > 0)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= 0.1f)
            {
                TakeDamage(currentDamage);
                damageTimer = 0f;
            }
        }
    }

    public void RecalculateCurrentDamage()
    {
        float damageValue = 0f;

        foreach (Enemy enemy in enemiesDamaging)
        {
            // Frozen/non damaging enemies will not damage townhall
            if (enemy.GetDoingDamage())
            {
                damageValue += enemy.GetDamage();
            }
        }

        currentDamage = damageValue;
        Debug.Log("Current Enemies Attacking Townhall: " + enemiesDamaging.Count +  " at " + currentDamage + " DPS");

    }
 
    public float GetHealth() => health;

    public void SetHealth(float newHealth)
    {
        if (newHealth < 0)
        {
            health = 0;
        }
        else
        {
            health = newHealth;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.SetDoingDamage(true);
            AddToEnemiesList(enemy);
        }
    }
    
    // Backup incase enemies glitch out map (happened during testing somehow)
    void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.SetDoingDamage(false);
            RemoveFromEnemiesList(enemy);
        }
    }

    /* 
        Recalculating only when an enemy is added rather than
        TriggerEnter allows freeze to stop enemies from damaging
    */
    public void AddToEnemiesList(Enemy enemy)
    {
        Debug.Log("Adding enemy: " + enemy);
        enemiesDamaging.Add(enemy);
        RecalculateCurrentDamage();
    }

    public void RemoveFromEnemiesList(Enemy enemy)
    {
        Debug.Log("Removing enemy: " + enemy);
        enemiesDamaging.Remove(enemy);
        RecalculateCurrentDamage();
    }

    public void TakeDamage(float amount)
    {
        health -= amount * Time.deltaTime;
        CheckTownhallDeath();
    }
  
    public void CheckTownhallDeath()
    {
        if (health <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        //Insert way to get rid of central thing, Perhaps add a particle effect/more elaborate than just destroy()
        RoundManager.SetRoundPhase(RoundManager.RoundPhase.GameOver);
        SceneManager.LoadScene("Game Over");
    }
}
