using Eflatun.SceneReference;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private List<SceneReference> scenesToLoad;
    private Scene loadingScreenScene;

    [Header("UI")]
    [SerializeField] private Slider loadingBarSlider;

    [Header("Config")]
    [SerializeField] private float afterLoadDelay = 1f;

    private void Awake()
    {
        loadingScreenScene = SceneManager.GetActiveScene();

        StartCoroutine(LoadScenesAsync(scenesToLoad));
    }

    private void Start()
    {
        loadingBarSlider.value = 0;
    }

    private IEnumerator LoadScenesAsync(List<SceneReference> scenes)
    {
        List<AsyncOperation> operations = new List<AsyncOperation>();

        foreach (SceneReference scene in scenes)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;
            operations.Add(operation);
        }

        while (!AreAsyncOperationsFinished(operations))
        {
            float totalProgress = 0f;

            foreach (AsyncOperation operation in operations)
            {
                totalProgress += operation.progress;
            }

            loadingBarSlider.value = totalProgress / operations.Count;

            yield return null;
        }

        // Activate all scenes once loading is complete
        foreach (AsyncOperation operation in operations)
        {
            operation.allowSceneActivation = true;
        }

        SceneManager.UnloadSceneAsync(loadingScreenScene);
    }

    private bool AreAsyncOperationsFinished(List<AsyncOperation> operations)
    {
        // Check if all async operations are complete (i.e., their progress is >= 0.9f)
        foreach (AsyncOperation operation in operations)
        {
            if (operation.progress < 0.9f)
            {
                return false; // If any operation is not yet done, return false
            }
        }

        return true; // All operations are complete
    }
}
