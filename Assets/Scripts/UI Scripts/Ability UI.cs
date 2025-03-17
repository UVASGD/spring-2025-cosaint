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
        
        float timeSinceActivation = Time.time - trackedAbility.lastActivatedTime;
        float cooldownLeft = trackedAbility.cooldownTime - timeSinceActivation;
        bool isOnCooldown = timeSinceActivation < trackedAbility.cooldownTime;

        // Update cooldown visuals
        cooldownFill.fillAmount = isOnCooldown ? 
            Mathf.Clamp01(1 - (timeSinceActivation / trackedAbility.cooldownTime)) : 0;

         cooldownText.text = isOnCooldown ? 
             $"{Mathf.CeilToInt(cooldownLeft)}s" : "Ready!";

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        levelText.text = $"Lv.{trackedAbility.level}";
        abilityIcon.color = new Color(1, 1, 1, 
            (trackedAbility.cooldownTime > Time.time - trackedAbility.lastActivatedTime) ? 0.5f : 1f);
    }
}