using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance { get; private set; }

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    private float startingAmplitude;
    private float startingFrequency;
    private float shakeTimer;
    private float shakeDuration;

    private void Awake()
    {
        if (Instance != null) Destroy(Instance.gameObject);
        Instance = this;

        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {

    }

    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        cinemachineBasicMultiChannelPerlin.ReSeed();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = amplitude;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = frequency;
        startingAmplitude = amplitude;
        startingFrequency = frequency;
        shakeDuration = duration;
        shakeTimer = duration;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.unscaledDeltaTime;

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
                Mathf.Lerp(startingAmplitude, 0, 1 - (shakeTimer / shakeDuration));
            cinemachineBasicMultiChannelPerlin.m_FrequencyGain =
                Mathf.Lerp(startingFrequency, 0, 1 - (shakeTimer / shakeDuration));
        }
    }
}