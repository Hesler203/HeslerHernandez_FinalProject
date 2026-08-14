using System;
using System.Collections.Generic;
using Alchemy.Serialization;
using UnityEngine;

[AlchemySerialize]
public partial class SandcastleHealth : MonoBehaviour
{
    private GameManager gameManager;

    [AlchemySerializeField, NonSerialized]
    public Dictionary<Health, GameObject> HealthIndicators = new();
    public enum Health { none, low, med, full };
    private Health health;

    void Start()
    {
        gameManager = GameManager.Instance;

        SetHealth();
    }

    private void SetHealth(Health newHealth = Health.full)
    {
        foreach (Health key in HealthIndicators.Keys)
        {
            if (newHealth == key)
            {
                HealthIndicators[newHealth].SetActive(true);
                continue;
            }
            HealthIndicators[key].SetActive(false);
        }
        health = newHealth;
    }

    private void TakeDamage()
    {
        if (health > Health.none)
        {
            SetHealth(--health);
        }

        if (health == Health.none)
        {
            gameManager.Lose();
        }
    }
}
