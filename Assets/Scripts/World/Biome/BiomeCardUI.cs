using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BiomeCardUI : MonoBehaviour
{
    private WorldManager worldManager;
    private Button button;

    [Header("References")]
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;

    public Biome CurrentBiome { get; private set; }

    private void Awake()
    {
        worldManager = FindObjectOfType<WorldManager>();
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClickCard);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickCard);
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

        switch (biome)
        {
            case Biome.DREAM:
                image.color = Color.magenta;
                break;
            case Biome.FIRE:
                image.color = Color.red;
                break;
            case Biome.FOOD:
                image.color = new Color(1f, 192f/255f, 203f/255f, 1f);
                break;
            case Biome.BIOME3:
                image.color = Color.white;
                break;
            default:
                break;
        }
    }

    private void OnClickCard()
    {
        worldManager.AssignBiomeToSpawnNext(CurrentBiome);
    }
}
