using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AnimatorManager AnimatorManager { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public SeagullController SeagullController { get; private set; }

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetReferences();
    }

    private void SetReferences()
    {
        try
        {
            AnimatorManager = FindAnyObjectByType<AnimatorManager>();
            AudioManager = FindAnyObjectByType<AudioManager>();

            PlayerController = FindAnyObjectByType<PlayerController>();
            SeagullController = FindAnyObjectByType<SeagullController>();
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    void Start()
    {
        AudioManager.PlayMusic(AudioManager.StandbyTrack);
    }

    void LateUpdate()
    {
        SetMusicTrackToSeagullState();
    }

    private void SetMusicTrackToSeagullState()
    {
        if (SeagullController.currentState == SeagullController.SeagullState.standby && AudioManager.CurrentTrackName != AudioManager.StandbyTrack.name)
        {
            AudioManager.PlayMusic(AudioManager.StandbyTrack);
        }
        else if (SeagullController.currentState == SeagullController.SeagullState.chasing && AudioManager.CurrentTrackName != AudioManager.ChaseTrack.name)
        {
            AudioManager.PlayMusic(AudioManager.ChaseTrack);
        }
    }

    public void Lose()
    {
        Debug.Break(); // TODO
    }
}
