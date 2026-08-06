using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("InputActionReferences")]
    [SerializeField] private InputActionReference PlayerMove;
    // UI input action ref TODO

    [field: Header("Settings")]
    [field: SerializeField] public float MoveDeadzone { get; private set; }
    public Vector2 PlayerMoveInput { get; private set; }

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
        PlayerMove.action.Enable();
    }

    void OnDisable()
    {
        PlayerMove.action.Disable();
    }

    void Update()
    {
        PlayerMoveInput = PlayerMove.action.ReadValue<Vector2>();
    }
}