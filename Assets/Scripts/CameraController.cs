using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Transform navAgentTransform;
    [SerializeField] private Transform playerTransform;
    public VideoPlayer VideoPlayer { get; private set; }

    void Start()
    {
        VideoPlayer = GetComponent<VideoPlayer>();
        initialPosition = transform.position;
    }

    void Update()
    {
        transform.position = initialPosition;
        
        if (PlayerController.CurrentState == PlayerController.PlayerState.caught)
        {
            transform.position = new Vector3(transform.position.x, navAgentTransform.position.y, transform.position.z);
        }
        else if (PlayerController.CurrentState == PlayerController.PlayerState.falling)
        {
            transform.position = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z);
        }
    }

    public void PlayVideo()
    {
        StartCoroutine(nameof(RunVideoClip));
    }

    IEnumerator RunVideoClip()
    {
        VideoPlayer.Play();
        yield return new WaitForSeconds((float)VideoPlayer.length);

        VideoPlayer.Stop();
        StopCoroutine(nameof(RunVideoClip));
    }
}
