using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class AudioData
{
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
    public bool loop;
    public bool playOnAwake;
}