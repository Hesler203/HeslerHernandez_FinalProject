using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class CameraController : MonoBehaviour
{
    public VideoPlayer VideoPlayer { get; private set; }
    public bool play = false;
    void Start()
    {
        VideoPlayer = GetComponent<VideoPlayer>();
    }

    void Update()
    {
        if (play)
        {
            StartCoroutine(nameof(PlayWaveClip));
        }
    }

    IEnumerator PlayWaveClip()
    {
        VideoPlayer.Play();
        yield return new WaitForSeconds((float)VideoPlayer.length);

        VideoPlayer.Stop();
        play = false;
        StopCoroutine(nameof(PlayWaveClip));
    }
}
