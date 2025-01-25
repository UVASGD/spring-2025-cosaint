using UnityEngine;

public abstract class AbilityBase
{

    public string abilityName { get; private set; }
    public KeyCode activationKey { get; private set; }
    public float cooldownTime { get; private set; }

    public float level { get; set; }
    private float lastActivatedTime;

    public AbilityBase(string name, KeyCode key, float cTime)
    {
        abilityName = name;
        activationKey = key;
        cooldownTime = cTime;
        level = 1;
    }

    public void Activate()
    {

        // Check if the current game phase allows ability activation
        if (RoundManager.Instance.getCurrentRoundPhase() != RoundManager.RoundPhase.EnemiesSpawning &&
            RoundManager.Instance.getCurrentRoundPhase() != RoundManager.RoundPhase.EnemiesNoLongerSpawning)
        {
            Debug.Log($"{abilityName} cannot be activated during the {RoundManager.Instance.getCurrentRoundPhase()} phase.");
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