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

        if (roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.ShopPhase ||
            roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.RoundOver)
        {
            float currentTimer = roundManager.GetPhaseTimer();
            timerText.text = "Time Left: " + (int) currentTimer; 
            timerText.enabled = true;
        }

        string currentPhase = roundManager.GetCurrentRoundPhase().ToString();
        phaseText.text = "Phase: " + currentPhase;

        string currentRoundNumber = roundManager.GetCurrentRound().ToString();
        currentRoundText.text = "Round: " + currentRoundNumber;
    }
}
