using UnityEngine;

public class Player : MonoBehaviour
{
    private AbilityManager abilityManager;

    public GameObject prototype1Prefab;
    public GameObject prototype2Prefab;

    public Transform orientation;

    private int wisdomPoints = 2;

    private void Start()
    {
        abilityManager = GameObject.Find("Ability Manager").GetComponent<AbilityManager>();
        abilityManager.AddAbility(new PrototypeAbility(prototype1Prefab, orientation));
        abilityManager.AddAbility(new Prototype2Ability(prototype2Prefab, orientation));
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
