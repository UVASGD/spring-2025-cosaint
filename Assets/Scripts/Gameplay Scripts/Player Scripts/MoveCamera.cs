using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    
    private Transform cameraPosition;

    private void Start()
    {
        cameraPosition = GameObject.Find("Camera Position").transform;
    }

    private void Update() 
    {
        transform.position = cameraPosition.position;
    }
}
