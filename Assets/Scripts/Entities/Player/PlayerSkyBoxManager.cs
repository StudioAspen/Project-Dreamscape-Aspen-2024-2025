using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkyBoxManager : MonoBehaviour
{
    private WorldManager worldManager;

    private Vector2Int previousGridPosition = new Vector2Int(int.MaxValue, int.MaxValue);

    [Header("Config")]
    [SerializeField] private float skyBoxChangeFadeDuration = 0.5f;
    private Sequence fadeSequence;

    private const string SKYBOX_HORIZON_COLOR = "_HorizonColor";
    private const string SKYBOX_SKY_COLOR = "_SkyColor";
    private const string SKYBOX_STAR_COLOR = "_StarColor";
    private const string SKYBOX_HORIZON_THICKNESS = "_HorizonThickness";

    private void Start()
    {
        worldManager = FindObjectOfType<WorldManager>();
    }

    private void Update()
    {
        DetectLandChange();
    }

    /// <summary>
    /// Uses the player's grid position to detect changes in player land
    /// </summary>
    private void DetectLandChange()
    {
        Vector2Int currentGridPosition = worldManager.GetGridPosition(transform.position);
        if (previousGridPosition != currentGridPosition)
        {
            previousGridPosition = currentGridPosition;
            if (worldManager.SpawnedLands.TryGetValue(currentGridPosition, out LandManager newLand))
            {
                OnPlayerEnterNewLand(newLand);
            }
        }
    }

    /// <summary>
    /// Uses the player's grid position to detect changes in player land
    /// </summary>
    /// <param name="newLand">The new land the player stepped on</param>
    private void OnPlayerEnterNewLand(LandManager newLand)
    {
        newLand.UpdateWwiseState();

        if (RenderSettings.skybox == null) return;
        if(newLand.SkyBoxMaterial == null) return;
        if (RenderSettings.skybox == newLand.SkyBoxMaterial) return; // Prevent redundant changes

        if (!RenderSettings.skybox.HasColor(SKYBOX_SKY_COLOR)) return;
        if (!RenderSettings.skybox.HasColor(SKYBOX_HORIZON_COLOR)) return;
        if (!RenderSettings.skybox.HasColor(SKYBOX_STAR_COLOR)) return;
        if (!RenderSettings.skybox.HasFloat(SKYBOX_HORIZON_THICKNESS)) return;
        if (!newLand.SkyBoxMaterial.HasColor(SKYBOX_SKY_COLOR)) return;
        if (!newLand.SkyBoxMaterial.HasColor(SKYBOX_HORIZON_COLOR)) return;
        if (!newLand.SkyBoxMaterial.HasColor(SKYBOX_STAR_COLOR)) return;
        if (!newLand.SkyBoxMaterial.HasFloat(SKYBOX_HORIZON_THICKNESS)) return;

        // Clone the skybox material to avoid modifying the original
        Material skyBoxMaterial = new Material(RenderSettings.skybox);
        RenderSettings.skybox = skyBoxMaterial;

        if (fadeSequence != null) fadeSequence.Kill();

        fadeSequence = DOTween.Sequence();

        fadeSequence.Join(skyBoxMaterial.DOColor(newLand.SkyBoxMaterial.GetColor(SKYBOX_SKY_COLOR), SKYBOX_SKY_COLOR, skyBoxChangeFadeDuration))
                    .Join(skyBoxMaterial.DOColor(newLand.SkyBoxMaterial.GetColor(SKYBOX_HORIZON_COLOR), SKYBOX_HORIZON_COLOR, skyBoxChangeFadeDuration))
                    .Join(skyBoxMaterial.DOColor(newLand.SkyBoxMaterial.GetColor(SKYBOX_STAR_COLOR), SKYBOX_STAR_COLOR, skyBoxChangeFadeDuration))
                    .Join(skyBoxMaterial.DOFloat(newLand.SkyBoxMaterial.GetFloat(SKYBOX_HORIZON_THICKNESS), SKYBOX_HORIZON_THICKNESS, skyBoxChangeFadeDuration));

        fadeSequence.Play();
    }
}
