using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerButtonUI : MonoBehaviour
{
    [Header("Referências do Prefab")]
    public TextMeshProUGUI nomeTorreTexto;
    public TextMeshProUGUI custoTorreTexto;
    public TextMeshProUGUI descricaoTorreTexto;
    public Image iconeTorreImagem;
    public Button botaoDeCompra;

    public void Setup(GameObject towerPrefab, BuildManager buildManager)
    {
        // Pega os dados do prefab da torre
        TowerData towerData = towerPrefab.GetComponent<TowerData>();
        if (towerData == null)
        {
            Debug.LogError("O prefab " + towerPrefab.name + " não tem o componente TowerData!");
            gameObject.SetActive(false); // Desativa o botão se não tiver dados
            return;
        }

        // Pega o sprite do prefab da torre
        SpriteRenderer towerSprite = towerPrefab.GetComponent<SpriteRenderer>();
        if (towerSprite == null)
        {
            Debug.LogError("O prefab " + towerPrefab.name + " não tem o componente SpriteRenderer!");
            gameObject.SetActive(false);
            return;
        }

        // Atualiza os elementos da UI com os dados da torre
        if (nomeTorreTexto != null) nomeTorreTexto.text = towerData.nomeDaTorre;
        if (custoTorreTexto != null) custoTorreTexto.text = towerData.custoDeConstrucao.ToString();
        if (descricaoTorreTexto != null) descricaoTorreTexto.text = towerData.descricao;
        if (iconeTorreImagem != null) iconeTorreImagem.sprite = towerSprite.sprite;

        // Configura o clique do botão para chamar o BuildManager
        if (botaoDeCompra != null)
        {
            botaoDeCompra.onClick.AddListener(() => {
                buildManager.BuildTower(towerPrefab);
            });
        }
    }
}