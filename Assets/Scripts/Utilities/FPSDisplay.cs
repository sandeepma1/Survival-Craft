using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] private bool enableFPS = true;
    [SerializeField] private Text fpsText;
    private float deltaTime = 0.0f;
    private float msec;
    private float fps;
    private string text;

    private void LateUpdate()
    {
        if (enableFPS)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;
            fpsText.text = fps.ToString("F0");
        }
    }
}