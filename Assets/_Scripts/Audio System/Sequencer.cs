using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public enum Note { C, Cs, D, Ds, E, F, Fs, G, Gs, A, As, B }
public enum Scale { Major, Minor, MajorPentatonic, MinorPentatonic, Blues, Chromatic}

[RequireComponent(typeof(CsoundUnity))]
public class Sequencer : MonoBehaviour
{
    [Header("Tempo")]
    [SerializeField] [Range(0, 1000)] private float _beatsPerMinute;
    
    [Header("Notes")]
    [SerializeField] [Range(330, 660)]private double _a4Frequency = 440.0;
    [SerializeField] private Note _rootNote;
    [SerializeField] private Scale _scale;
    [SerializeField] private Vector2Int _range;
    
    [Header("Mixer")]
    [SerializeField] private AudioMixerGroup _mixerGroup;
    
    private CsoundUnity _csound;
    private int _currentBeat;

    private const int A4Degree = 57;

    private void Start()
    {
        _csound = GetComponent<CsoundUnity>();
        GetComponent<AudioSource>().outputAudioMixerGroup = _mixerGroup;
    }

    private void Update()
    {
        _csound.SetChannel("tempo", _beatsPerMinute / 60f);
        _csound.SetChannel("pitch", GetFrequency(GetRandomNoteInScale(_rootNote, _scale), Random.Range(_range.x, _range.y + 1)));
    }

    private static Note GetRandomNoteInScale(Note rootNote, Scale scale)
    {
        List<Note> notesInKey = new() { rootNote };

        List<int> noteDegrees = scale switch
        {
            Scale.Major => new() { 2, 4, 5, 7, 9, 11 },
            Scale.Minor => new() { 2, 3, 5, 7, 8, 10 },
            Scale.MajorPentatonic => new() { 2, 4, 7, 9 },
            Scale.MinorPentatonic => new() { 3, 5, 7, 10 },
            Scale.Blues => new() { 3, 5, 6, 7, 10 },
            Scale.Chromatic => new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 },
            _ => throw new ArgumentOutOfRangeException(nameof(scale), scale, null)
        };

        foreach (int noteDegree in noteDegrees)
        {
            notesInKey.Add((Note)(((int)rootNote + noteDegree) % 12));
        }
        
        return notesInKey.GetRandomElement();
    }

    private double GetFrequency(Note note, int octave)
    {
        return GetFrequency(octave * 12 + (int)note);
    }

    private double GetFrequency(int noteDegree)
    {
        return Math.Round(_a4Frequency * Math.Pow(2.0, (noteDegree - A4Degree) / 12.0), 2);
    }
}