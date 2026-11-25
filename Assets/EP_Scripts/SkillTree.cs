using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTree : MonoBehaviour
{
    public List<Skill> habilidades;
    public GameObject painelSkillTree;
    private bool painelAtivo = false;
    public ToolTip tooltip;

    void Awake()
    {
        if (habilidades.Count >= 3)
        {
            habilidades[1].prerequisito = habilidades[0];
            habilidades[2].prerequisito = habilidades[1];
            habilidades[4].prerequisito = habilidades[3];
            habilidades[5].prerequisito = habilidades[4];
            habilidades[7].prerequisito = habilidades[6];
            habilidades[8].prerequisito = habilidades[7];
            habilidades[10].prerequisito = habilidades[9];
            habilidades[11].prerequisito = habilidades[10];
            habilidades[13].prerequisito = habilidades[12];
            habilidades[14].prerequisito = habilidades[13];
            habilidades[16].prerequisito = habilidades[15];
            habilidades[17].prerequisito = habilidades[16];
            habilidades[19].prerequisito = habilidades[18];
            habilidades[22].prerequisito = habilidades[21];
            habilidades[23].prerequisito = habilidades[22];
            habilidades[25].prerequisito = habilidades[24];
            habilidades[26].prerequisito = habilidades[25];
            habilidades[28].prerequisito = habilidades[27];
            habilidades[29].prerequisito = habilidades[28];
            habilidades[31].prerequisito = habilidades[30];
            habilidades[32].prerequisito = habilidades[31];
        }
    }

    void Start()
    {
        painelSkillTree.SetActive(false);

        foreach (Skill skill in habilidades)
        {
            Skill localSkill = skill;
            skill.botao.onClick.AddListener(() => TentarDesbloquear(localSkill));
            AtualizarVisual(skill);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            painelAtivo = !painelAtivo;
            painelSkillTree.SetActive(painelAtivo);
            tooltip.tooltipObject.SetActive(false);
        }
        foreach (Skill skill in habilidades)
        {
            AtualizarVisual(skill);
        }
    }

    void TentarDesbloquear(Skill skill)
    {
        if (skill.desbloqueada)
            return; 

        if (skill.prerequisito == null || skill.prerequisito.desbloqueada)
        {
            skill.desbloqueada = true;
            AtualizarVisual(skill);
        }
        else
        {
            Debug.Log("Anterior antes kk");
        }
    }

    void AtualizarVisual(Skill skill)
    {
        if (skill.desbloqueada)
            skill.botao.GetComponent<Image>().color = Color.green;
        else if (skill.prerequisito != null && !skill.prerequisito.desbloqueada)
            skill.botao.GetComponent<Image>().color = Color.red; // bloqueada
        else
            skill.botao.GetComponent<Image>().color = Color.gray; // disponível para desbloquear
    }
}