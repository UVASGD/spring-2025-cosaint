using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;           // The enemy to spawn
    private Transform areaCenter;          
    public float spawnDiameter = 10f;       // Diameter of the spawn circle
    [SerializeField] private float enemiesMultiplier = 1.5f;         // Number of enemies to spawn per round
    [SerializeField] private int enemiesPerRound = 5;
    public float spawnDelay = 1f;           // Delay between spawns

    private int enemiesSpawned = 0;         // Track how many enemies are spawned
    private float spawnTimer = 0f;
    
    private bool allEnemiesDefeated;          // Timer for delay between spawns
    private RoundManager roundManager;       // Reference to the round manager

    private int currentEnemyCount = 0;
    private void Start()
    {
        Debug.Log($"Spawn Diameter: {spawnDiameter}");
        areaCenter = GameObject.Find("Townhall").transform;
        roundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();

        if (roundManager == null)
        {
            Debug.LogError("RoundManager not assigned!");
        }
    }

    private void Update()
    {

        if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.EnemiesSpawning && enemiesSpawned < enemiesPerRound)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnDelay)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }

        if (enemiesPerRound == enemiesSpawned)
        {
            roundManager.AdvancePhase();
            ResetSpawnedCount();
        }

        if (AreAllEnemiesDefeated() && roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.EnemiesNoLongerSpawning)
        {
            roundManager.AdvancePhase();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || areaCenter == null)
        {
            Debug.LogError("EnemyPrefab or AreaCenter not set!");
            return;
        }

        // Get a random spawn position on the circumference of the circle
        Vector3 spawnPosition = GetRandomPositionOnCircle(spawnDiameter);

        // Spawn the enemy
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemyCount++;
        enemiesSpawned++;
        Debug.Log($"Enemy spawned at {spawnPosition}! Total: {enemiesSpawned}/{enemiesPerRound}");
    }

    private Vector3 GetRandomPositionOnCircle(float diameter)
    {
        // Calculate radius from diameter
        float radius = diameter / 2;
        Debug.Log($"Radius: {radius}"); // Add this line

        // Generate a random angle in radians
        float randomAngle = Random.Range(0f, Mathf.PI * 2);

        // Calculate position on the circle's circumference
        float x = areaCenter.position.x + Mathf.Cos(randomAngle) * radius;
        float z = areaCenter.position.z + Mathf.Sin(randomAngle) * radius;

        // Return the position with the same Y-coordinate as the area center
        return new Vector3(x, 3, z);
    }

    private void ResetSpawnedCount()
    {
        enemiesSpawned = 0;
        Debug.Log("Enemy spawn count reset for the next round.");
    }

    private bool AreAllEnemiesDefeated()
    {
        // Check if there are no active instances of the enemyPrefab in the scene
        return GameObject.FindGameObjectsWithTag(enemyPrefab.tag).Length == 0;
    }

    private void OnDrawGizmos()
    {
        // Visualize the spawn circle in the editor
        if (areaCenter != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Transparent green
            Gizmos.DrawWireSphere(areaCenter.position, spawnDiameter / 2);
        }
    }

    public void DecrementEnemyCount()
    {
        currentEnemyCount--;
    }

    public int GetCurrentEnemyCount()
    {
        return currentEnemyCount;
    }    
    
    public void updateEnemyCount()
    {
        enemiesPerRound = (int)(enemiesPerRound * enemiesMultiplier);
        Debug.Log($"Enemies Per Round: {enemiesPerRound}");
    }

}

