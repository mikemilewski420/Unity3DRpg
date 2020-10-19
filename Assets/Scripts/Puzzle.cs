﻿#pragma warning disable 0649, 0414
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [SerializeField]
    private CharacterData[] characterData;

    [SerializeField]
    private Enemy enemyToSpawn = null;

    [SerializeField]
    private GameObject ParticleEffect;

    [SerializeField]
    private GameObject ObjectToSpawn = null, ObjectToDespawn = null;

    [SerializeField]
    private BoxCollider BridgePathCollider = null;

    [SerializeField]
    private Transform EnemySpawnPoint = null;

    [SerializeField]
    private Settings settings;

    [SerializeField]
    private int MaxEnemyCountRequired, SpawnIndex, NumberOfEnemiesToSpawn;

    private int EnemyCountRequired;

    public int GetEnemyCount
    {
        get
        {
            return EnemyCountRequired;
        }
        set
        {
            EnemyCountRequired = value;
        }
    }

    public void EnemyPuzzleType()
    {
        EnemyCountRequired++;

        if (EnemyCountRequired >= MaxEnemyCountRequired)
        {
            if(settings.UseParticleEffects)
            {
                ParticleEffect.SetActive(true);
            }
            ObjectToSpawn.SetActive(true);
            BridgePathCollider.enabled = false;
            gameObject.SetActive(false);
        }
        else return;
    }

    public void BushPuzzle()
    {
        EnemyCountRequired++;

        if (EnemyCountRequired >= MaxEnemyCountRequired)
        {
            if (settings.UseParticleEffects)
            {
                ParticleEffect.SetActive(true);
            }
            ObjectToDespawn.SetActive(false);
            gameObject.SetActive(false);
        }
        else return;
    }

    public void SpawnNewEnemy()
    {
        SpawnIndex++;

        Invoke("ShouldSpawn", 5f);
    }

    private void ShouldSpawn()
    {
        if(SpawnIndex < NumberOfEnemiesToSpawn)
        {
            enemyToSpawn.transform.position = EnemySpawnPoint.position;
            enemyToSpawn.GetCharacter.GetCharacterData = characterData[SpawnIndex];
            characterData[SpawnIndex].CheckedData = false;
            enemyToSpawn.GetComponent<Enemy>().GetExperiencePoints = characterData[SpawnIndex].EXP;
            enemyToSpawn.GetComponent<Enemy>().GetCoins = characterData[SpawnIndex].Coins;
            EnemySpawnParticle();
            enemyToSpawn.gameObject.SetActive(true);
        }
        else
        {
            ParticleEffect.SetActive(true);
            ObjectToSpawn.SetActive(true);
        }
    }

    private void EnemySpawnParticle()
    {
        if (settings.UseParticleEffects)
        {
            var SpawnParticle = ObjectPooler.Instance.GetEnemyAppearParticle();

            SpawnParticle.SetActive(true);

            SpawnParticle.transform.position = new Vector3(EnemySpawnPoint.position.x, EnemySpawnPoint.position.y, EnemySpawnPoint.position.z);
        }
    }
}
