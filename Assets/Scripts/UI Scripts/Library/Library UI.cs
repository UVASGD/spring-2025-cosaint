using UnityEngine;
using TMPro; // Use TextMeshPro

public class LibraryUI : MonoBehaviour
{
    // References set in Inspector
    [SerializeField] private GameObject libraryUIPanel;
    [SerializeField] private LibraryUIInteraction libraryUIInteraction;
    [SerializeField] private TextMeshProUGUI wisdomPointsText;
    [SerializeField] private TextMeshProUGUI slotsTakenText;
    [SerializeField] private AbilityManager abilityManager;

    // References likely found automatically or assigned
    private Player player;
    private RoundManager roundManager;
    private ShopManager shopManager;

    private bool libraryActive = false;

    void Start()
    {
        // Find necessary components (using FindFirstObjectByType for modern Unity)
        if (!libraryUIPanel) libraryUIPanel = gameObject;
        if (!player) player = FindFirstObjectByType<Player>();
        if (!roundManager) roundManager = FindFirstObjectByType<RoundManager>();
        if (!shopManager) shopManager = FindFirstObjectByType<ShopManager>();

        // Ensure panel starts inactive
        if (libraryUIPanel != null)
        {
            libraryUIPanel.SetActive(false);
            libraryActive = false;
        }
        else Debug.LogError("Library UI Panel reference not set!");

        // Initial Wisdom Points display
        UpdateDisplays();

    }

    void Update()
    {
        // Check components exist before using them
        if (libraryUIInteraction == null || roundManager == null) return;

        bool canAccessLibrary = libraryUIInteraction.getIsPlayerInRange() &&
                                roundManager.GetCurrentRoundPhase() == RoundManager.RoundPhase.ShopPhase;

        if (Input.GetKeyDown(KeyCode.L)) // Toggle key
        {
            if (canAccessLibrary && !libraryActive)
            {
                OpenLibrary();
            }
            else if (libraryActive) // If already open, L closes it
            {
                CloseLibrary();
            }
        }

        // Auto-close if player moves out of range or phase changes
        if (libraryActive && (!libraryUIInteraction.getIsPlayerInRange() || roundManager.GetCurrentRoundPhase() != RoundManager.RoundPhase.ShopPhase))
        {
            CloseLibrary();
        }
    }

    void OpenLibrary()
    {
        if (libraryUIPanel != null)
        {
            libraryUIPanel.SetActive(true);
            libraryActive = true;
            Time.timeScale = 0f; // Optional: Pause game time

            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true;                  // Make cursor visible

            UpdateDisplays();
            shopManager?.RefreshShopItemStates();
            Debug.Log("Library Opened");
        }
    }

    void CloseLibrary()
    {
        if (libraryUIPanel != null)
        {
            libraryUIPanel.SetActive(false);
            libraryActive = false;
            Time.timeScale = 1f; // Optional: Resume game time

            Cursor.lockState = CursorLockMode.Locked; // Re-lock the cursor for gameplay
            Cursor.visible = false;                 // Hide cursor for gameplay

            Debug.Log("Library Closed");
        }
    }

    public void UpdateDisplays()
    {
        if (!player) player = FindFirstObjectByType<Player>();

        if (wisdomPointsText != null && slotsTakenText != null && player != null)
        {
            wisdomPointsText.text = $"Wisdom Points: {player.GetWisdomPoints()}";
            slotsTakenText.text = $"Slots Taken: {player.GetSlotsUsed()} / {player.GetMaxSlots()}";
        }
        else
        {
            if (wisdomPointsText == null)
                Debug.LogWarning("LibraryUI: Wisdom Points Text not assigned.");
            if (slotsTakenText == null)
                Debug.LogWarning("LibraryUI: Slots Taken Text not assigned.");
            if (player == null)
                Debug.LogWarning("LibraryUI: Player reference is missing.");
        }
    }
}