using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    public Material skyboxOne;
    public Material skyboxTwo;

  void Start()
  {
    RenderSettings.skybox = skyboxOne;
  }


    public void SetSkyboxTwo()
    {
        RenderSettings.skybox = skyboxTwo;
    }
}