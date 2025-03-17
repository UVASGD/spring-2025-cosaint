using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LighthouseInfoUI : MonoBehaviour
{

    [SerializeField] Lighthouse lighthouse;
    [SerializeField] TextMeshProUGUI lighthouseHealthText;
    [SerializeField] Slider lighthouseHealthBar;

    private float lighthouseMaxHealth;

    private void Start()
    {
        lighthouseMaxHealth = lighthouse.GetStartingHealth();
    }

    void Update()
    {
        string lighthouseHealth = ((int) lighthouse.GetHealth()).ToString();
        lighthouseHealthText.text = "Lighthouse Health: " +  lighthouseHealth;
        lighthouseHealthBar.value = (lighthouse.GetHealth() / lighthouseMaxHealth);
    }

}
