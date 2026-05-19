using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // Canvas смотрит в ту же сторону что и камера
        transform.rotation = mainCamera.transform.rotation;
    }
}