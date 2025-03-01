using Eflatun.SceneReference;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("Scenes References")]
    [SerializeField] private List<SceneReference> scenesToLoad = new();
    [SerializeField] private SceneReference titleScene;
    private Scene loadingScreenScene;

    [Header("UI")]
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Slider loadingBarSlider;
    [SerializeField] private float sliderSmoothSpeed = 3f;

    [Header("Config")]
    [SerializeField] private float maxLoadDurationBeforeFail = 30f;

    private void Awake()
    {
        loadingScreenScene = SceneManager.GetActiveScene();
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

        SceneManager.UnloadSceneAsync(loadingScreenScene);
    }
}
