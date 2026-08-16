using System;
using System.Collections.Generic;
using Alchemy.Serialization;
using TMPro;
using UnityEngine;

[AlchemySerialize]
public partial class SandcastleHealth : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI scoreValue;


    [Header("Stats")]
    // TODO list of trinket objects currently on the sandcastle
    [AlchemySerializeField, NonSerialized]
    public Dictionary<Health, GameObject> HealthIndicators = new();
    public enum Health { none, low, med, full };
    private Health health;
    [SerializeField] private int defense;

    void Start()
    {
        gameManager = GameManager.Instance;

        SetHealth();
        ClearDefenses();
    }

    private void UpdateScoreUI()
    {
        scoreValue.text = defense.ToString();
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

    private void ClearDefenses()
    {
        defense = 0;
        UpdateScoreUI();
        // TODO clear trinkets list
    }

    public void IncreaseDefense()
    {
        defense++;
        UpdateScoreUI();
    }

    public void WaveHit(int damage)
    {
        if (CanResistDamage(damage))
        {
            defense -= damage;
            UpdateScoreUI();
            // TODO remove few trinkets from list
            return;
        }
        DecreaseHealth();
        ClearDefenses();
    }

    private bool CanResistDamage(int damage)
    {
        if (defense > damage)
        {
            return true;
        }
        return false;
    }

    private void DecreaseHealth()
    {
        if (health > Health.none)
        {
            SetHealth(--health);
        }

        if (health == Health.none)
        {
            gameManager.StartCoroutine(nameof(gameManager.Lose));
        }
    }
}