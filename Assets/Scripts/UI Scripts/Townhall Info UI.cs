using TMPro;
using UnityEngine;

public class LighthouseInfoUI : MonoBehaviour
{

    Lighthouse lighthouse;
    TextMeshProUGUI lighthouseHealthText;
    
    void Start()
    {
        lighthouse = GameObject.Find("Lighthouse").GetComponent<Lighthouse>();
        lighthouseHealthText = GameObject.Find("Lighthouse Health Text").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        string lighthouseHealth = ((int) lighthouse.GetHealth()).ToString();
        lighthouseHealthText.text = "Lighthouse Health: " +  lighthouseHealth;
    }

}
