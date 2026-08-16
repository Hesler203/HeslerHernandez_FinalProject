using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public AnimatorManager AnimatorManager { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public SeagullController SeagullController { get; private set; }
    public SandcastleHealth SandcastleHealth { get; private set; }
    public CameraController CameraController { get; private set; }

    [Header("Wave Settings")]
    [SerializeField] private int WaveDamage;
    [SerializeField] private float WaveCooldown;

    [Header("Game Over")]
    [SerializeField] private Image loseImage;
    [SerializeField] private TextMeshProUGUI loseText;

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
            SandcastleHealth = FindAnyObjectByType<SandcastleHealth>();
            CameraController = FindAnyObjectByType<CameraController>();
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    void Start()
    {
        AudioManager.PlayMusic(AudioManager.StandbyTrack);

        loseImage.enabled = false;
        loseText.enabled = false;
    }

    void LateUpdate()
    {
        SetMusicTrackToSeagullState();

        StartCoroutine(nameof(WaveCrashAndCooldownTimer));
    }

    private void SetMusicTrackToSeagullState()
    {
        if (SeagullController.currentState == SeagullController.SeagullState.standby && AudioManager.CurrentTrack != AudioManager.StandbyTrack)
        {
            AudioManager.StopCurrentTrack();
            AudioManager.PlayMusic(AudioManager.StandbyTrack);
        }
        else if (SeagullController.currentState == SeagullController.SeagullState.chasing && AudioManager.CurrentTrack != AudioManager.ChaseTrack && !AnimatorManager.SeagullAnimator.GetBool(AnimatorManager.IsClimbingHash))
        {
            AudioManager.StopCurrentTrack();
            AudioManager.PlayMusic(AudioManager.ChaseTrack);
        }
    }

    public IEnumerator Lose()
    {
        yield return new WaitForSeconds((float)CameraController.VideoPlayer.length / 2);

        AudioManager.StopAllAudio();
        AudioManager.PlayMusic(AudioManager.WavesTrack);

        PlayerController.enabled = false;
        SeagullController.enabled = false;
        SandcastleHealth.enabled = false;
        
        loseImage.enabled = true;
        loseText.enabled = true;

        Debug.Log("You've failed to stay above the waves...");
        yield return new WaitWhile(() => AudioManager.CurrentTrack.isPlaying);
        Debug.Break(); // TODO lose screen
    }

    IEnumerator WaveCrashAndCooldownTimer()
    {
        yield return new WaitForSeconds(WaveCooldown);
        CameraController.PlayVideo();

        yield return new WaitForSeconds((float)CameraController.VideoPlayer.length / 2);
        SandcastleHealth.WaveHit(WaveDamage);

        StopCoroutine(nameof(WaveCrashAndCooldownTimer));
    }
}