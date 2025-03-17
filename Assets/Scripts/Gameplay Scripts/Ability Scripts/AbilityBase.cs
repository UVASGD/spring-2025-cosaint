using UnityEngine;

public abstract class AbilityBase
{
    public string abilityName { get; private set; }
    public KeyCode activationKey { get; private set; }
    public float cooldownTime { get; private set; }
    public float level { get; set; }
    public Sprite AbilityIcon { get; protected set; }
    public float lastActivatedTime { get; protected set; }

    public AbilityBase(string name, KeyCode key, float cTime)
    {
        abilityName = name;
        activationKey = key;
        cooldownTime = cTime;
        level = 1;
        

        //LoadIcon();
        //ValidateIcon();
    }

    private void LoadIcon()
    {
        string path = $"AbilityIcons/{abilityName}";
        AbilityIcon = Resources.Load<Sprite>(path);

        // Additional debug information
        if (AbilityIcon == null)
        {
            Debug.LogError($"Failed to load icon for: {abilityName}");
            Debug.Log($"Tried path: 'Assets/Resources/{path}.png'");
            Debug.Log("Check:\n" +
                "1. Exact spelling/casing match\n" +
                "2. File exists in Resources/AbilityIcons\n" +
                "3. File is imported as Sprite (2D and UI)\n" +
                "4. No hidden file extensions");
        }
    }

    private void ValidateIcon()
    {
        #if UNITY_EDITOR
        // Editor-only validation
        if (AbilityIcon == null)
        {
            string fullPath = System.IO.Path.Combine(
                Application.dataPath, 
                "Resources", 
                "AbilityIcons", 
                abilityName + ".png"
            );
            
            Debug.LogWarning($"File exists check: {System.IO.File.Exists(fullPath)}");
        }
        #endif
    }
    public void Activate()
    {

        // Check if the current game phase allows ability activation
        if (RoundManager.Instance.GetCurrentRoundPhase() != RoundManager.RoundPhase.EnemiesSpawning &&
            RoundManager.Instance.GetCurrentRoundPhase() != RoundManager.RoundPhase.EnemiesNoLongerSpawning)
        {
            Debug.Log($"{abilityName} cannot be activated during the {RoundManager.Instance.GetCurrentRoundPhase()} phase.");
            return;
        }


        if (Time.time >= lastActivatedTime + cooldownTime)
        {
            lastActivatedTime = Time.time;
            Execute();
        }
        else
        {
            Debug.Log($"{abilityName} is on cooldown.");
        }
    }


    protected abstract void Execute();

    public abstract void UpgradeAbility();


}