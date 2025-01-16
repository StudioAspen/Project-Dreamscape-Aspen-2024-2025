using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BiomeCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        nameText.text = worldManager.BiomeDatabase.BiomesDictionary[CurrentBiome].BiomeName;
        image.color = worldManager.BiomeDatabase.BiomesDictionary[CurrentBiome].BiomeColor;
    }

    private void OnClickCard()
    {
        worldManager.AssignBiomeToSpawnNext(CurrentBiome);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
