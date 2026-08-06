using System;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Serialization;

[AlchemySerialize]
public partial class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioSource music;

    [Header("SFX")]
    [SerializeField] private AudioSource sFX;

    [AlchemySerializeField, NonSerialized]
    public Dictionary<string, AudioClip> SFXClips = new();

    public void PlaySound(AudioClip sound)
    {
        sFX.PlayOneShot(sound);
    }
}
