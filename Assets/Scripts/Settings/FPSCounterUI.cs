using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text fpsText;

    [Header("Config")]
    [SerializeField] private float updateInterval = 0.25f;
    [SerializeField] private int rollingWindowSize = 10;

    private float fps;
    private float[] windowFrames;
    private int currentFrame = 0;

    private float timeSinceLastUpdate = 0f;

    private void Start()
    {
        windowFrames = new float[rollingWindowSize];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            fpsText.gameObject.SetActive(!fpsText.gameObject.activeSelf);
        }

        if (!fpsText.gameObject.activeSelf) return;
        // Track time taken for the current frame
        float frameTime = Time.unscaledDeltaTime;

        // Store the frame time in the rolling window
        windowFrames[currentFrame] = frameTime;

        // Move to the next frame, and wrap around if necessary
        currentFrame = (currentFrame + 1) % rollingWindowSize;

        // Calculate the average frame time for the window
        float totalFrameTime = 0f;
        for (int i = 0; i < rollingWindowSize; i++)
        {
            totalFrameTime += windowFrames[i];
        }

        // Calculate FPS (FPS = 1 / frameTime)
        fps = (int)(1f / (totalFrameTime / rollingWindowSize));

        timeSinceLastUpdate += Time.unscaledDeltaTime;
        if(timeSinceLastUpdate >= updateInterval)
        {
            // Display the FPS every update interval
            fpsText.text = $"FPS: {fps}";

            timeSinceLastUpdate = 0f;
        }
    }
}
