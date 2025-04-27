using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;           // The enemy to spawn
    public GameObject tankEnemyPrefab;
    public GameObject runnerEnemyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;          // Array of spawn points to choose from
    
    private Transform areaCenter;          
    public float spawnDiameter = 10f;       // Kept for backwards compatibility
    [SerializeField] private float enemiesMultiplier = 1.5f;         // Number of enemies to spawn per round
    [SerializeField] private int enemiesPerRound = 5;
    public float spawnDelay = 1f;           // Delay between spawns

    private int enemiesSpawned = 0;         // Track how many enemies are spawned
    private float spawnTimer = 0f;

    private bool allEnemiesDefeated;          // Timer for delay between spawns
    private RoundManager roundManager;       // Reference to the round manager
    private HashSet<Enemy> aliveEnemies = new HashSet<Enemy>();

    private void Start()
    {
        Debug.Log($"Spawn Points: {spawnPoints.Length}");
        areaCenter = GameObject.Find("Lighthouse").transform;
        roundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();

        if (roundManager == null)
        {
            Debug.LogError("RoundManager not assigned!");
        }
        
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned! Will fall back to circle spawn method.");
        }
    }

private void Update()
{
    if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.EnemiesSpawning)
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnDelay && enemiesSpawned < enemiesPerRound)
        {
            SpawnEnemy();
            SpawnTankEnemy();
            spawnTimer = 0f;
        }

        if (enemiesSpawned >= enemiesPerRound)
        {
            roundManager.AdvancePhase();
            ResetSpawnedCount();
        }
    }
    else if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.EnemiesNoLongerSpawning)
    {
        if (AreAllEnemiesDefeated())
        {
            Debug.Log("All enemies defeated!");
            roundManager.AdvancePhase();
        }
    }
}

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyPrefab not set!");
            return;
        }

        // Get a random spawn position from the spawn points array
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        // Spawn the enemy and track it in aliveEnemies
        Enemy spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponentInChildren<Enemy>();
        aliveEnemies.Add(spawnedEnemy);

        enemiesSpawned++;
        Debug.Log($"Enemy spawned at {spawnPosition}! Total: {enemiesSpawned}/{enemiesPerRound}");
    }

    private void SpawnTankEnemy()
    {
        if (tankEnemyPrefab == null)
        {
            Debug.LogError("TankEnemyPrefab not set!");
            return;
        }

        // Get a random spawn position from the spawn points array
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Spawn the enemy and track it in aliveEnemies
        Enemy spawnedEnemy = Instantiate(tankEnemyPrefab, spawnPosition, Quaternion.identity).GetComponentInChildren<TankEnemy>();
        aliveEnemies.Add(spawnedEnemy);

        enemiesSpawned++;
        Debug.Log($"Tank enemy spawned at {spawnPosition}! Total: {enemiesSpawned}/{enemiesPerRound}");
    }

    private void SpawnRunnerEnemy()
    {
        if (runnerEnemyPrefab == null)
        {
            Debug.LogError("RunnerEnemyPrefab not set!");
            return;
        }

        // Get a random spawn position from the spawn points array
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Spawn the enemy and track it in aliveEnemies
        Enemy spawnedEnemy = Instantiate(runnerEnemyPrefab, spawnPosition, Quaternion.identity).GetComponentInChildren<RunnerEnemy>();
        aliveEnemies.Add(spawnedEnemy);

        enemiesSpawned++;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // If spawn points are available, use them
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            // Select a random spawn point
            Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return selectedSpawnPoint.position;
        }
        else
        {
            // Fall back to the original circle spawn method if no spawn points are assigned
            return GetRandomPositionOnCircle(spawnDiameter);
        }
    }

    // Keep the original method for backward compatibility
    private Vector3 GetRandomPositionOnCircle(float diameter)
    {
        // Calculate radius from diameter
        float radius = diameter / 2;

        // Generate a random angle in radians
        float randomAngle = Random.Range(0f, Mathf.PI * 2);

        // Calculate position on the circle's circumference
        float x = areaCenter.position.x + Mathf.Cos(randomAngle) * radius;
        float z = areaCenter.position.z + Mathf.Sin(randomAngle) * radius;

        // Return the position with the same Y-coordinate as the area center
        return new Vector3(x, 1, z);
    }

    public void RemoveEnemyFromList(Enemy enemy)
    {
        aliveEnemies.Remove(enemy);
    }

    private void ResetSpawnedCount()
    {
        enemiesSpawned = 0;
        Debug.Log("Enemy spawn count reset for the next round.");
    }

    public int GetAliveEnemiesCount()
    {
        return aliveEnemies.Count;
    }

    public Enemy[] GetEnemies()
    {
        Enemy[] enemies = new Enemy[aliveEnemies.Count];
        aliveEnemies.CopyTo(enemies);
        return enemies;
    }

    private bool AreAllEnemiesDefeated()
    {
        Debug.Log("enemies all DEFEATED CALLED");
        // Check if there are no active instances of the enemyPrefab in the scene
        return aliveEnemies.Count == 0;
    }

    private void OnDrawGizmos()
    {
        // Visualize the spawn points in the editor
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Gizmos.color = Color.green;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawSphere(spawnPoint.position, 0.5f);
                }
            }
        }
        // Also visualize the circle spawn area for backward compatibility
        else if (areaCenter != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Transparent green
            Gizmos.DrawWireSphere(areaCenter.position, spawnDiameter / 2);
        }
    }

    public void UpdateEnemyCount()
    {
        enemiesPerRound = (int)(enemiesPerRound * enemiesMultiplier);
        Debug.Log($"Enemies Per Round: {enemiesPerRound}");
    }
}