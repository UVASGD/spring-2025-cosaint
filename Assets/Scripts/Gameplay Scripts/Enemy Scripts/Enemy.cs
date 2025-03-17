using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private float maxHealth = 100f;
    private float health = 100f;
    private const float DEFAULT_SPEED = 3f;
    private const float BASE_DAMAGE = 10f;
    private float speed = DEFAULT_SPEED;
    private float damage;
    private Transform target;

    // Status Effects ---------------------
    private bool isFrozen = false;

    private bool isBurned = false;

    private bool isVulnerable = false;
    private bool isDoingDamage = false;
    private bool isBeguiled = false;
    private bool isSlowed = false;
    private bool isPoisoned = false;
    private bool isDefenseDown = false;
    // -------------------------------------

    private float vulnerableHealth = 0f;
    private float beguileDamage = 10f;

    private const float HEALTH_WEAKNESS = 0.8f;

    private GameObject townHall;
    private Lighthouse lighthouse;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image sliderBar;
    [SerializeField] private Transform healthBarTransform;
    [SerializeField] private TextMeshProUGUI healthText; //only needed for text over health bar for debugging

    private EnemySpawnManager enemySpawnManager;

    private void Start()
    {
        townHall = GameObject.Find("Lighthouse");
        enemySpawnManager = GameObject.Find("Enemy Spawn Manager").GetComponent<EnemySpawnManager>();
        lighthouse = townHall.GetComponent<Lighthouse>();
        target = lighthouse.transform;
        SetRoundDamage();
        //healthText.text = health.ToString("#.0") + " / " + maxHealth.ToString("#.0");
    }

    private void Update()
    {
        // If the enemy is beguiled, BeguileTimer() will run MoveTo()
        if (!isBeguiled)
        {
            Move();
        }
    }

    private void LateUpdate()
    {
        healthBarTransform.LookAt(Camera.main.transform);
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
    protected virtual void MoveToEnemy(Enemy enemy)
    {
        // If the enemy its chasing is killed it'll return back to normal
        if (enemy == null)
        {
            isBeguiled = false;
            return;
        }

        Vector3 directionToEnemy = enemy.transform.position - transform.position;
        transform.position += directionToEnemy.normalized * speed * Time.deltaTime;
    }

    private void SetRoundDamage()
    {
        int currentRound = RoundManager.Instance.GetCurrentRound();
        float damageMultiplier;

        if (currentRound >= 20)
        {
            damageMultiplier = (float)(Math.Log10(currentRound - 19) + 4);
        }
        else if (currentRound >= 10)
        {
            damageMultiplier = (float)(Math.Log10(currentRound - 9) + 2.5);
        }
        else
        {
            damageMultiplier = (float)(Math.Log10(currentRound + 1) + .7);
        }

        damage = BASE_DAMAGE * damageMultiplier;
        Debug.Log($"Doing {damage} damage");
    }

    // Generic method to take damage
    public virtual void TakeDamage(float amount)
    {
        health -= amount;

        if (isVulnerable)
        {
            vulnerableHealth -= amount;
            if (vulnerableHealth <= 0)
            {
                Die();
            }
            healthBar.value = vulnerableHealth / maxHealth;
            //healthText.text = vulnerableHealth.ToString("#.0") + " / " + maxHealth.ToString("#.0");
        }
        else
        {
            if (health <= 0)
            {
                Die();
            }
            healthBar.value = health / maxHealth;
            //healthText.text = health.ToString("#.0") + " / " + maxHealth.ToString("#.0");
        }
    }

    public void ApplyBeguile(float beguileTime, float beguileDamage)
    {
        if (isBeguiled)
        {
            return;
        }

        // If no enemies to chase
        if (enemySpawnManager.GetAliveEnemiesCount() <= 1)
        {
            return;
        }

        this.beguileDamage = beguileDamage;
        StartCoroutine(HandleBeguile(beguileTime));
    }

    private IEnumerator HandleBeguile(float beguileTime)
    {
        
        isBeguiled = true;
        Enemy closestEnemy = FindClosestEnemy();

        float beguileTimer = 0f;
        while (isBeguiled)
        {
            if (closestEnemy == null)
            {
                if (enemySpawnManager.GetAliveEnemiesCount() > 1)
                {
                    closestEnemy = FindClosestEnemy();
                }
                else
                {
                    isBeguiled = false;
                    yield break;
                }
            }

            MoveToEnemy(closestEnemy);
            beguileTimer += Time.deltaTime;

            if (beguileTimer > beguileTime)
            {
                isBeguiled = false;
            }
            yield return null;
        }
    }

    float beguileTimer = 0f;
    void OnCollisionStay(Collision other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (isBeguiled && enemy == FindClosestEnemy())
        {
            beguileTimer += Time.deltaTime;
            if (beguileTimer > 1f)
            {
                enemy.TakeDamage(beguileDamage);
                beguileTimer = 0f;
            }
        }
    }

    public Enemy FindClosestEnemy()
    {
        Enemy closestEnemy = enemySpawnManager.GetEnemies()
            .Where(enemy => enemy != this)
            .OrderBy(enemy => (enemy.transform.position - this.transform.position).sqrMagnitude)
            .FirstOrDefault();

        return closestEnemy;
    }

    public void ApplyFreeze(float freezeTime)
    {
        if (!isFrozen)
        {
            StartCoroutine(HandleFreeze(freezeTime));
        }
    }

    private IEnumerator HandleFreeze(float freezeTime)
    {
        isFrozen = true;
        isDoingDamage = false;
        SetSpeed(0f);
        lighthouse.RemoveFromEnemiesList(this);

        yield return new WaitForSeconds(freezeTime);
        isFrozen = false;
        isDoingDamage = true;
        ResetSpeed();
        lighthouse.AddToEnemiesList(this);
    }

    private void SetSpeed(float speed)
    {
        if (speed < 0f)
        {
            speed = 0f;
        }

        this.speed = speed;
    }

    public void ApplyPoison(float poisonTime, float poisionWeakness)
    {
        if (!isPoisoned)
        {
            StartCoroutine(HandlePoison(poisonTime, poisionWeakness));
        }
    }
    private IEnumerator HandlePoison(float poisonTime, float poisonWeakness)
    {
        isPoisoned = true;
        damage *= poisonWeakness;

        yield return new WaitForSeconds(poisonTime);
        isPoisoned = false;
        damage /= poisonWeakness;
    }

    public void ApplySlow(float slowTime, float slowMangitude)
    {
        if (!isSlowed)
        {
            StartCoroutine(HandleSlow(slowTime, slowMangitude));
        }
    }

    private IEnumerator HandleSlow(float slowTime, float slowMangitude)
    {
        isSlowed = true;
        speed *= slowMangitude;

        yield return new WaitForSeconds(slowTime);
        isSlowed = false;
        ResetSpeed();
    }

    public void ApplyBurn(float tickDur, float burnDamage, int numTicks)
    {
        if (!isBurned)
        {
            StartCoroutine(HandleBurn(tickDur, burnDamage, numTicks));
        }
    }

    private IEnumerator HandleBurn(float tickDur, float burnDamage, int numTicks)
    {

        isBurned = true;
        int ticksLeft = numTicks;
        while (ticksLeft > 0)
        {
            if (health <= 0)
            {
                yield break;
            }

            TakeDamage(burnDamage);
            yield return new WaitForSeconds(tickDur);
            ticksLeft--;
        }

        isBurned = false;
    }

    public void ApplyVulnerable(float vulnerableTime, float vulnerablePercentage)
    {
        if (!isVulnerable)
        {
            StartCoroutine(HandleVulnerable(vulnerableTime, vulnerablePercentage));
        }
    }

    private IEnumerator HandleVulnerable(float vulnerableTime, float vulnerablePercentage)
    {
        isVulnerable = true;
        vulnerableHealth = health * vulnerablePercentage;
        sliderBar.color = Color.green;
        healthBar.value = vulnerableHealth / maxHealth;
        //healthText.text = vulnerableHealth.ToString("#.0") + " / " + maxHealth.ToString("#.0");

        yield return new WaitForSeconds(vulnerableTime);
        isVulnerable = false;
        sliderBar.color = Color.red;
        healthBar.value = health / maxHealth;
        //healthText.text = health.ToString("#.0") + " / " + maxHealth.ToString("#.0");
    }

    public void ApplyDefenseDown(float defenseTime)
    {
        if (!isDefenseDown)
        {
            StartCoroutine(HandleDefenseDown(defenseTime));
        }
    }

    private IEnumerator HandleDefenseDown(float defenseTime)
    {
        isDefenseDown = true;
        health *= HEALTH_WEAKNESS;

        yield return new WaitForSeconds(defenseTime);
        isDefenseDown = false;
        health /= HEALTH_WEAKNESS;
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
            lighthouse.RemoveFromEnemiesList(this);
        }

        enemySpawnManager.RemoveEnemyFromList(this);
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