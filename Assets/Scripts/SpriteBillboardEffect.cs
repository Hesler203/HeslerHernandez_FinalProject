using UnityEngine;

public class SpriteBillboardEffect : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    void LateUpdate()
    {
        transform.rotation = cameraTransform.rotation;
    }
}
