using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LibraryUI : MonoBehaviour
{

    [SerializeField] private List<AbilityBase> abilities = new List<AbilityBase>();

    private AbilityManager abilityManager;
    private Player player;

    private LibraryUIInteraction libraryUIInteraction;

    private int wisdomPoints;

    private RoundManager roundManager;

    private TextMeshProUGUI wisdomPointsText;

    private GameObject libraryUI;

    private bool libraryActive;

    void Start()
    {
        libraryActive = false;
        libraryUI = GameObject.Find("Library UI");
        roundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();
        libraryUIInteraction = GameObject.Find("Library Interaction Text").GetComponent<LibraryUIInteraction>();
        abilityManager = GameObject.Find("Ability Manager").GetComponent<AbilityManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        wisdomPointsText = GameObject.Find("Wisdom Points Text").GetComponent<TextMeshProUGUI>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && 
            libraryUIInteraction.getIsPlayerInRange() &&
            roundManager.getCurrentRoundPhase() == RoundManager.RoundPhase.ShopPhase && 
            !libraryActive) //In range and want to get into library UI
        {
            libraryUI.SetActive(true);
            libraryActive = true;

        } else if (libraryActive && Input.GetKeyDown(KeyCode.L)) //In library UI within range but want to get out
        {
            libraryUI.SetActive(false);
            libraryActive = false;

        } else if (!libraryUIInteraction.getIsPlayerInRange()) //Player not in range, always have libraryUI off
        {
            libraryUI.SetActive(false);
            libraryActive = false;

        } else if (libraryActive){ // In library UI, keep updating information
            wisdomPoints = player.GetWisdomPoints();
            wisdomPointsText.text = $"Wisdom Points: {wisdomPoints}";
            abilities = abilityManager.GetAbilities();
        }
    }

       

    
}
