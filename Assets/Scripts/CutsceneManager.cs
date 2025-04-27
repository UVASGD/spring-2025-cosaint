using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector cutsceneDirector;

    void Start()
    {
        if (cutsceneDirector != null)
        {
            cutsceneDirector.Play();
        }
        else
        {
            Debug.LogWarning("CutsceneManager: No PlayableDirector assigned!");
        }
    }
}
