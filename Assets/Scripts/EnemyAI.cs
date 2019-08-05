﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum States { Patrol, Chase, Attack, ApplyingAttack, Skill, SkillAnimation, Damaged, Immobile }

[System.Serializable]
public class AiStates
{
    [SerializeField]
    private States state;

    [SerializeField]
    private int SkillIndex;

    public States GetState
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
        }
    }

    public int GetSkillIndex
    {
        get
        {
            return SkillIndex;
        }
        set
        {
            SkillIndex = value;
        }
    }
}

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private States states;

    [SerializeField]
    private AiStates[] aiStates;

    [SerializeField]
    private Character character;

    [SerializeField]
    private Enemy enemy;

    [SerializeField]
    private EnemySkills enemySkills;

    [SerializeField]
    private EnemyAnimations Anim;

    [SerializeField]
    private Transform[] Waypoints;

    [SerializeField]
    private float MoveSpeed, AttackRange, AttackDelay, AutoAttackTime, LookSpeed;

    [SerializeField] [Tooltip("Current targeted Player. Keep this empty!")]
    private Character PlayerTarget = null;

    [SerializeField]
    private SphereCollider EnemyTriggerSphere;

    [SerializeField]
    private ParticleSystem HitParticle;

    private bool ParticleExists, DamageTextExists;

    [SerializeField] 
    private float TimeToMoveAgain; //A value that determines how long the enemy will stay at one waypoint before moving on to the next.

    private float TimeToMove, DistanceToTarget;

    private bool StandingStill, PlayerEntry;

    [SerializeField]
    private Image ThreatPic;

    [SerializeField]
    private Sprite DocileSprite, ThreatSprite;

    private int WaypointIndex;

    [SerializeField]
    private bool IsHostile;

    private int StateArrayIndex;

    public int GetStateArrayIndex
    {
        get
        {
            return StateArrayIndex;
        }
        set
        {
            StateArrayIndex = value;
        }
    }

    public EnemyAnimations GetAnimation
    {
        get
        {
            return Anim;
        }
        set
        {
            Anim = value;
        }
    }

    public AiStates[] GetAiStates
    {
        get
        {
            return aiStates;
        }
        set
        {
            aiStates = value;
        }
    }

    public void IncreaseArray()
    {
        StateArrayIndex++;
        if(StateArrayIndex >= aiStates.Length)
        {
            StateArrayIndex = 0;
        }
    }

    private void Awake()
    {
        states = States.Patrol;

        TimeToMove = TimeToMoveAgain;
    }

    private void OnEnable()
    {
        ResetStats();
    }

    private void Update()
    {
        if (this.character.CurrentHealth > 0)
        {
            switch (states)
            {
                case (States.Patrol):
                    Patrol();
                    break;
                case (States.Chase):
                    Chase();
                    break;
                case (States.Attack):
                    Attack();
                    break;
                case (States.ApplyingAttack):
                    ApplyingNormalAtk();
                    break;
                case (States.Skill):
                    Skill();
                    break;
                case (States.Damaged):
                    Damage();
                    break;
                case (States.Immobile):
                    Immobile();
                    break;
                case (States.SkillAnimation):
                    break;
            }
        }
    }

    public States GetStates
    {
        get
        {
            return states;
        }
        set
        {
            states = value;
        }
    }

    public float GetAutoAttack
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

    public bool GetIsHostile
    {
        get
        {
            return IsHostile;
        }
        set
        {
            IsHostile = value;
        }
    }

    public Character GetPlayerTarget
    {
        get
        {
            return PlayerTarget;
        }
        set
        {
            PlayerTarget = value;
        }
    }

    public SphereCollider GetSphereTrigger
    {
        get
        {
            return EnemyTriggerSphere;
        }
        set
        {
            EnemyTriggerSphere = value;
        }
    }

    private void Patrol()
    {
        float DistanceToWayPoint = Vector3.Distance(new Vector3(this.transform.position.x, 0, this.transform.position.z),
                                                    new Vector3(Waypoints[WaypointIndex].position.x, 0, Waypoints[WaypointIndex].position.z));

        if (!StandingStill)
        {
            Anim.RunAni();

            Vector3 Distance = new Vector3(Waypoints[WaypointIndex].position.x - this.transform.position.x, 0,
                                           Waypoints[WaypointIndex].position.z - this.transform.position.z).normalized;

            Quaternion LookDir = Quaternion.LookRotation(Distance);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, LookDir, LookSpeed * Time.deltaTime);

            this.transform.position += Distance * MoveSpeed * Time.deltaTime;
        }
        else
        {
            Anim.IdleAni();
        }

        if (DistanceToWayPoint <= 0.1f)
        {
            StandingStill = true;
            TimeToMove -= Time.deltaTime;
            if(TimeToMove <= 0)
            {
                WaypointIndex++;
                if (WaypointIndex >= Waypoints.Length)
                {
                    WaypointIndex = 0;
                }
                TimeToMove = TimeToMoveAgain;
                StandingStill = false;
            }
        }
    }

    private void Chase()
    {
        StandingStill = false;

        Anim.RunAni();

        enemySkills.GetSkillBar.gameObject.SetActive(false);

        if (PlayerTarget != null)
        {
            if(PlayerTarget != null)
            {
                DistanceToTarget = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);
            }

            if (DistanceToTarget >= AttackRange)
            {
                Vector3 Distance = new Vector3(PlayerTarget.transform.position.x - this.transform.position.x, 0,
                                               PlayerTarget.transform.position.z - this.transform.position.z).normalized;

                Quaternion LookDir = Quaternion.LookRotation(Distance);

                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, LookDir, LookSpeed * Time.deltaTime);

                this.transform.position += Distance * MoveSpeed * Time.deltaTime;
            }
            else
            {
                states = States.Attack;
            }
        }
        else
        {
            states = States.Patrol;
        }
    }

    private void Attack()
    {
        Anim.IdleAni();

        if(PlayerTarget != null)
        {
            DistanceToTarget = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);
        }

        if (PlayerTarget != null && DistanceToTarget <= AttackRange)
        {
            Vector3 Distance = new Vector3(PlayerTarget.transform.position.x - this.transform.position.x, 0,
                                           PlayerTarget.transform.position.z - this.transform.position.z).normalized;

            Quaternion LookDir = Quaternion.LookRotation(Distance);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, LookDir, LookSpeed * Time.deltaTime);

            if (PlayerTarget.CurrentHealth > 0)
            { 
                AutoAttackTime += Time.deltaTime;
                if (AutoAttackTime >= AttackDelay)
                {
                    states = aiStates[StateArrayIndex].GetState;
                }
            }
            else
            {
                enemy.GetHealth.IncreaseHealth(character.MaxHealth);
                enemy.GetLocalHealthInfo();

                PlayerTarget = null;
                AutoAttackTime = 0;
                states = States.Patrol;
            }
        }
        else
        {
            AutoAttackTime = 0;
            states = States.Chase;
        }
    }

    private void ApplyingNormalAtk()
    {
        Anim.AttackAni();
    }

    private void Skill()
    {
        enemySkills.ChooseSkill(aiStates[StateArrayIndex].GetSkillIndex);
    }

    //Sets the enemy to this state if they are inflicted with the stun/sleep status effect.
    private void Immobile()
    {
        Anim.IdleAni();
    }

    public void Damage()
    {
        Anim.DamageAni();
    }

    public void Dead()
    {
        StandingStill = false;
        PlayerTarget = null;
        AutoAttackTime = 0;
        EnemyTriggerSphere.enabled = false;

        this.gameObject.GetComponent<BoxCollider>().enabled = false;

        enemy.GetLocalHealth.gameObject.SetActive(false);

        enemySkills.GetSkillBar.gameObject.SetActive(false);
        enemySkills.GetActiveSkill = false;

        character.GetRigidbody.useGravity = false;

        if(enemySkills.GetManager.Length > 0)
        {
            enemySkills.DisableRadiusImage();
            enemySkills.DisableRadius();
        }

        GameManager.Instance.GetEventSystem.SetSelectedGameObject(null);
        GameManager.Instance.GetEnemyObject = null;
        GameManager.Instance.GetLastEnemyObject = null;

        enemy.ToggleHealthBar();

        enemy.ReturnExperience();

        Anim.DeathAni();
    }

    //Resets the enemy's stats when enabled in the scene.
    private void ResetStats()
    {
        if (IsHostile)
        {
            ThreatPic.sprite = ThreatSprite;
            EnemyTriggerSphere.enabled = true;
        }
        else
        {
            ThreatPic.sprite = DocileSprite;
            EnemyTriggerSphere.enabled = false;
        }

        character.CurrentHealth = character.MaxHealth;
        enemy.GetFilledBar();
        enemy.GetLocalHealth.gameObject.SetActive(true);
        enemy.GetLocalHealthInfo();

        this.gameObject.GetComponent<BoxCollider>().enabled = true;
        character.GetRigidbody.useGravity = true;

        states = States.Patrol;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerEntry = true;
        if(other.gameObject.GetComponent<PlayerController>())
        {
            PlayerTarget = other.GetComponent<Character>();
            states = States.Chase;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() && IsHostile)
        {
            PlayerEntry = false;
            if(states != States.SkillAnimation)
            {
                PlayerTarget = null;
                states = States.Patrol;
                AutoAttackTime = 0;
                if(enemySkills.GetManager.Length > 0)
                {
                    enemySkills.DisableRadiusImage();
                    enemySkills.DisableRadius();
                }
                enemySkills.GetActiveSkill = false;
                enemySkills.GetSkillBar.gameObject.SetActive(false);
            }
            enemy.GetHealth.IncreaseHealth(character.MaxHealth);
            enemy.GetLocalHealthInfo();
            StateArrayIndex = 0;
        }
        if (other.gameObject.GetComponent<PlayerController>() && !IsHostile)
        {
            PlayerEntry = false;
            if(states != States.SkillAnimation)
            {
                PlayerTarget = null;
                states = States.Patrol;
                AutoAttackTime = 0;
                EnemyTriggerSphere.gameObject.SetActive(false);
                if (enemySkills.GetManager.Length > 0)
                {
                    enemySkills.DisableRadiusImage();
                    enemySkills.DisableRadius();
                }
                enemySkills.GetActiveSkill = false;
                enemySkills.GetSkillBar.gameObject.SetActive(false);
            }
            enemy.GetHealth.IncreaseHealth(character.MaxHealth);
            enemy.GetLocalHealthInfo();
            StateArrayIndex = 0;
        }
    }

    public void CheckTarget()
    {
        if(!PlayerEntry)
        {
            PlayerTarget = null;
            states = States.Patrol;
            AutoAttackTime = 0;
            enemySkills.DisableRadiusImage();
            enemySkills.DisableRadius();
            enemySkills.GetActiveSkill = false;
            enemySkills.GetSkillBar.gameObject.SetActive(false);
        }
        else
        {
            AutoAttackTime = 0;
            states = States.Attack;
        }
    }

    public TextMeshProUGUI TakeDamage()
    {
        if(PlayerTarget == null)
        {
            return null;
        }

        if(!ParticleExists)
        {
            CreateParticle();
        }
        else
        {
            HitParticle.gameObject.SetActive(true);
        }

        float Critical = character.GetCriticalChance;

        var t = ObjectPooler.Instance.GetPlayerDamageText();

        if (PlayerTarget != null)
        {
            t.gameObject.SetActive(true);

            t.transform.SetParent(PlayerTarget.GetComponent<Health>().GetDamageTextParent.transform, false);

            #region CriticalHitCalculation
            if (Random.value * 100 <= Critical)
            {
                PlayerTarget.GetComponent<Health>().ModifyHealth(-((character.CharacterStrength + 5) - PlayerTarget.CharacterDefense));

                t.GetComponentInChildren<TextMeshProUGUI>().text = "<size=35>" + ((character.CharacterStrength + 5) - PlayerTarget.CharacterDefense).ToString() + "!";
            }
            else
            {
                PlayerTarget.GetComponent<Health>().ModifyHealth(-(character.CharacterStrength - PlayerTarget.CharacterDefense));

                t.GetComponentInChildren<TextMeshProUGUI>().text = "<size=25>" + (character.CharacterStrength - PlayerTarget.CharacterDefense).ToString();
            }
            #endregion

            PlayerTarget.GetComponent<PlayerAnimations>().DamagedAnimation();
        }
        return t.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void CreateParticle()
    {
        var Hitparticle = ObjectPooler.Instance.GetHitParticle();

        Hitparticle.SetActive(true);

        Hitparticle.transform.position = new Vector3(PlayerTarget.transform.position.x, PlayerTarget.transform.position.y + 0.6f, PlayerTarget.transform.position.z);

        Hitparticle.transform.SetParent(PlayerTarget.transform, true);
    }
}