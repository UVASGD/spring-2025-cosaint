using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;           // The enemy to spawn
    public GameObject tankEnemyPrefab;
    private Transform areaCenter;          
    public float spawnDiameter = 10f;       // Diameter of the spawn circle
    [SerializeField] private float enemiesMultiplier = 1.5f;         // Number of enemies to spawn per round
    [SerializeField] private int enemiesPerRound = 5;
    [SerializeField] private float tankEnemiesMultiplier = 1.1f; // Number of tank enemies to spawn
    [SerializeField] private int tankEnemiesPerRound = 0; //Only start spawning on round 3(in last method below) 
    public float spawnDelay = 1f;           // Delay between spawns

    private int tankEnemiesSpawned = 0; //Track # of tank enemies
    private int enemiesSpawned = 0;         // Track how many enemies are spawned
    private float spawnTimer = 0f;
    
    private bool allEnemiesDefeated;          // Timer for delay between spawns
    private RoundManager roundManager;       // Reference to the round manager
    private HashSet<Enemy> aliveEnemies = new HashSet<Enemy>();

    private void Start()
    {
        Debug.Log($"Spawn Diameter: {spawnDiameter}");
        areaCenter = GameObject.Find("Lighthouse").transform;
        roundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();

        if (roundManager == null)
        {
            Debug.LogError("RoundManager not assigned!");
        }
    }

    private void Update()
    {

        if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.EnemiesSpawning && (enemiesSpawned < enemiesPerRound))
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnDelay)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }
        if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.EnemiesSpawning && (tankEnemiesSpawned < tankEnemiesPerRound) && (enemiesSpawned == enemiesPerRound))
        {
            Debug.Log(tankEnemiesPerRound);
            Debug.Log(tankEnemiesSpawned);
            Debug.Log(tankEnemiesSpawned < tankEnemiesPerRound);
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnDelay)
            {
                SpawnTankEnemy();
                spawnTimer = 0f;
            }
        }

        if ((enemiesPerRound == enemiesSpawned) && (tankEnemiesPerRound == tankEnemiesSpawned))
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



        // Spawn the enemy and track it in aliveEnemies
        Enemy spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<Enemy>();
        aliveEnemies.Add(spawnedEnemy);

        enemiesSpawned++;
        Debug.Log($"Enemy spawned at {spawnPosition}! Total: {enemiesSpawned}/{enemiesPerRound}");
    }

    private void SpawnTankEnemy()
    {
        if(tankEnemyPrefab == null || areaCenter == null)
        {
            Debug.LogError("TankEnemyPrefab or AreaCenter not set!");
            return;
        }
        // Get a random spawn position on the circumference of the circle
        Vector3 spawnPosition = GetRandomPositionOnCircle(spawnDiameter);

        // Spawn the tank enemy and track it in aliveEnemies
        TankEnemy spawnedEnemy = Instantiate(tankEnemyPrefab, spawnPosition, Quaternion.identity).GetComponent<TankEnemy>();
        aliveEnemies.Add(spawnedEnemy);

        tankEnemiesSpawned++;
        Debug.Log($"Enemy spawned at {spawnPosition}! Total: {tankEnemiesSpawned}/{tankEnemiesPerRound}");

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

    public void RemoveEnemyFromList(Enemy enemy)
    {
        aliveEnemies.Remove(enemy);
    }

    private void ResetSpawnedCount()
    {
        enemiesSpawned = 0;
        tankEnemiesSpawned = 0;
        Debug.Log("Enemy spawn count reset for the next round.");
        Debug.Log("Tank enemy spawn count reset for the next round.");
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
        // Check if there are no active instances of the enemyPrefab or tank enemy in the scene
        return (GameObject.FindGameObjectsWithTag(enemyPrefab.tag).Length == 0) && (GameObject.FindGameObjectsWithTag(tankEnemyPrefab.tag).Length == 0);
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
    
    public void UpdateEnemyCount()
    {
        enemiesPerRound = (int)(enemiesPerRound * enemiesMultiplier);
        Debug.Log($"Enemies Per Round: {enemiesPerRound}");
        if (enemiesPerRound == 11.25) { //Only start spawning tank enemies on round 3
            tankEnemiesPerRound = 1;
        }
        tankEnemiesPerRound = (int)(tankEnemiesPerRound * tankEnemiesMultiplier);
        Debug.Log($"Tank enemies Per Round: {tankEnemiesPerRound}");

    }

}

