using TMPro;
using UnityEngine;

public class RoundInfoUI : MonoBehaviour
{
    TextMeshProUGUI timerText;

    TextMeshProUGUI phaseText;

    TextMeshProUGUI currentRoundText;

    TextMeshProUGUI currentEnemyCountText;

    RoundManager roundManager;

    EnemySpawnManager enemySpawnManager;



    private void Start() 
    {
        roundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();
        enemySpawnManager = GameObject.Find("Enemy Spawn Manager").GetComponent<EnemySpawnManager>();
        timerText = GameObject.Find("Timer Text").GetComponent<TextMeshProUGUI>();
        phaseText = GameObject.Find("Round Phase Text").GetComponent<TextMeshProUGUI>();
        currentRoundText = GameObject.Find("Current Round Text").GetComponent<TextMeshProUGUI>();
        currentEnemyCountText = GameObject.Find("Current Enemy Count Text").GetComponent<TextMeshProUGUI>();
    }

    private void Update() 
    {
        timerText.enabled = false;
        currentEnemyCountText.enabled = false;

        if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.ShopPhase ||
            roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.RoundOver)
        {
            float currentTimer = roundManager.GetPhaseTimer();
            timerText.text = "Time Left: " + (int) currentTimer;
            if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.ShopPhase)
            {
                timerText.text += "\nHold Right Shift to Begin Round";
            }
            timerText.enabled = true;
        }

        if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.EnemiesSpawning ||
            roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.EnemiesNoLongerSpawning)
        {
            int currentEnemyCount = enemySpawnManager.GetCurrentEnemyCount();
            currentEnemyCountText.text = "Enemies: " + currentEnemyCount;
            currentEnemyCountText.enabled = true;

        }
        string currentPhase = roundManager.GetCurrentRoundPhase().ToString();
        phaseText.text = "Phase: " + currentPhase;

        string currentRoundNumber = roundManager.GetCurrentRound().ToString();
        currentRoundText.text = "Round: " + currentRoundNumber;
    }
}
