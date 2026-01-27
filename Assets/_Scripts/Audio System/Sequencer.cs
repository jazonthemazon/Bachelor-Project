using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Note { C, Cs, D, Ds, E, F, Fs, G, Gs, A, As, B }

public enum Scale
{
    Ionian,
    Dorian,
    Phrygian,
    Lydian,
    Mixolydian,
    Aeolian,
    Locrian,
    MajorPentatonic,
    MinorPentatonic,
    Blues,
    Chromatic,
    WholeTone,
    OneNote,
    PowerChord
}

public enum Speed
{
    SixteenthNotes,
    EighthNotes,
    QuarterNotes,
    HalfNotes,
    WholeNotes
}

[RequireComponent(typeof(CsoundUnity))]
public class Sequencer : Singleton<Sequencer>
{
    [Header("Global Tempo")]
    [SerializeField] [Range(0, 1000)] private float _beatsPerMinute;
    
    [Header("Global Notes")]
    [SerializeField] [Range(330, 660)] private double _a4Frequency = 440.0;
    [SerializeField] private Note _rootNote;
    [SerializeField] private Scale _scale;
    
    [Header("Tuned Instruments")]
    [SerializeField] private List<TunedInstrument> _tunedInstruments;
    
    private CsoundUnity _csound;
    private int _currentBeat;

    private const int A4Degree = 57;

    private void Start()
    {
        _csound = GetComponent<CsoundUnity>();
    }

    private void Update()
    {
        // update global parameters
        _csound.SetChannel("tempo", (_beatsPerMinute / 60f) * 4);

        if (_tunedInstruments.Count > 8)
        {
            Debug.LogError("Too many tuned instruments! Maximum is 8.");
            return;
        }
        
        // update parameters per instrument
        for (var i = 0; i < _tunedInstruments.Count; i++)
        {
            TunedInstrument tunedInstrument = _tunedInstruments[i];
            
            _csound.SetChannel($"active{i}", tunedInstrument.Active ? 1 : 0);
            if (!tunedInstrument.Active) continue;
            
            _csound.SetChannel($"prob{i}", tunedInstrument.Probability);
            
            _csound.SetChannel($"instrument{i}", (int)tunedInstrument.InstrumentType + 2);

            int speedDivider = tunedInstrument.Speed switch
            {
                Speed.SixteenthNotes => 1,
                Speed.EighthNotes => 2,
                Speed.QuarterNotes => 4,
                Speed.HalfNotes => 8,
                Speed.WholeNotes => 16,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _csound.SetChannel($"speed{i}", speedDivider);

            double frequency = GetFrequency(GetRandomNoteInScale(_rootNote, _scale), Random.Range(tunedInstrument.Range.x, tunedInstrument.Range.y + 1));
            _csound.SetChannel($"pitch{i}",  frequency);
            
            _csound.SetChannel($"volume{i}",  tunedInstrument.Volume);
        }
    }

    private static Note GetRandomNoteInScale(Note rootNote, Scale scale)
    {
        List<Note> notesInKey = new() { rootNote };

        List<int> noteDegrees = scale switch
        {
            Scale.Ionian => new() { 2, 4, 5, 7, 9, 11 },
            Scale.Dorian => new() { 2, 3, 5, 7, 9, 10 },
            Scale.Phrygian => new() { 1, 3, 5, 7, 8, 10 },
            Scale.Lydian => new() { 2, 4, 6, 7, 9, 11 },
            Scale.Mixolydian => new() { 2, 4, 5, 7, 9, 10 },
            Scale.Aeolian => new() { 2, 3, 5, 7, 8, 10 },
            Scale.Locrian => new() { 1, 3, 5, 6, 8, 10 },
            Scale.MajorPentatonic => new() { 2, 4, 7, 9 },
            Scale.MinorPentatonic => new() { 3, 5, 7, 10 },
            Scale.Blues => new() { 3, 5, 6, 7, 10 },
            Scale.Chromatic => new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 },
            Scale.WholeTone => new() { 2, 4, 6, 8, 10 },
            Scale.OneNote => new(),
            Scale.PowerChord => new() { 7 },
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

    public void SetTempo(float bpm, float changeDuration)
    {
        _beatsPerMinute = bpm;
    }
}