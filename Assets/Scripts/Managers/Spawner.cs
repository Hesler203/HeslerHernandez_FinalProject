using System.Collections.Generic;
using Alchemy.Serialization;
using UnityEngine;

[AlchemySerialize]
abstract public partial class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected Transform[] spawnLocations;
    protected Dictionary<SpawnType, GameObject> prefabs = new();
    protected float spawnRate;
    protected enum SpawnType { }

    abstract protected GameObject RandomizePrefabToSpawn();

    protected Vector3 RandomizeSpawnLocation()
    {
        return spawnLocations[Random.Range(0, spawnLocations.Length)].position;
    }

    abstract protected void SpawnRandomPrefab();
}