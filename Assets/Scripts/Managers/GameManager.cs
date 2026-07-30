using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _GameManager { get; private set; }

    void Awake()
    {
        if (_GameManager && _GameManager != this)
        {
            Destroy(_GameManager);
        }
        _GameManager = this;
    }
}
