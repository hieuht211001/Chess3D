using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher Instance { get; private set; }
    public Camera[] cameras;
    private int currentCameraIndex;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == 0);
        }
        currentCameraIndex = 0;
        SwitchCamera();
        SwitchCamera();
    }

    public void SwitchCamera()
    {
        // Disable the current camera
        cameras[currentCameraIndex].gameObject.SetActive(false);

        // Update the index to the next camera
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;

        // Enable the new current camera
        cameras[currentCameraIndex].gameObject.SetActive(true);
    }

    public Camera GetCurrentCamera()
    {
        return cameras[currentCameraIndex];
    }
}