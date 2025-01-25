using TMPro;
using UnityEngine;

public class RoundInfoUI : MonoBehaviour
{
    TextMeshProUGUI timerText;

    TextMeshProUGUI phaseText;

    TextMeshProUGUI currentRoundText;

    RoundManager roundManager;

    private void Start() 
    {
        roundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();
        timerText = GameObject.Find("Timer Text").GetComponent<TextMeshProUGUI>();
        phaseText = GameObject.Find("Round Phase Text").GetComponent<TextMeshProUGUI>();
        currentRoundText = GameObject.Find("Current Round Text").GetComponent<TextMeshProUGUI>();

    }

    private void Update() 
    {
        timerText.enabled = false;

        if (roundManager.getCurrentRoundPhase() == RoundManager.RoundPhase.ShopPhase ||
            roundManager.getCurrentRoundPhase() == RoundManager.RoundPhase.RoundOver)
        {
            float currentTimer = roundManager.getPhaseTimer();
            timerText.text = "Time Left: " + (int) currentTimer; 
            timerText.enabled = true;
        }

        string currentPhase = roundManager.getCurrentRoundPhase().ToString();
        phaseText.text = "Phase: " + currentPhase;

        string currentRoundNumber = roundManager.getCurrentRound().ToString();
        currentRoundText.text = "Round: " + currentRoundNumber;
    }
}
