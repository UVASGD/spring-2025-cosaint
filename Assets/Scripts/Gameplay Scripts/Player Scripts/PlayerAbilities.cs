using UnityEngine;

public class Player : MonoBehaviour
{
    private AbilityManager abilityManager;

    public GameObject prototype1Prefab;
    public GameObject prototype2Prefab;
    public GameObject prototypeBurnPrefab;
    public GameObject prototypeVulnerablePrefab;
    public GameObject prototypeSlowPrefab;
    public GameObject prototypePoisonPrefab;
    public GameObject prototypeBeguilePrefab;

    public Transform orientation;

    private int wisdomPoints = 2;
    //private static bool abilitiesRegistered = false;

    void Awake()
    {
        abilityManager = GameObject.Find("Ability Manager").GetComponent<AbilityManager>();
        abilityManager.AddAbility(new PrototypeAbility(prototype1Prefab, orientation));
        abilityManager.AddAbility(new Prototype2Ability(prototype2Prefab, orientation));
        abilityManager.AddAbility(new PrototypeBurnAbility(prototypeBurnPrefab, orientation));
        abilityManager.AddAbility(new PrototypeVulnerableAbility(prototypeVulnerablePrefab, orientation));
        abilityManager.AddAbility(new PrototypeSlowAbility(prototypeSlowPrefab, orientation));
        abilityManager.AddAbility(new PrototypePoisonAbility(prototypePoisonPrefab, orientation));
        abilityManager.AddAbility(new PrototypeBeguileAbility(prototypeBeguilePrefab, orientation));
        //abilitiesRegistered = true;
    }

    public void AwardWisdomPoints(int points)
    {
        wisdomPoints += points;
    }

    public int GetWisdomPoints()
    {
        return wisdomPoints;
    }

}
