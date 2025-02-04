using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class Townhall : MonoBehaviour
{
    private const float STARTING_HEALTH = 100f;
    private float health;

    private RoundManager roundManager;

    private HashSet<Enemy> enemiesInRange = new HashSet<Enemy>();
    
    public string enemyTag = "Enemy";
    private float damageTimer = 0f;

    private float currentDamage = 0f;

    private void Start()
    {
        health = STARTING_HEALTH;
        roundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();
    }


    private void Update() 
    {
        Debug.Log("Enemies in range: " + enemiesInRange.Count);

         if (enemiesInRange.Count > 0)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= 0.5f)
            {
                TakeDamage(currentDamage);
                damageTimer = 0f;
            }
        }

        CheckTownhallDeath();
    }

    public void RecalculateCurrentDamage()
    {
        float damageValue = 0f;

        foreach (Enemy enemy in enemiesInRange)
        {
           damageValue += enemy.GetDamage();
        }

        currentDamage = damageValue;
    }
 
    public float getHealth()
    {
        return health;
    }

    public void setHealth(float newHealth)
    {
        this.health = newHealth;
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.SetDoingDamage();
            enemiesInRange.Add(enemy);
            RecalculateCurrentDamage();
            Debug.Log("Current Enemies Attacking Townhall:" + enemiesInRange.Count + "at " + currentDamage + " DPS");
        }
    }

    public void RemoveFromEnemiesList(Enemy enemy)
    {
        Debug.Log("Removing enemy: " + enemy);
        enemiesInRange.Remove(enemy);
    }

    /*
        No need to know the trigger exit since the enemy never willingly leaves it
        When the enemy is killed they'll tell the townhall to remove it from enemiesInRange
    */
    // void OnTriggerExit(Collider other)
    // {
    //     Enemy enemy = other.GetComponent<Enemy>();
    //     Debug.Log("Removing enemy: " + enemy);

    //     if (enemy != null)
    //     {
    //         Debug.Log("Removing enemy: " + enemy);
    //         enemiesInRange.Remove(enemy);
    //         RecalculateCurrentDamage();
    //         Debug.Log("Current Enemies Attacking Townhall:" + enemiesInRange.Count + "at " + currentDamage + " DPS");
    //     }
    // }

    public void TakeDamage(float amount)
    {
        health -= amount * Time.deltaTime;
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
        roundManager.setRoundPhase(RoundManager.RoundPhase.GameOver);
        SceneManager.LoadScene("Game Over");
    }
}
