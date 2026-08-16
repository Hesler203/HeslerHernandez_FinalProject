using System.Collections.Generic;
using Alchemy.Serialization;
using UnityEngine;

[AlchemySerialize]
public partial class ObstacleSpawner : Spawner
{
    [Header("Settings")]
    [AlchemySerializeField, System.NonSerialized]
    new public Dictionary<SpawnType, GameObject> prefabs = new();
    [SerializeField] new private float spawnRate = 3f;
    new public enum SpawnType { urchin, puddle }

    void Start()
    {
        InvokeRepeating(nameof(SpawnRandomPrefab), 0, spawnRate);
    }

    override protected GameObject RandomizePrefabToSpawn()
    {
        int randomType = Random.Range((int)SpawnType.urchin, (int)SpawnType.puddle);
        return prefabs[(SpawnType)randomType];
    }

    override protected void SpawnRandomPrefab()
    {
        GameObject randomPrefab = RandomizePrefabToSpawn();
        Vector3 randomLocation = RandomizeSpawnLocation();
        Instantiate(randomPrefab, randomLocation, randomPrefab.transform.rotation, transform);
    }
}