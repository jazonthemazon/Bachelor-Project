using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public enum Note { C, Cs, D, Ds, E, F, Fs, G, Gs, A, As, B }

[RequireComponent(typeof(CsoundUnity))]
public class Sequencer : MonoBehaviour
{
    [SerializeField] [Range(0, 1000)] private float _beatsPerMinute;
    [SerializeField] private AudioMixerGroup _mixerGroup;
    [SerializeField] [Range(400, 500)]private double _a4Frequency = 440.0;

    private CsoundUnity _csound;
    private int _currentBeat;

    private const int A4Degree = 4 * 12 + 9;

    private void Start()
    {
        _csound = GetComponent<CsoundUnity>();
        GetComponent<AudioSource>().outputAudioMixerGroup = _mixerGroup;
    }

    private void Update()
    {
        _csound.SetChannel("tempo", _beatsPerMinute / 60f);
        _csound.SetChannel("pitch", GetFrequency((Note)((int)Time.time % 12), 4));
    }

    private double GetFrequency(Note note, int octave)
    {
        return GetFrequency( octave * 12 + (int)note);
    }

    private double GetFrequency(int noteDegree)
    {
        int noteDifference = noteDegree - A4Degree;
        return Math.Round(_a4Frequency * Math.Pow(2.0, noteDifference / 12.0), 2);
    }
}