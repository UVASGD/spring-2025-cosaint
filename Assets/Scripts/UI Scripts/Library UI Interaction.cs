using TMPro;
using UnityEngine;

public class LibraryUIInteraction : MonoBehaviour
{
    private Transform libraryTransform;
    [SerializeField] private float interactionRadius = 15f; // Radius for interaction
    private TextMeshProUGUI libraryInteractionText; // Text element to display outside the library
    private Transform playerTransform; // Transform of the player

    private RoundManager roundManager;
    private bool isPlayerInRange = false;

    void Start()
    {
        roundManager = GameObject.Find("Round Manager").GetComponent<RoundManager>();
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        libraryTransform = GameObject.Find("Library").GetComponent<Transform>();
        libraryInteractionText = GameObject.Find("Library Interaction Text").GetComponent<TextMeshProUGUI>();
        libraryInteractionText.enabled = false;
    }

    void Update()
    {

        if (roundManager.getCurrentRoundPhase() != RoundManager.RoundPhase.ShopPhase)
        {
            libraryInteractionText.enabled = false;
        }

        float distanceToLibrary = Vector3.Distance(playerTransform.position, libraryTransform.position);

        if (!isPlayerInRange && distanceToLibrary <= interactionRadius)
        {
            EnterLibraryRange();
        }
        else if (isPlayerInRange && distanceToLibrary > interactionRadius)
        {
            ExitLibraryRange();
        }
    }

    private void EnterLibraryRange()
    {
        isPlayerInRange = true;
        libraryInteractionText.enabled = true;
        Debug.Log("Player entered the library range.");
    }

    private void ExitLibraryRange()
    {
        isPlayerInRange = false;
        libraryInteractionText.enabled = false;
        Debug.Log("Player exited the library range.");
    }

    public bool getIsPlayerInRange()
    {
        return isPlayerInRange;
    }
}