using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AnimatorManager AnimatorManager { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public GameObject Player { get; private set; }
    public GameObject Seagull { get; private set; }

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;

        try
        {
            AnimatorManager = FindAnyObjectByType<AnimatorManager>();
            AudioManager = FindAnyObjectByType<AudioManager>();

            Player = FindAnyObjectByType<PlayerController>().gameObject;
            Seagull = FindAnyObjectByType<SeagullController>().gameObject;
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError(ex.Message);
            return;
        }
    }

    public void Lose()
    {
        Debug.Break(); // TODO
    }
}
