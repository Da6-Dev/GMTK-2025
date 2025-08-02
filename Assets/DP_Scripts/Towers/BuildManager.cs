using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    [Header("Referências da UI")]
    public GameObject buildMenuUI;
    public GameObject blockerFundo;
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
            buildMenuUI.SetActive(true);
            blockerFundo.SetActive(true);
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
            blockerFundo.SetActive(false);
        }
    }

    public void BuildTower(GameObject towerPrefab)
    {
        if (selectedSlot == null)
        {
            Debug.LogError("Nenhum slot selecionado para construir!");
            return;
        }

        TowerData towerData = towerPrefab.GetComponent<TowerData>();
        if (towerData == null)
        {
            Debug.LogError("O prefab da torre '" + towerPrefab.name + "' não possui o componente TowerData! Não é possível verificar o custo.");
            return;
        }

        bool transacaoBemSucedida = EconomyManager.Instance.GastarDinheiro(towerData.custoDeConstrucao);

        if (transacaoBemSucedida)
        {
            selectedSlot.BuildTowerOnSlot(towerPrefab);
            CloseBuildMenu();
        }
        else
        {
        }
    }

    void PopulateBuildMenu()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject towerPrefab in availableTowers)
        {
            GameObject buttonGO = Instantiate(towerButtonPrefab, buttonContainer);

            TowerButtonUI towerButton = buttonGO.GetComponent<TowerButtonUI>();
            if (towerButton != null)
            {
                towerButton.Setup(towerPrefab, this);
            }
            else
            {
                Debug.LogError("O prefab do botão de torre não possui o script TowerButtonUI!");
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonContainer as RectTransform);
    }
}