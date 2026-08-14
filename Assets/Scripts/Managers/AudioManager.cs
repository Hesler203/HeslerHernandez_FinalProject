using System;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Serialization;

[AlchemySerialize]
public partial class AudioManager : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public string CurrentTrackName { get; private set; }
    [field: SerializeField] public AudioSource StandbyTrack { get; private set; }
    [field: SerializeField] public AudioSource ChaseTrack { get; private set; }

    [Header("SFX")]
    [SerializeField] private AudioSource sFX;
    [AlchemySerializeField, NonSerialized]
    public Dictionary<string, AudioClip> SFXClips = new();

    public void PlayMusic(AudioSource track)
    {
        track.Play();
        CurrentTrackName = track.name;
    }

    public void PlaySound(AudioClip sound)
    {
        sFX.PlayOneShot(sound);
    }
}
