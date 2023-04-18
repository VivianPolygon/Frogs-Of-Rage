using UnityEngine;

public class CameraControlManager : MonoBehaviour
{
    public UIManager uiManager;
    public GameObject cameraController;

    private void Update()
    {
        if (uiManager.state == CanvasState.Player || uiManager.state == CanvasState.Death)
        {
            EnableCameraControl(true);
        }
        else
        {
            EnableCameraControl(false);
        }
    }

    private void EnableCameraControl(bool enable)
    {
        if (cameraController != null)
        {
            cameraController.SetActive(enable);
        }
        else
        {
            Debug.LogError("CameraController is not assigned!");
        }
    }
}
