using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    [Header("Visualização")]
    public GameObject highlightObject;

    private GameObject currentTower;
    private bool isSelected = false;

    void Start()
    {
        if (highlightObject != null)
        {
            highlightObject.SetActive(false);
        }
    }

    public void OnMouseEnterSlot()
    {
        if (currentTower == null)
        {
            highlightObject.SetActive(true);
        }
    }

    public void OnMouseExitSlot()
    {
        if (!isSelected) // Só apaga o destaque se não for o slot selecionado
        {
            highlightObject.SetActive(false);
        }
    }

    public void ClickSlot()
    {
        if (currentTower != null)
        {
            return;
        }
        BuildManager.instance.OpenBuildMenu(this);
    }
    

    public void BuildTowerOnSlot(GameObject towerPrefab)
    {
        currentTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
        isSelected = false;
        highlightObject.SetActive(false);
    }
    
    public void OnSelect()
    {
        isSelected = true;
        highlightObject.SetActive(true);
    }
    
    public void OnDeselect()
    {
        isSelected = false;
        highlightObject.SetActive(false);
    }
}