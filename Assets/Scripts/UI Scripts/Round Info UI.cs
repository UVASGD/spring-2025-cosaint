using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundInfoUI : MonoBehaviour
{
    /*TextMeshProUGUI timerText;

    //TextMeshProUGUI phaseText;

    TextMeshProUGUI currentRoundText;

    TextMeshProUGUI currentEnemyCountText;*/

    [SerializeField] Lighthouse lighthouse;

    [SerializeField] RectMask2D mask;

    RoundManager roundManager;

    EnemySpawnManager enemySpawnManager;

    TextMeshProUGUI HUDtext;

    TextMeshProUGUI RightShiftText;

    private string[] textFields = new string[3];
    private float lighthouseMaxHealth;


    private void Start() 
    {
        roundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();
        enemySpawnManager = GameObject.Find("Enemy Spawn Manager").GetComponent<EnemySpawnManager>();
        HUDtext = GameObject.Find("HUD Text").GetComponent<TextMeshProUGUI>();
        RightShiftText = GameObject.Find("Right Shift Text").GetComponent <TextMeshProUGUI>();
        RightShiftText.enabled = false;

        textFields[0] = "Round: 0";
        textFields[1] = "Enemies: 0";
        textFields[2] = "";
        HUDtext.text = textFields[0] + "\n" + textFields[1] + "\n" + textFields[2];
        lighthouseMaxHealth = lighthouse.GetStartingHealth();
        /*timerText = GameObject.Find("Timer Text").GetComponent<TextMeshProUGUI>();
        phaseText = GameObject.Find("Round Phase Text").GetComponent<TextMeshProUGUI>();
        currentRoundText = GameObject.Find("Current Round Text").GetComponent<TextMeshProUGUI>();
        currentEnemyCountText = GameObject.Find("Current Enemy Count Text").GetComponent<TextMeshProUGUI>();*/
    }

    private void Update() 
    {
        if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.ShopPhase ||
            roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.RoundOver)
        {
            float currentTimer = roundManager.GetPhaseTimer();
            textFields[2] = "Time Left: " + (int) currentTimer;
            if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.ShopPhase)
            {
                RightShiftText.enabled = true;
            }
            else 
            { 
                RightShiftText.enabled = false; 
            }
        }
        else
        {
            textFields[2] = "";
            RightShiftText.enabled = false;
        }

        if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.EnemiesSpawning ||
            roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.EnemiesNoLongerSpawning)
        {
            int currentEnemyCount = enemySpawnManager.GetAliveEnemiesCount();
            textFields[1] = "Enemies: " + currentEnemyCount;

        }
        else
        {
            textFields[1] = "Enemies: 0";
        }
        //string currentPhase = roundManager.GetCurrentRoundPhase().ToString();
        //phaseText.text = "Phase: " + currentPhase;

        string currentRoundNumber = roundManager.GetCurrentRound().ToString();
        textFields[0] = "Round: " + currentRoundNumber;

        HUDtext.text = textFields[0] + "\n" + textFields[1] + "\n" + textFields[2];

        var padding = mask.padding;
        padding.z = ((lighthouseMaxHealth - lighthouse.GetHealth()) / lighthouseMaxHealth) * 328;
        mask.padding = padding;
    }
}
