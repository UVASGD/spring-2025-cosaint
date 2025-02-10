using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityUI : MonoBehaviour
{

  List<AbilityBase> currentAbilities;

  AbilityManager abilityManager;

    void Start()
    {
      abilityManager = GameObject.Find("Ability Manager").GetComponent<AbilityManager>();
    }

    void Update()
    {
      currentAbilities = abilityManager.GetAbilities();
    }

}
