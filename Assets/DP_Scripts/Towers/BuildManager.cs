using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using TMPro; // Necessário para controlar componentes TextMeshPro

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    [Header("Referências da UI")]
    public GameObject upgradeMenuUI;
    public Image upgradeIcon;
    public TextMeshProUGUI towerNameText;
    public TextMeshProUGUI nivelAtualText;
    public TextMeshProUGUI danoAtualText;
    public TextMeshProUGUI danoUpgradeText;
    public GameObject upgradeButtonUI;
    public TextMeshProUGUI upgradeCostText;
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
        CloseAllMenus();

        if (slot == null)
        {
            Debug.LogError("Slot de torre inválido!");
            return;
        }

        SelectSlot(slot);
        buildMenuUI.SetActive(true);
        blockerFundo.SetActive(true);
    }

    public void OpenUpgradeMenu(TowerSlot slot)
    {
        CloseAllMenus();

        if (slot == null || slot.currentTower == null)
        {
            Debug.LogError("Slot ou torre inválida para upgrade!");
            return;
        }

        TowerData towerData = slot.currentTower.GetComponent<TowerData>();
        if (towerData == null)
        {
            Debug.LogError("O prefab da torre não possui o componente TowerData!");
            return;
        }

        // Atualiza UI
        upgradeIcon.sprite = towerData.icon;
        towerNameText.text = towerData.nomeDaTorre;
        nivelAtualText.text = "Nível Atual: " + towerData.nivel;
        danoAtualText.text = "Dano Atual: " + towerData.danoAtual;
        danoUpgradeText.text = "Dano no Próximo Nível: " + towerData.danoUpgrade;
        upgradeCostText.text = "Custo de Upgrade: " + towerData.custoUpgrade;

        SelectSlot(slot);
        upgradeMenuUI.SetActive(true);
        blockerFundo.SetActive(true);
    }

    private void SelectSlot(TowerSlot slot)
    {
        if (selectedSlot != null)
            selectedSlot.OnDeselect();

        selectedSlot = slot;
        selectedSlot.OnSelect();
    }

    public void UpgradeTower()
    {
        if (selectedSlot == null || selectedSlot.currentTower == null)
        {
            Debug.LogError("Nenhum slot ou torre selecionada para upgrade!");
            return;
        }

        TowerData towerData = selectedSlot.currentTower.GetComponent<TowerData>();
        if (towerData == null)
        {
            Debug.LogError("O prefab da torre não possui o componente TowerData!");
            return;
        }

        // Limite de upgrades: máximo nível 4
        if (towerData.nivel >= 4)
        {
            Debug.LogWarning("A torre já está no nível máximo!");
            return;
        }

        bool transacaoBemSucedida = EconomyManager.Instance.GastarDinheiro(towerData.custoUpgrade);

        if (transacaoBemSucedida)
        {
            towerData.nivel++;
            towerData.danoAtual = towerData.danoUpgrade;
            towerData.danoUpgrade = towerData.danoAtual * 2; // Progressão de dano
            towerData.custoUpgrade = towerData.custoUpgrade * 2; // Progressão de custo

            // Se chegou no nível máximo, pode ocultar o botão de upgrade ou atualizar a UI
            if (towerData.nivel >= 4 && upgradeButtonUI != null)
                upgradeButtonUI.SetActive(false);

            CloseUpgradeMenu();
        }
        else
        {
            Debug.LogWarning("Dinheiro insuficiente para fazer upgrade da torre: " + towerData.custoUpgrade);
        }
    }

    public void CloseUpgradeMenu()
    {
        CloseAllMenus();
    }

    public void CloseBuildMenu()
    {
        CloseAllMenus();
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
            selectedSlot = null;
            CloseBuildMenu();
        }
        else
        {
            Debug.LogWarning("Dinheiro insuficiente para construir a torre: " + towerData.custoDeConstrucao);
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

    public void CloseAllMenus()
    {
        if (selectedSlot != null)
        {
            selectedSlot.OnDeselect();
            selectedSlot = null;
        }

        buildMenuUI.SetActive(false);
        upgradeMenuUI.SetActive(false);
        blockerFundo.SetActive(false);
    }
}