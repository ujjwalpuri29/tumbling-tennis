using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    [SerializeField] private float targetWidth = 16f;
    [SerializeField] private float targetHeight = 9f;

    private Camera cam;
    private int previousWidth;
    private int previousHeight;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateCameraRect();
    }

    private void Update()
    {
        if (Screen.width != previousWidth || Screen.height != previousHeight)
            UpdateCameraRect();
    }

    private void UpdateCameraRect()
    {
        previousWidth = Screen.width;
        previousHeight = Screen.height;

        float targetAspect = targetWidth / targetHeight;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
        {
            cam.rect = new Rect(0f, (1f - scaleHeight) / 2f, 1f, scaleHeight);
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;
            cam.rect = new Rect((1f - scaleWidth) / 2f, 0f, scaleWidth, 1f);
        }
    }
}