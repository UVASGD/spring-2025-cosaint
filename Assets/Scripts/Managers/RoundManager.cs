using UnityEngine;
using System.Collections;

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

    [Header("Shop Music")]
    [SerializeField] private AudioClip[] shopMusic;

    [Header("Enemy Phase Music")]
[SerializeField] private AudioClip enemyPhaseMusic;


    private AudioSource audioSource;

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
        currentRound = 1;
        enemySpawnManager = GameObject.Find("Enemy Spawn Manager").GetComponent<EnemySpawnManager>();
        

        if (enemySpawnManager == null)
        {
            Debug.LogError("EnemySpawnManager not assigned!");
        }
        

        SetRoundPhase(RoundPhase.ShopPhase);
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
        

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
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

    private IEnumerator FadeOutAndChangeMusic(AudioClip newClip, float fadeDuration = 1.0f)
{
    if (audioSource == null)
    {
        yield break;
    }

    float startVolume = audioSource.volume;

    // Fade out
    while (audioSource.volume > 0)
    {
        audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
        yield return null;
    }

    // Switch clip
    audioSource.Stop();
    audioSource.clip = newClip;
    audioSource.Play();

    // Fade in
    while (audioSource.volume < startVolume)
    {
        audioSource.volume += startVolume * Time.deltaTime / fadeDuration;
        yield return null;
    }

    // Ensure it's exactly back to the start volume
    audioSource.volume = startVolume;
}

    private IEnumerator FadeOutAndStopMusic(float fadeDuration = 1.0f)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private void PlayRandomShopMusic()
    {
        if (shopMusic == null || shopMusic.Length == 0)
        {
            Debug.LogWarning("No shop music assigned!");
            return;
        }

        int randomIndex = Random.Range(0, shopMusic.Length);
        AudioClip selectedTrack = shopMusic[randomIndex];

        if (selectedTrack != null)
        {
            StartCoroutine(FadeOutAndChangeMusic(selectedTrack));
            Debug.Log($"Fading into shop music track: {selectedTrack.name}");
        }
    }

    private void PlayEnemyPhaseMusic()
    {
        if (enemyPhaseMusic == null)
        {
            Debug.LogWarning("No enemy phase music assigned!");
            return;
        }

        StartCoroutine(FadeOutAndChangeMusic(enemyPhaseMusic));
        Debug.Log("Fading into enemy phase music");
    }

    private void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            StartCoroutine(FadeOutAndStopMusic());
        }
        else
        {
            Debug.LogWarning("Tried to stop music but AudioSource is null or not playing");
        }
    }

    public void SetRoundPhase(RoundPhase roundPhase)
    {
        RoundPhase previousPhase = this.roundPhase;
        this.roundPhase = roundPhase;

        if (previousPhase == RoundPhase.ShopPhase && roundPhase != RoundPhase.ShopPhase)
        {
            StopMusic();
            Debug.Log("Shop music stopped");
        }

        switch (roundPhase)
        {
            case RoundPhase.ShopPhase:
                phaseTimer = shopPhaseDuration;
                PlayRandomShopMusic();
                break;

            case RoundPhase.EnemiesSpawning:
                StopMusic();
                PlayEnemyPhaseMusic();
                break;

            case RoundPhase.EnemiesNoLongerSpawning:
                phaseTimer = 0;
                break;

            case RoundPhase.RoundOver:
            StopMusic();
                phaseTimer = roundOverDuration;
                StopMusic();
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
        enemySpawnManager.UpdateEnemyCount();
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