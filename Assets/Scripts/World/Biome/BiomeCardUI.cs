using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BiomeCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private WorldManager worldManager;
    private Button button;

    [Header("References")]
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject backgroundGlow;

    public Biome CurrentBiome { get; private set; }

    private bool isSelected;

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

    private void OnDisable()
    {
        isSelected = false;
    }

    public void EnableButton()
    {
        button.interactable = true;

        if(isSelected) EnableSelectedIndicator();
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
        OnSelect(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselect(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;

        EnableSelectedIndicator();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;

        DisableSelectedIndicator();
    }

    public void EnableSelectedIndicator()
    {
        backgroundGlow.SetActive(true);
    }

    public void DisableSelectedIndicator()
    {
        backgroundGlow.SetActive(false);
    }
}
