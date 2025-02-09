using AYellowpaper.SerializedCollections;
using Eflatun.SceneReference;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private List<SceneReference> scenesToLoad = new();
    private Scene loadingScreenScene;

    [Header("UI")]
    [SerializeField] private Slider loadingBarSlider;
    [SerializeField] private float sliderSmoothSpeed = 1f;

    [Header("Config")]
    [SerializeField] private float afterLoadDelay = 1f;

    private void Awake()
    {
        loadingScreenScene = SceneManager.GetActiveScene();

        StartCoroutine(LoadScenesAsync());
    }

    private void Start()
    {
        loadingBarSlider.value = 0;
    }

    private IEnumerator LoadScenesAsync()
    {
        Debug.Log("Starting to load scenes...");

        List<AsyncOperation> operations = new List<AsyncOperation>();
        float totalProgress = 0f;

        foreach (SceneReference scene in scenesToLoad)
        {
            Debug.Log($"Loading scene: {scene.Name}");

            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;
            operations.Add(operation);

            while (operation.progress < 0.9f) // Scene is still loading
            {
                // Calculate the total progress
                totalProgress = operations.Sum(op => op.progress) / scenesToLoad.Count;

                // Move towards the target progress at a constant speed
                loadingBarSlider.value = Mathf.MoveTowards(loadingBarSlider.value, totalProgress, Time.unscaledDeltaTime * sliderSmoothSpeed);

                yield return null;
            }
        }

        // Smoothly transition to full progress (1f) after all scenes are loaded
        while (loadingBarSlider.value < 1f)
        {
            loadingBarSlider.value = Mathf.MoveTowards(loadingBarSlider.value, 1f, Time.unscaledDeltaTime * sliderSmoothSpeed);
            yield return null;
        }

        loadingBarSlider.value = 1f;

        yield return new WaitForSecondsRealtime(afterLoadDelay);

        // Activate all scenes once loading is complete
        foreach (AsyncOperation operation in operations)
        {
            operation.allowSceneActivation = true;
        }
        Debug.Log("Activating scenes");

        SceneManager.UnloadSceneAsync(loadingScreenScene);
    }
}
