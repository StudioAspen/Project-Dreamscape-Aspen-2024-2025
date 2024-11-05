using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BiomeCardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private WorldManager worldManager;
    [SerializeField, Self] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;

    public Biome CurrentBiome { get; private set; }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        worldManager = FindObjectOfType<WorldManager>();

        button.onClick.AddListener(OnClickCard);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickCard);
    }

    private void Start()
    {
        DisableButton();
    }

    public void EnableButton()
    {
        button.interactable = true;
    }

    public void DisableButton()
    {
        button.interactable = false;
    }

    public void AssignCardBiome(Biome biome)
    {
        CurrentBiome = biome;

        nameText.text = $"{CurrentBiome.ToString()}";
    }

    private void OnClickCard()
    {
        worldManager.AssignBiomeToSpawnNext(CurrentBiome);
    }
}
