using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    [Header("Visualização")]
    public GameObject highlightObject;
    public GameObject nohighlightObject;

    public GameObject currentTower;
    private bool isSelected = false;

    void Start()
    {
        if (highlightObject != null)
        {
            highlightObject.SetActive(false);
            nohighlightObject.SetActive(true);
        }
    }

    public void OnMouseEnterSlot()
    {
        if (currentTower == null)
        {
            highlightObject.SetActive(true);
            nohighlightObject.SetActive(false);
        }
    }

    public void OnMouseExitSlot()
    {
        if (!isSelected && currentTower == null) // Só apaga o destaque se não for o slot selecionado
        {
            highlightObject.SetActive(false);
            nohighlightObject.SetActive(true);
        }
    }

    public void ClickSlot()
    {
        if (currentTower != null)
        {
            BuildManager.instance.OpenUpgradeMenu(this);
        }else
        {
            BuildManager.instance.OpenBuildMenu(this);
        }
    }


    public void BuildTowerOnSlot(GameObject towerPrefab)
    {
        //Fazer a torre ser intanciada aqui dentro do slot como filha do slot
        currentTower = Instantiate(towerPrefab, transform.position, Quaternion.identity, transform);
        TowerData towerData = currentTower.GetComponent<TowerData>();
        if (towerData == null)
        {
            Debug.LogError("O prefab da torre não possui o componente TowerData!");
            return;
        }
        // Configurar a torre com os dados do prefab
        towerData.nivel = 1; // Iniciar no nível 1
        towerData.danoUpgrade = towerData.danoAtual * 2; // Exemplo de dano de upgrade
        towerData.custoUpgrade = towerData.custoDeConstrucao * 2; // Exemplo de custo de upgrade
        isSelected = false;
        highlightObject.SetActive(false);
        nohighlightObject.SetActive(false);
    }

    public void OnSelect()
    {
        isSelected = true;
        highlightObject.SetActive(true);
        nohighlightObject.SetActive(false);
    }

    public void OnDeselect()
    {
        isSelected = false;
        highlightObject.SetActive(false);
        nohighlightObject.SetActive(true);
    }
}