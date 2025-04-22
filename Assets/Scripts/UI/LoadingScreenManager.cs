using DG.Tweening;
using Eflatun.SceneReference;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public static Action OnLoadingScreenFinished = delegate { };

    [Header("Scenes References")]
    [SerializeField] private List<SceneReference> scenesToLoad = new();
    [SerializeField] private SceneReference titleScene;
    private Scene loadingScreenScene;

    [Header("UI")]
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Slider loadingBarSlider;
    [SerializeField] private float sliderSmoothSpeed = 3f;
    [SerializeField] private Image fadePanel;

    [Header("Config")]
    [SerializeField] private float maxLoadDurationBeforeFail = 30f;
    [SerializeField] private float afterFinishLoadDelay = 1f;

    private void Awake()
    {
        loadingScreenScene = SceneManager.GetActiveScene();

        fadePanel.color = Color.clear;
    }

    private void Start()
    {
        loadingBarSlider.value = 0;

        StartCoroutine(LoadScenesAsync());
    }

    private IEnumerator LoadScenesAsync()
    {
        float totalProgress = 0f;

        foreach (SceneReference scene in scenesToLoad)
        {
            loadingText.text = $"Loading scene: {scene.Name}";
            float timeElapsed = 0f;

            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.BuildIndex, LoadSceneMode.Additive);

            while (!operation.isDone)
            {
                totalProgress += operation.progress / scenesToLoad.Count;
                loadingBarSlider.value = Mathf.MoveTowards(loadingBarSlider.value, totalProgress, Time.unscaledDeltaTime * sliderSmoothSpeed);

                yield return null;

                timeElapsed += Time.unscaledDeltaTime;
                if(timeElapsed > maxLoadDurationBeforeFail)
                {
                    SceneManager.LoadScene(titleScene.BuildIndex);
                    yield break;
                }
            }
        }
        loadingBarSlider.value = 1f;

        loadingText.text = $"Done!";

        yield return fadePanel.DOColor(Color.black, afterFinishLoadDelay).SetUpdate(true).WaitForCompletion();

        SceneManager.UnloadSceneAsync(loadingScreenScene);
        OnLoadingScreenFinished?.Invoke();
    }
}
