using UnityEngine;

public class RoundManager : MonoBehaviour
{

    public static RoundManager Instance { get; private set; }

    private RoundPhase roundPhase;
    private int currentRound = 0;
    private float phaseTimer;

    public float shopPhaseDuration = 5f;
    public float roundOverDuration = 10f;


    public RoundPhase getCurrentRoundPhase() => roundPhase;
    public int getCurrentRound() => currentRound;
    public float getPhaseTimer() => phaseTimer;

    private Player player;

    private bool RShiftIsPressed = false;

    public enum RoundPhase
    {
        ShopPhase,
        EnemiesSpawning,
        EnemiesNoLongerSpawning,
        RoundOver,
        GameOver
    }


    public void setRoundPhase(RoundPhase roundPhase)
    {
        this.roundPhase = roundPhase;

           switch (roundPhase)
        {
            case RoundPhase.ShopPhase:
                break;

            case RoundPhase.EnemiesSpawning:
                break;

            case RoundPhase.EnemiesNoLongerSpawning:
                phaseTimer = 0;
                break;

            case RoundPhase.RoundOver:
                Debug.Log($"Round {currentRound} is over!");
                break;

            case RoundPhase.GameOver:
                Debug.Log("Game Over!");
                break;
        }
    }

    public void setCurrentRound(int round) => currentRound = round;


    public void goToNextRound()
    {
        currentRound++;
        player.AwardWisdomPoints(2);
        setRoundPhase(RoundPhase.ShopPhase);
    }

    public void AdvancePhase()
    {
        switch (roundPhase)
        {
            case RoundPhase.ShopPhase:
                setRoundPhase(RoundPhase.EnemiesSpawning);
                break;

            case RoundPhase.EnemiesSpawning:
                setRoundPhase(RoundPhase.EnemiesNoLongerSpawning);
                break;

            case RoundPhase.EnemiesNoLongerSpawning:
                setRoundPhase(RoundPhase.RoundOver);
                phaseTimer = roundOverDuration;
                break;

            case RoundPhase.RoundOver:
                goToNextRound();
                break;
        }
    }



    private void Start() 
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        setRoundPhase(RoundPhase.ShopPhase);
        setCurrentRound(1);
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
            AdvancePhase();
            RShiftIsPressed = false;
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
                phaseTimer = 0;
            }
        }
    }

    



}
