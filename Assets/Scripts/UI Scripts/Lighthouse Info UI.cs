using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// I don't think this is used anywhere. 
/// Likely should be deprecated
/// </summary>

public class LighthouseInfoUI : MonoBehaviour
{

    [SerializeField] Lighthouse lighthouse;
    [SerializeField] TextMeshProUGUI lighthouseHealthText;
    [SerializeField] RectMask2D mask;
    //[SerializeField] Slider lighthouseHealthBar;

    private float lighthouseMaxHealth;

    private void Start()
    {
        lighthouseMaxHealth = lighthouse.GetStartingHealth();
        Debug.Log("This script actually runs");
    }

    void Update()
    {
        string lighthouseHealth = ((int) lighthouse.GetHealth()).ToString();
        lighthouseHealthText.text = "Lighthouse Health: " +  lighthouseHealth;
        var padding = mask.padding;
        padding.z = ((lighthouseMaxHealth - lighthouse.GetHealth()) / lighthouseMaxHealth) * 330;
        mask.padding = padding;
        Debug.Log(mask.padding.z);
        //lighthouseHealthBar.value = (lighthouse.GetHealth() / lighthouseMaxHealth);
    }

}
