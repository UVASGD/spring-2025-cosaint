using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }
    private RoundPhase roundPhase;
    private Player player;
    private int currentRound;
    private float phaseTimer;
    [SerializeField] private float shopPhaseDuration = 5f;
    [SerializeField] private float roundOverDuration = 10f;
    private bool RShiftIsPressed = false;

    private EnemySpawnManager enemySpawnManager;

    public enum RoundPhase
    {
        ShopPhase,
        EnemiesSpawning,
        EnemiesNoLongerSpawning,
        RoundOver,
        GameOver
    }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        SetRoundPhase(RoundPhase.ShopPhase);
        currentRound = 1;
        enemySpawnManager = GameObject.Find("Enemy Spawn Manager").GetComponent<EnemySpawnManager>();

        if (enemySpawnManager == null)
        {
            Debug.LogError("EnemySpawnManager not assigned!");
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {

        if (roundPhase == RoundPhase.RoundOver || RShiftIsPressed)
        {
            phaseTimer -= Time.deltaTime;

            if (phaseTimer <= 0)
            {
                RShiftIsPressed = false;
                AdvancePhase();
            }
        }

        if (roundPhase == RoundPhase.ShopPhase)
        {
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                Debug.Log("RShift Pressed");
                RShiftIsPressed = true;
                phaseTimer = shopPhaseDuration;
            }
            if (Input.GetKeyUp(KeyCode.RightShift))
            {
                Debug.Log("RShift Released");
                RShiftIsPressed = false;
                phaseTimer = shopPhaseDuration;
            }
        }
    }

    public RoundPhase GetCurrentRoundPhase() => roundPhase;
    public float GetPhaseTimer() => phaseTimer;
    public int GetCurrentRound() => currentRound;

    public void SetRoundPhase(RoundPhase roundPhase)
    {
        this.roundPhase = roundPhase;

        switch (roundPhase)
        {
            case RoundPhase.ShopPhase:
                phaseTimer = shopPhaseDuration;
                break;

            case RoundPhase.EnemiesSpawning:
                break;

            case RoundPhase.EnemiesNoLongerSpawning:
                phaseTimer = 0;
                break;

            case RoundPhase.RoundOver:
                phaseTimer = roundOverDuration;
                Debug.Log($"Round {currentRound} is over!");
                break;

            case RoundPhase.GameOver:
                Debug.Log("Game Over!");
                break;
        }
    }

    public void GoToNextRound()
    {
        currentRound++;
        enemySpawnManager.updateEnemyCount();
        player.AwardWisdomPoints(2);
        SetRoundPhase(RoundPhase.ShopPhase);
    }

    public void AdvancePhase()
    {
        switch (roundPhase)
        {
            case RoundPhase.ShopPhase:
                SetRoundPhase(RoundPhase.EnemiesSpawning);
                break;

            case RoundPhase.EnemiesSpawning:
                SetRoundPhase(RoundPhase.EnemiesNoLongerSpawning);
                break;

            case RoundPhase.EnemiesNoLongerSpawning:
                SetRoundPhase(RoundPhase.RoundOver);
                break;

            case RoundPhase.RoundOver:
                GoToNextRound();
                break;
        }
    }
}
