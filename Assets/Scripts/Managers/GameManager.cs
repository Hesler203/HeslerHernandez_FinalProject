using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AnimatorManager _AnimatorManager { get; private set; }
    public AudioManager _AudioManager { get; private set; }

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    void Start()
    {
        _AnimatorManager = GetComponentInChildren<AnimatorManager>();
        _AudioManager = GetComponentInChildren<AudioManager>();
    }
}
