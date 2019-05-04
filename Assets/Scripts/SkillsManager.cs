﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance = null;

    private ParticleSystem ParticleObj = null;

    [SerializeField]
    private List<Skills> skills;

    [SerializeField]
    private bool ActivatedSkill;

    public bool GetActivatedSkill
    {
        get
        {
            return ActivatedSkill;
        }
        set
        {
            ActivatedSkill = value;
        }
    }

    public ParticleSystem GetParticleObj
    {
        get
        {
            return ParticleObj;
        }
        set
        {
            ParticleObj = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (skills[0].GetComponent<Button>().GetComponent<Image>().fillAmount >= 1 && skills[0].GetCharacter.CurrentHealth > 0
                                                   && skills[0].GetCharacter.CurrentMana >= skills[0].GetManaCost && skills[0].GetComponent<Button>().interactable)
            {
                skills[0].GetComponent<Button>().onClick.Invoke();
            }
            else if (skills[0].GetCharacter.CurrentMana < skills[0].GetManaCost)
            {
                GameManager.Instance.ShowNotEnoughManaText();
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (skills[1].GetComponent<Button>().GetComponent<Image>().fillAmount >= 1 && skills[1].GetCharacter.CurrentHealth > 0
                                                  && skills[1].GetCharacter.CurrentMana >= skills[1].GetManaCost && skills[1].GetComponent<Button>().interactable)
            {
                skills[1].GetComponent<Button>().onClick.Invoke();
            }
            else if (skills[1].GetCharacter.CurrentMana < skills[1].GetManaCost)
            {
                GameManager.Instance.ShowNotEnoughManaText();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (skills[2].GetComponent<Button>().GetComponent<Image>().fillAmount >= 1 && skills[2].GetCharacter.CurrentHealth > 0
                                                  && skills[2].GetCharacter.CurrentMana >= skills[2].GetManaCost && skills[2].GetComponent<Button>().interactable)
            {
                skills[2].GetComponent<Button>().onClick.Invoke();
            }
            else if (skills[2].GetCharacter.CurrentMana < skills[2].GetManaCost)
            {
                GameManager.Instance.ShowNotEnoughManaText();
            }
        }
    }

    public void DeactivateSkillButtons()
    {
        foreach(Skills s in skills)
        {
            s.GetComponent<Button>().interactable = false;
        }
    }

    public void ReactivateSkillButtons()
    {
        foreach (Skills s in skills)
        {
            s.GetComponent<Button>().interactable = true;
        }
    }
}
