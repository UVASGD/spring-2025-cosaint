using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image cooldownFill;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private TextMeshProUGUI keybindText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI cooldownText;

    private AbilityBase trackedAbility;

    public void Initialize(AbilityBase ability)
    {
        trackedAbility = ability;
        abilityIcon.sprite = ability.AbilityIcon;
        keybindText.text = ability.activationKey.ToString();
        UpdateDisplay();
    }

    void Update()
    {
        if (trackedAbility == null) return;
        
        float timeSinceActivation = Time.time - trackedAbility.LastActivationTime;
        float cooldownLeft = trackedAbility.cooldown - timeSinceActivation;
        bool isOnCooldown = timeSinceActivation < trackedAbility.cooldown;

        // Update cooldown visuals
        cooldownFill.fillAmount = isOnCooldown ? 
            Mathf.Clamp01(1 - (timeSinceActivation / trackedAbility.cooldown)) : 0;

         cooldownText.text = isOnCooldown ? 
             $"{Mathf.CeilToInt(cooldownLeft)}s" : "Ready!";

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        levelText.text = $"Lv.{trackedAbility.CurrentLevel}";
        abilityIcon.color = new Color(1, 1, 1, 
            (trackedAbility.cooldown > Time.time - trackedAbility.LastActivationTime) ? 0.5f : 1f);

    }
}