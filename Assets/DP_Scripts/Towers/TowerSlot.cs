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

    // Chamado pelo InputManager quando o mouse entra neste slot
    public void OnMouseEnterSlot()
    {
        if (currentTower == null)
        {
            highlightObject.SetActive(true);
        }
    }

    // Chamado pelo InputManager quando o mouse sai deste slot
    public void OnMouseExitSlot()
    {
        if (!isSelected) // Só apaga o destaque se não for o slot selecionado
        {
            highlightObject.SetActive(false);
        }
    }

    // Chamado pelo InputManager quando este slot é clicado
    public void ClickSlot()
    {
        if (currentTower != null)
        {
            Debug.Log("Este slot já está ocupado!");
            return;
        }
        BuildManager.instance.OpenBuildMenu(this);
    }
    
    // --- Métodos controlados pelo BuildManager (permanecem os mesmos) ---

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