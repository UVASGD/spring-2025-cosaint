using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


public class AbilityManagerUI : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject abilityUIPrefab;
    [SerializeField] private float elementSpacing = 10f;

    [Header("References")]
    [SerializeField] private AbilityManager abilityManager;

    private HorizontalLayoutGroup layoutGroup;
    private bool isInitialized = false;
    private List<GameObject> activeAbilitySlots = new List<GameObject>();

    void Start()
    {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = elementSpacing;
        StartCoroutine(DelayedInitialize());
    }

    IEnumerator DelayedInitialize()
    {
        yield return null;
        if (!isInitialized)
        {
            CreateAbilityDisplays();
            isInitialized = true;
        }
    }

    void CreateAbilityDisplays()
    {

    
        if (abilityUIPrefab == null)
        {
            Debug.LogError("AbilityUIPrefab is not assigned!");
            return;
        }

        if (abilityManager == null)
        {
            Debug.LogError("AbilityManager reference is missing!");
            return;
        }

        Debug.Log($"Creating UI for {abilityManager.GetAbilities().Count} abilities");

        foreach (AbilityBase ability in abilityManager.GetAbilities())
        {
            GameObject abilityUI = Instantiate(abilityUIPrefab, transform);
            abilityUI.GetComponent<AbilityUI>().Initialize(ability);
        }
    }

    void RefreshUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        // Recreate UI
        CreateAbilityDisplays();
    }
}