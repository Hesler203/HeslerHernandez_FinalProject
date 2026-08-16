using System;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Serialization;

[AlchemySerialize]
public partial class AudioManager : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public AudioSource CurrentTrack { get; private set; }
    [field: SerializeField] public AudioSource StandbyTrack { get; private set; }
    [field: SerializeField] public AudioSource ChaseTrack { get; private set; }
    [field: SerializeField] public AudioSource WavesTrack { get; private set; }

    [Header("SFX")]
    [SerializeField] private AudioSource playerSFX;
    [SerializeField] private AudioSource seagullSFX;
    [SerializeField] private AudioSource environmentalSFX;
    [SerializeField] private AudioSource uiSFX;
    [AlchemySerializeField, NonSerialized]
    public Dictionary<string, SFXClip> SFXClips = new();
    public enum SFXType { player, seagull, environmental, ui }

    public void PlayMusic(AudioSource track)
    {
        track.Play();
        CurrentTrack = track;
    }

    public void StopCurrentTrack()
    {
        CurrentTrack.Stop();
    }

    public void StopAllAudio()
    {
        CurrentTrack.Stop();
        playerSFX.Stop();
        seagullSFX.Stop();
        environmentalSFX.Stop();
        uiSFX.Stop();
    }

    public void PlaySound(string soundName)
    {
        if (SFXClips.ContainsKey(soundName))
        {
            SFXClip sfx = SFXClips[soundName];
            switch (sfx.Type)
            {
                case SFXType.player:
                    playerSFX.PlayOneShot(sfx.Sound);
                    break;
                case SFXType.seagull:
                    seagullSFX.PlayOneShot(sfx.Sound);
                    break;
                case SFXType.environmental:
                    environmentalSFX.PlayOneShot(sfx.Sound);
                    break;
                case SFXType.ui:
                    uiSFX.PlayOneShot(sfx.Sound);
                    break;
            }
        }
    }

    public void StopSound(SFXType type)
    {
        switch (type)
        {
            case SFXType.player:
                playerSFX.Stop();
                break;
            case SFXType.seagull:
                seagullSFX.Stop();
                break;
            case SFXType.environmental:
                environmentalSFX.Stop();
                break;
            case SFXType.ui:
                uiSFX.Stop();
                break;
        }
    }
}

public class SFXClip
{
    public AudioClip Sound;
    public AudioManager.SFXType Type;
}