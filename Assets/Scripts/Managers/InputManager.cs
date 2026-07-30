using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager _InputManager { get; private set; }
    [SerializeField] public InputActionReference Inputs;

    void Awake()
    {
        if (_InputManager && _InputManager != this)
        {
            Destroy(_InputManager);
        }
        _InputManager = this;
    }


}
