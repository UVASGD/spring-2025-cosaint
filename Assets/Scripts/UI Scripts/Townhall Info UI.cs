using TMPro;
using UnityEngine;

public class TownhallInfoUI : MonoBehaviour
{

    Townhall townhall;
    TextMeshProUGUI townhallHealthText;
    
    void Start()
    {
        townhall = GameObject.Find("Townhall").GetComponent<Townhall>();
        townhallHealthText = GameObject.Find("Townhall Health Text").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        string townhallHealth = ((int) townhall.GetHealth()).ToString();
        townhallHealthText.text = "Townhall Health: " +  townhallHealth;
    }

}
