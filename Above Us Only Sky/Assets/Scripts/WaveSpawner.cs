using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<SpawnGroup> spawnGroups;
        public float timeTillNextWave = 20f;
        public bool SkipWaveOnEnemyDeath = false;
    }

    [System.Serializable]
    public class SpawnGroup
    {
        public EnemyType enemyType;
        public int count;
        public Transform spawnPoint;
        public float spawnInterval = 1f;
    }

    public enum EnemyType { Grunt, Mage, Brute, Sniper }

    public GameObject gruntPrefab;
    public GameObject magePrefab;
    public GameObject brutePrefab;
    public GameObject sniperPrefab;

    public List<Wave> waves;
    private int currentWaveIndex = 0;
    public float timeBetweenWaves = 5f;
    private float introTime = 22.85f;

    public delegate void WaveStarted(int waveNumber);
    public event WaveStarted OnWaveStarted;

    private bool spawning = false;
    private int activeEnemies = 0;
    private Coroutine waveRoutine;
    private bool skipWave = false;

    void Start()
    {
        waveRoutine = StartCoroutine(StartWaves());
    }

    IEnumerator StartWaves()
    {
        while (currentWaveIndex < waves.Count)
        {
            if (currentWaveIndex == 0)
            {
                yield return new WaitForSeconds(introTime - timeBetweenWaves);
            }
            yield return new WaitForSeconds(timeBetweenWaves);
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            float timeRemaining = waves[currentWaveIndex].timeTillNextWave;

            while (timeRemaining > 0f && !skipWave)
            {
                yield return new WaitForSeconds(1f);
                timeRemaining -= 1f;
            }

            skipWave = false;
            currentWaveIndex++;
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        OnWaveStarted?.Invoke(currentWaveIndex + 1);
        spawning = true;

        List<Coroutine> spawnCoroutines = new List<Coroutine>();
        foreach (var spawnGroup in wave.spawnGroups)
        {
            spawnCoroutines.Add(StartCoroutine(SpawnGroupIE(spawnGroup)));
        }

        foreach (var coroutine in spawnCoroutines)
        {
            yield return coroutine;
        }

        spawning = false;
    }

    IEnumerator SpawnGroupIE(SpawnGroup spawnGroup)
    {
        GameObject enemyPrefab = GetEnemyPrefab(spawnGroup.enemyType);
        for (int i = 0; i < spawnGroup.count; i++)
        {
            Instantiate(enemyPrefab, spawnGroup.spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnGroup.spawnInterval);
        }
    }

    public void EnemyDeath()
    {
        activeEnemies--;
        if (activeEnemies <= 0 && !spawning)
        {
            SkipToNextWave();
        }
    }

    public void SkipToNextWave()
    {
        if(currentWaveIndex >= waves.Count)
        {
            return;
        }

        skipWave = waves[currentWaveIndex].SkipWaveOnEnemyDeath;
    }

    private GameObject GetEnemyPrefab(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Grunt: return gruntPrefab;
            case EnemyType.Mage: return magePrefab;
            case EnemyType.Brute: return brutePrefab;
            case EnemyType.Sniper: return sniperPrefab;
            default: return null;
        }
    }
}
