﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasicAttack : MonoBehaviour
{
    [SerializeField]
    private Character character;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private PlayerAnimations playerAnimations;

    [SerializeField] [Tooltip("Current targeted object. Keep this empty!")]
    private Enemy Target = null;

    [SerializeField]
    private ParticleSystem HitParticle;

    private ParticleSystem Obj = null;

    private bool ParticleExists;

    [SerializeField]
    private float MouseRange, AttackRange, AttackDelay, AutoAttackTime, HideStatsDistance;

    public float GetAutoAttackTime
    {
        get
        {
            return AutoAttackTime;
        }
        set
        {
            AutoAttackTime = value;
        }
    }

    public Enemy GetTarget
    {
        get
        {
            return Target;
        }
        set
        {
            Target = value;
        }
    }

    private void Awake()
    {
        cam.GetComponent<Camera>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            MousePoint();
        }

        if (Target != null)
        {
            Attack();
        }
    }

    private void MousePoint()
    {
        Vector3 MousePos = Input.mousePosition;

        Ray ray = cam.ScreenPointToRay(MousePos);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, MouseRange))
        {
            if (hit.collider.GetComponent<Enemy>())
            {
                if (hit.collider.GetComponent<Character>().CurrentHealth > 0)
                {
                    AutoAttackTime = 0;
                    Target = hit.collider.GetComponent<Enemy>();

                    GameManager.Instance.GetEventSystem.SetSelectedGameObject(Target.gameObject);
                    GameManager.Instance.GetLastObject = GameManager.Instance.GetEventSystem.currentSelectedGameObject;

                    Target.GetFilledBar();
                }
            }
            else
            {
                if (!IsPointerOnUIObject())
                {
                    GameManager.Instance.GetEventSystem.SetSelectedGameObject(null);
                    GameManager.Instance.GetLastObject = null;
                    Target = null;
                    AutoAttackTime = 0;
                }
            }
        }
    }

    //Checks to see if the mouse is positioned over a UI element(s).
    private bool IsPointerOnUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    private void Attack()
    {
        if(Vector3.Distance(this.transform.position, Target.transform.position) <= AttackRange)
        {
            if(Target.GetCharacter.CurrentHealth > 0)
            {
                if(!SkillsManager.Instance.GetActivatedSkill)
                {
                    AutoAttackTime += Time.deltaTime;
                }
                else if(SkillsManager.Instance.GetActivatedSkill)
                {
                    AutoAttackTime = 0;
                }
                if (AutoAttackTime >= AttackDelay && Target != null && !SkillsManager.Instance.GetActivatedSkill)
                {
                    Vector3 TargetPosition = new Vector3(Target.transform.position.x - this.transform.position.x, 0, 
                                                         Target.transform.position.z - this.transform.position.z).normalized;

                    Quaternion LookDir = Quaternion.LookRotation(TargetPosition);

                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, LookDir, 5 * Time.deltaTime);

                    playerAnimations.AttackAnimation();
                    if(Target.GetAI.GetIsHostile == false)
                    {
                        Target.GetAI.GetSphereTrigger.gameObject.SetActive(true);
                        Target.GetAI.GetPlayerTarget = this.character;
                    }
                }
            }
            else
            {
                playerAnimations.EndAttackAnimation();
                Target = null;
                AutoAttackTime = 0;
            }
        }
        else
        {
            playerAnimations.EndAttackAnimation();
        }
        if(Target != null)
        {
            if(Vector3.Distance(this.transform.position, Target.transform.position) >= HideStatsDistance)
            {
                Target.GetSkills.DisableEnemySkillBar();
                Target = null;
                GameManager.Instance.GetEventSystem.SetSelectedGameObject(null);
                GameManager.Instance.GetLastObject = null;
                AutoAttackTime = 0;
            }
        }
    }

    public Text TakeDamage()
    {
        if(Target == null)
        {
            return null;
        }

        var DamageObject = Instantiate(Target.GetComponentInChildren<Health>().GetDamageText);

        DamageObject.transform.SetParent(Target.GetComponentInChildren<Health>().GetDamageTextParent.transform, false);

        float Critical = character.GetCriticalChance;

        if (Target != null)
        {
            #region CriticalHitCalculation
            if (Random.value * 100 <= Critical)
            {
                Target.GetComponentInChildren<Health>().GetTakingDamage = true;
                Target.GetComponentInChildren<Health>().ModifyHealth((-character.CharacterStrength - 5) - -Target.GetCharacter.CharacterDefense);

                DamageObject.GetComponentInChildren<Text>().fontSize = 30;

                DamageObject.GetComponentInChildren<Text>().text = ((character.CharacterStrength + 5) - Target.GetCharacter.CharacterDefense).ToString() + "!";
            }
            else
            {
                Target.GetComponentInChildren<Health>().GetTakingDamage = true;
                Target.GetComponentInChildren<Health>().ModifyHealth(-character.CharacterStrength - -Target.GetCharacter.CharacterDefense);

                DamageObject.GetComponentInChildren<Text>().fontSize = 20;

                DamageObject.GetComponentInChildren<Text>().text = (character.CharacterStrength - Target.GetCharacter.CharacterDefense).ToString();
            }
            #endregion

            if(Target.GetAI.GetStates != States.Skill)
            Target.GetAI.GetStates = States.Damaged;
        }
        return DamageObject.GetComponentInChildren<Text>();
    }
}