using UnityEngine;
using TMPro; // NOVO: Necessário para controlar componentes TextMeshPro

public class UIManager : MonoBehaviour
{
    [Header("Referências da UI")]
    [Tooltip("Arraste o objeto de texto que exibe o dinheiro para este campo.")]
    public TextMeshProUGUI textoDeDinheiro;

    private void OnEnable()
    {
        EconomyManager.OnDinheiroAlterado += AtualizarTextoDeDinheiro;
    }

    private void OnDisable()
    {
        EconomyManager.OnDinheiroAlterado -= AtualizarTextoDeDinheiro;
    }
    private void AtualizarTextoDeDinheiro(int novaQuantidade)
    {
        if (textoDeDinheiro != null)
        {
            // Atualiza o texto na tela para a nova quantidade.
            textoDeDinheiro.text = novaQuantidade.ToString();
        }
    }
}