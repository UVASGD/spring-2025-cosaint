using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
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
    protected Lighthouse lighthouse;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image sliderBar;
    [SerializeField] private Transform healthBarTransform;
    [SerializeField] private TextMeshProUGUI healthText; //only needed for text over health bar for debugging

    private EnemySpawnManager enemySpawnManager;

    private Animator enemyAnimator;
    private bool isAttacking = false;
    private NavMeshAgent agent;


    Transform closestPoint;


    protected virtual void Start()
    {
        townHall = GameObject.Find("Lighthouse");
        enemySpawnManager = GameObject.Find("Enemy Spawn Manager").GetComponent<EnemySpawnManager>();
        lighthouse = townHall.GetComponent<Lighthouse>();
        target = lighthouse.transform;
        agent = GetComponentInParent<NavMeshAgent>();
        SetRoundDamage();


        closestPoint = GetClosestTaggedEdgePoint();
        agent.speed = speed;
        agent.acceleration = 8f; // Optional, tweak as needed
        agent.angularSpeed = 120f;
        agent.SetDestination(closestPoint.position);
        enemyAnimator = GetComponent<Animator>();
        agent.updateRotation = true;
        //healthText.text = health.ToString("#.0") + " / " + maxHealth.ToString("#.0");
    }

    private void Update()
    {
        // if (!isBeguiled)
        // {
        //     Move();
        // }


        // If agent is close enough to attack and has moved at least once
        if (
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f)
        )
        {
            isAttacking = true;
            enemyAnimator.SetBool("isMoving", false);
            enemyAnimator.SetBool("isAttacking", true);
        }
        else
        {
            isAttacking = false;
            enemyAnimator.SetBool("isMoving", true);
            enemyAnimator.SetBool("isAttacking", false);
        }
    }

    private void LateUpdate()
    {
        //healthBarTransform.LookAt(Camera.main.transform);
    }

    private Transform GetClosestTaggedEdgePoint()
    {
        GameObject[] edgePoints = GameObject.FindGameObjectsWithTag("EnemyAIPoint");

        Debug.Log("Found " + edgePoints.Length + " tagged objects");

        if (edgePoints.Length == 0)
        {
            Debug.LogWarning("No LighthouseEdgePoint tagged objects found!");
        }

        Transform closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject obj in edgePoints)
        {
            float distance = Vector3.Distance(currentPosition, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = obj.transform;
            }
        }

        return closest;
    }

    // Basic movement logic shared by all enemies
    protected virtual void Move()
    {
        if (target == null || isFrozen || isBeguiled)
        {
            return;
        }

        // Vector3 direction = (target.position - transform.position).normalized;
        // transform.position += direction * speed * Time.deltaTime;
        //agent.destination = target.position;
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

        Debug.Log("Beguiled");
        this.beguileDamage = beguileDamage;
        StartCoroutine(HandleBeguile(beguileTime));
    }

    private IEnumerator HandleBeguile(float beguileTime)
    {
        isBeguiled = true;
        Enemy closestEnemy = FindClosestEnemy();
        if (closestEnemy == null)
        {
            Debug.LogWarning("No closest enemy found at start of beguile.");
            isBeguiled = false;
            yield break;
        }

        float beguileTimer = 0f;
        float attackTimer = 0f;
        isDoingDamage = false;

        while (isBeguiled)
        {
            if (closestEnemy != null && enemySpawnManager.GetAliveEnemiesCount() > 1)
            {
                agent.SetDestination(closestEnemy.transform.position);

                if (Vector3.Distance(gameObject.transform.position, closestEnemy.gameObject.transform.position) <= 2f)
                {
                    enemyAnimator.SetBool("isAttacking", true);
                    attackTimer += Time.deltaTime;
                    if (attackTimer > 1f)
                    {
                        closestEnemy.TakeDamage(beguileDamage);
                        attackTimer = 0f;
                    }
                }
                else
                {
                    enemyAnimator.SetBool("isAttacking", false);
                }
            }
            else
            {
                Debug.Log("Enemy count <= 1 or enemy is null ");
                isBeguiled = false;
                isDoingDamage = true;
                yield break;
            }

            beguileTimer += Time.deltaTime;
            if (beguileTimer > beguileTime)
            {
                isBeguiled = false;
                enemyAnimator.SetBool("isAttacking", false);
                agent.SetDestination(closestPoint.position);
                isDoingDamage = true;
                Debug.Log("Reverted destination");
                yield break;
            }

            yield return null;
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

        StartCoroutine(HandleFreeze(freezeTime));

    }

    private IEnumerator HandleFreeze(float freezeTime)
    {
        isFrozen = true;
        isDoingDamage = false;
        agent.speed = 0f;
        lighthouse.RemoveFromEnemiesList(this);
        enemyAnimator.speed = 0f;

        yield return new WaitForSeconds(freezeTime);

        Debug.Log($"{name} unfreezing now");
        isFrozen = false;
        isDoingDamage = true;
        ResetSpeed();

        agent.speed = DEFAULT_SPEED;
        agent.SetDestination(target.position);
        enemyAnimator.speed = 1f;
    }

    protected void SetSpeed(float speed)
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


    public virtual void SetDoingDamage(bool doingDamage)
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
        if (isDoingDamage)
        {
            lighthouse.RemoveFromEnemiesList(this);
        }

        enemySpawnManager.RemoveEnemyFromList(this);
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        enemyAnimator.SetBool("Die", true);
        Debug.Log($"{name} has died!");
        agent.isStopped = true;
        agent.enabled = false;

        // float animDuration = enemyAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }

    // Placeholder for abilities (to be overridden by child or component scripts)
    public virtual void UseAbility()
    {
        Debug.Log($"{name} has no special ability!");
    }

    public float GetDamage() => damage;

    protected void setMaxHealth(int h)
    {
        maxHealth = h;
        health = h;
    }

    protected void setDamage(float f)
    {
        damage = f;
    }


}