using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    #region Singleton
    public static EconomyManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    [Header("Configurações da Economia")]
    public int dinheiroInicial = 0;
    public static event Action<int> OnDinheiroAlterado;

    public int DinheiroAtual { get; private set; }

    void Start()
    {
        DinheiroAtual = dinheiroInicial;
        OnDinheiroAlterado?.Invoke(DinheiroAtual);
    }

    public void AdicionarDinheiro(int quantidade)
    {
        if (quantidade < 0) return;

        DinheiroAtual += quantidade;
        OnDinheiroAlterado?.Invoke(DinheiroAtual);
    }

    public bool GastarDinheiro(int quantidade)
    {
        if (quantidade < 0) return false;

        if (DinheiroAtual >= quantidade)
        {
            DinheiroAtual -= quantidade;
            OnDinheiroAlterado?.Invoke(DinheiroAtual);
            return true;
        }
        else
        {
            return false;
        }
    }
}