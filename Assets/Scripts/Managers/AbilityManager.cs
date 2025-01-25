using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private List<AbilityBase> abilities = new List<AbilityBase>();

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        foreach (var ability in abilities)
        {
            if (Input.GetKeyDown(ability.activationKey))
            {
                ability.Activate();
            }
        }
    }

    public AbilityBase GetAbilityByName(string name)
    {
        return abilities.Find(ability => ability.abilityName == name);
    }

    public void AddAbility(AbilityBase ability)
    {
        if (!abilities.Contains(ability))
        {
            abilities.Add(ability);
        }
        else
        {
            Debug.Log($"Ability {ability.abilityName} is already added.");
        }
    }

    public void RemoveAbility(AbilityBase ability)
    {
        if (abilities.Contains(ability))
        {
            abilities.Remove(ability);
        }
        else
        {
            Debug.Log($"Ability {ability.abilityName} not found in the list.");
        }
    }


    public List<AbilityBase> GetAbilities()
    {
        return new List<AbilityBase>(abilities);
    }
}