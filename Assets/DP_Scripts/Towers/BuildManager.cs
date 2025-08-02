using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    [Header("Referências da UI")]
    public GameObject buildMenuUI;
    public GameObject towerButtonPrefab;
    public Transform buttonContainer;

    [Header("Configuração de Construção")]
    public List<GameObject> availableTowers = new List<GameObject>();

    private TowerSlot selectedSlot;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        PopulateBuildMenu();

        if (buildMenuUI != null)
        {
            // Garante que o menu comece fechado.
            buildMenuUI.SetActive(false);
        }
    }

    // MÉTODO AJUSTADO
    public void OpenBuildMenu(TowerSlot slot)
    {
        if (selectedSlot != null)
        {
            selectedSlot.OnDeselect();
        }

        selectedSlot = slot;
        selectedSlot.OnSelect();

        if (buildMenuUI != null)
        {
            // Apenas ativa o menu. Ele aparecerá na posição em que
            // você o deixou no Editor. A linha de posição foi removida.
            buildMenuUI.SetActive(true);
        }
    }

    public void CloseBuildMenu()
    {
        if (selectedSlot != null)
        {
            selectedSlot.OnDeselect();
            selectedSlot = null;
        }

        if (buildMenuUI != null)
        {
            buildMenuUI.SetActive(false);
        }
    }

    public void BuildTower(GameObject towerPrefab)
    {
        if (selectedSlot == null)
        {
            return;
        }
        
        selectedSlot.BuildTowerOnSlot(towerPrefab);
        Debug.Log("Torre " + towerPrefab.name + " construída!");
        
        CloseBuildMenu();
    }

    void PopulateBuildMenu()
    {
        // Limpa botões antigos para evitar duplicatas.
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Cria os botões.
        foreach (GameObject towerPrefab in availableTowers)
        {
            GameObject buttonGO = Instantiate(towerButtonPrefab, buttonContainer);
            Image buttonImage = buttonGO.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = towerPrefab.GetComponent<SpriteRenderer>().sprite;
            }
            Button button = buttonGO.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => {
                    BuildTower(towerPrefab);
                });
            }
        }

        // Força o container a recalcular o layout (IMPORTANTE para os botões aparecerem).
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonContainer as RectTransform);
    }
}