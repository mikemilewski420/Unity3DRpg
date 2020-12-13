﻿#pragma warning disable 0649
using UnityEngine;

public class ConfirmStat : MonoBehaviour
{
    [SerializeField]
    private Character Knight, ShadowPriest, Toadstool;

    [SerializeField]
    private GameObject StatusButton, Stats, StatParent;

    public void ConfirmStatus(Experience experience)
    {
        if(Knight.gameObject.activeInHierarchy)
        {
            experience = Knight.GetComponent<Experience>();
        }
        if(ShadowPriest.gameObject.activeInHierarchy)
        {
            experience = ShadowPriest.GetComponent<Experience>();
        }
        if (Toadstool.gameObject.activeInHierarchy)
        {
            experience = Toadstool.GetComponent<Experience>();
        }

        if (!GameManager.Instance.GetIsDead)
        {
            experience.GetMaxStatPoints = experience.GetStatPoints;

            AddStats();

            if (experience.GetStatPoints <= 0)
            {
                experience.GetRespecButton.interactable = true;

                Stats.SetActive(false);

                StatusButton.GetComponent<Animator>().SetBool("StatPoints", false);
                gameObject.SetActive(false);
            }
        }
    }

    private void AddStats()
    {
        var stat = StatParent.GetComponentsInChildren<StatUpgrade>();

        for(int i = 0; i < stat.Length; i++)
        {
            stat[i].AddStats();
        }
        SkillsManager.Instance.GetCharacterMenu.SetCharacterInfoText();
    }
}
