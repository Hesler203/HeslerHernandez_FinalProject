using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("InputActionReferences")]
    [SerializeField] private InputActionReference playerMove;
    [SerializeField] private InputActionReference playerRoll;
    // UI input action ref TODO

    [field: Header("Settings")]
    [field: SerializeField] public float MoveDeadzone { get; private set; }
    public Vector2 PlayerMoveInput { get; private set; }
    public bool PlayerRollInput { get; private set; }

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        playerMove.action.Enable();
        playerRoll.action.Enable();
    }

    void OnDisable()
    {
        playerMove.action.Disable();
        playerRoll.action.Disable();
    }

    void Update()
    {
        PlayerMoveInput = playerMove.action.ReadValue<Vector2>();
        PlayerRollInput = playerRoll.action.IsPressed();
    }
}