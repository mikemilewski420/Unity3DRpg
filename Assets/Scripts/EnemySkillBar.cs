﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySkillBar : MonoBehaviour
{
    [SerializeField]
    private Character character;

    [SerializeField]
    private Enemy enemy;

    [SerializeField]
    private EnemyAI enemyAI;

    [SerializeField]
    private EnemySkills enemySkills;

    [SerializeField]
    private Image SkillBarFillImage;

    [SerializeField]
    private TextMeshProUGUI SkillName;

    private GameObject CastParticle;

    [SerializeField]
    private bool Casting;

    private float CastTime;

    [SerializeField]
    private float CastParticleSize;

    public Character GetCharacter
    {
        get
        {
            return character;
        }
        set
        {
            character = value;
        }
    }

    public Image GetFillImage
    {
        get
        {
            return SkillBarFillImage;
        }
        set
        {
            SkillBarFillImage = value;
        }
    }

    public TextMeshProUGUI GetSkillName
    {
        get
        {
            return SkillName;
        }
        set
        {
            SkillName = value;
        }
    }

    public Enemy GetEnemy
    {
        get
        {
            return enemy;
        }
        set
        {
            enemy = value;
        }
    }

    public bool GetCasting
    {
        get
        {
            return Casting;
        }
        set
        {
            Casting = value;
        }
    }

    private void OnEnable()
    {
        CastParticle = ObjectPooler.Instance.GetEnemyCastParticle();

        CastParticle.SetActive(true);

        CastParticle.transform.position = new Vector3(character.transform.position.x, character.transform.position.y + 0.6f, character.transform.position.z);

        CastParticle.transform.SetParent(character.transform);

        CastTime = enemySkills.GetManager[enemySkills.GetRandomValue].GetCastTime;
        character.GetComponentInChildren<DamageRadius>().GetShapes = enemySkills.GetManager[enemySkills.GetRandomValue].GetShapes;
        Casting = true;
        SkillBarFillImage.fillAmount = 0;
    }

    private void OnDisable()
    {
        ObjectPooler.Instance.ReturnEnemyCastParticleToPool(CastParticle);

        Casting = false;
        SkillBarFillImage.fillAmount = 0;
    }

    public void GetEnemySkill()
    {
        if(enemySkills.GetManager.Length > 0)
        {
            SkillName.text = enemySkills.GetManager[enemySkills.GetRandomValue].GetSkillName;
            CastTime = enemySkills.GetManager[enemySkills.GetRandomValue].GetCastTime;
        }
    }

    public void ToggleCastBar()
    {
        if (GameManager.Instance.GetEnemyObject == enemy.gameObject)
        {
            GetEnemySkill();
            enemySkills.EnableEnemySkillBar();
        }
        else if (GameManager.Instance.GetEnemyObject != enemy.gameObject)
        {
            GetEnemySkill();
            enemySkills.DisableEnemySkillBar();
        }
    }

    private void LateUpdate()
    {
        SkillBarFillImage.fillAmount += Time.deltaTime / enemySkills.GetManager[enemySkills.GetRandomValue].GetCastTime;
        CastTime -= Time.deltaTime;
        SkillName.text = enemySkills.GetManager[enemySkills.GetRandomValue].GetSkillName;
        if (SkillBarFillImage.fillAmount >= 1)
        {
            enemySkills.GetActiveSkill = false;

            enemySkills.ChooseSkill(enemySkills.GetRandomValue);

            //enemyAI.GetAutoAttack = 0;
            //enemyAI.GetStates = States.Attack;
            enemyAI.GetStates = States.SkillAnimation;

            ObjectPooler.Instance.ReturnEnemyCastParticleToPool(CastParticle);

            SkillBarFillImage.fillAmount = 0;
            CastTime = enemySkills.GetManager[enemySkills.GetRandomValue].GetCastTime;

            gameObject.SetActive(false);
        }
        else if(enemySkills.GetDisruptedSkill)
        {  
            enemySkills.GetActiveSkill = false;

            enemyAI.GetAutoAttack = 0;

            SkillBarFillImage.fillAmount = 0;

            ObjectPooler.Instance.ReturnEnemyCastParticleToPool(CastParticle);

            gameObject.SetActive(false);
        }
    }
}