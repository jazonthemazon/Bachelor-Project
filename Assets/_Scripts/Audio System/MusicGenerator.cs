using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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
    PowerChord,
    HarmonicMinor,
    MelodicMinor
}

public enum Speed
{
    SixteenthNotes,
    EighthNotes,
    DottedEighthNotes,
    QuarterNotes,
    DottedQuarterNotes,
    HalfNotes,
    DottedHalfNotes,
    WholeNotes,
    DottedWholeNotes
}

[RequireComponent(typeof(CsoundUnity))]
public class MusicGenerator : Singleton<MusicGenerator>
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer _mixer;
    
    [Header("Global Volume")]
    [SerializeField] private bool _globalMute;
    [SerializeField] [Range(0, 1)] private float _globalVolume;
    
    [Header("Global Tempo")]
    [SerializeField] [Range(0, 1000)] private float _beatsPerMinute;
    
    [Header("Global Notes")]
    [SerializeField] [Range(330, 660)] private float _a4Frequency = 440f;
    [SerializeField] private Note _rootNote;
    [SerializeField] private Scale _scale;
    [SerializeField] private bool _holdCurrentNotes;
    
    [Header("Audio Effects")]
    
    [Header("Reverb")]
    [SerializeField] [Range(0, 1)] private float _reverbAmount;
    [SerializeField] [Range(0, 20)] private float _reverbTime;
    
    [Header("Filter")]
    [SerializeField] [Range(0, 1)] private float _filterAmount;
    [SerializeField] [Range(10, 22000)] private float _filterCutoff;
    
    [Header("Tuned Instruments")]
    [SerializeField] private List<TunedInstrument> _tunedInstruments;
    
    private CsoundUnity _csound;
    
    private int _currentBeat;

    private const int A4Degree = 57;

    private float _timeOfLastTapTempoPulse;

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
            
            _csound.SetChannel($"active{i}", tunedInstrument.Active && !_globalMute ? 1 : 0);
            if (!tunedInstrument.Active) continue;
            
            _csound.SetChannel($"prob{i}", tunedInstrument.Probability);
            
            _csound.SetChannel($"instrument{i}", (int)tunedInstrument.InstrumentType + 2);

            int speedDivider = tunedInstrument.Speed switch
            {
                Speed.SixteenthNotes => 1,
                Speed.EighthNotes => 2,
                Speed.DottedEighthNotes => 3,
                Speed.QuarterNotes => 4,
                Speed.DottedQuarterNotes => 6,
                Speed.HalfNotes => 8,
                Speed.DottedHalfNotes => 12,
                Speed.WholeNotes => 16,
                Speed.DottedWholeNotes => 24,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _csound.SetChannel($"speed{i}", speedDivider);
            _csound.SetChannel($"length{i}",  tunedInstrument.NoteLength);

            if (!_holdCurrentNotes)
            {
                Note randomNote = GetRandomNoteInScale(_rootNote, _scale);
                int octave = Random.Range(tunedInstrument.Range.x, tunedInstrument.Range.y + 1);
                
                double frequency = GetFrequency(randomNote, octave);
                
                _csound.SetChannel($"pitch{i}",  frequency);
            }
            
            _csound.SetChannel($"volume{i}",  tunedInstrument.Volume * _globalVolume);
        }

        _mixer.SetFloat("ReverbAmount", Mathf.Pow(_reverbAmount, 0.2f) * 10000f - 10000f);
        _mixer.SetFloat("ReverbLength", _reverbTime);
        
        _mixer.SetFloat("FilterAmount", _filterAmount * 80f - 80f);
        _mixer.SetFloat("FilterCutoff", _filterCutoff);
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
            Scale.HarmonicMinor => new() { 2, 3, 5, 7, 8, 11 },
            Scale.MelodicMinor => new() { 2, 3, 5, 7, 9, 11 },
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
        return Math.Round(_a4Frequency * Math.Pow(2, (noteDegree - A4Degree) / 12.0), 2);
    }

    public void MuteGlobal()
    {
        _globalMute = true;
    }

    public void UnmuteGlobal()
    {
        _globalMute = false;
    }

    public void ToggleMuteGlobal()
    {
        _globalMute = !_globalMute;
    }
    
    public void SetGlobalVolume(float targetVolume, float changeDuration)
    {
        targetVolume = Mathf.Max(0f,  targetVolume);
        
        targetVolume = Mathf.Clamp01(targetVolume);
        if (changeDuration <= 0f)
        {
            _globalVolume = targetVolume;
            return;
        }
        
        StartCoroutine(SetGlobalVolumeCoroutine(targetVolume, changeDuration));
    }

    private IEnumerator SetGlobalVolumeCoroutine(float targetVolume, float changeDuration)
    {
        float startVolume = _globalVolume;
        
        float startTime = Time.time;
        float endTime = startTime + changeDuration;

        while (Time.time < endTime)
        {
            float percentageOfTime = Mathf.Clamp01((Time.time - startTime) / changeDuration);
            _globalVolume = Mathf.Lerp(startVolume, targetVolume, percentageOfTime);
            yield return null;
        }
        
        _globalVolume = targetVolume;
    }

    public void SetTempo(float targetTempo, float changeDuration)
    {
        targetTempo = Mathf.Max(0f,  targetTempo);
        
        if (changeDuration <= 0f || (int)_beatsPerMinute == (int)targetTempo)
        {
            _beatsPerMinute = targetTempo;
            return;
        }
        
        StartCoroutine(SetTempoCoroutine(targetTempo, changeDuration));
    }

    private IEnumerator SetTempoCoroutine(float targetTempo, float changeDuration)
    {
        float startTempo = _beatsPerMinute;
        
        float startTime = Time.time;
        float endTime = startTime + changeDuration;

        while (Time.time < endTime)
        {
            float percentageOfTime = Mathf.Clamp01((Time.time - startTime) / changeDuration);
            _beatsPerMinute = Mathf.Lerp(startTempo, targetTempo, percentageOfTime);
            yield return null;
        }
        
        _beatsPerMinute = targetTempo;
    }

    [ContextMenu("Send Tap Tempo Pulse")]
    public void SendTapTempoPulse()
    {
        float tapTempoPulseDelta = Time.time - _timeOfLastTapTempoPulse;
        
        _beatsPerMinute = 4f * 60f / tapTempoPulseDelta;
        
        _timeOfLastTapTempoPulse = Time.time;
    }
    
    public void SetA4Frequency(float targetFrequency, float changeDuration)
    {
        if (targetFrequency is < 330f or > 660f)
        {
            Debug.LogError("Invalid frequency. Must be between 330 and 660");
            return;
        }
        
        if (changeDuration <= 0f || (int)_a4Frequency == (int)targetFrequency)
        {
            _a4Frequency = targetFrequency;
            return;
        }
        
        StartCoroutine(SetA4FrequencyCoroutine(targetFrequency, changeDuration));
    }

    private IEnumerator SetA4FrequencyCoroutine(float targetFrequency, float changeDuration)
    {
        float startFrequency = _a4Frequency;
        
        float startTime = Time.time;
        float endTime = startTime + changeDuration;

        while (Time.time < endTime)
        {
            float percentageOfTime = Mathf.Clamp01((Time.time - startTime) / changeDuration);
            _a4Frequency = Mathf.Lerp(startFrequency, targetFrequency, percentageOfTime);
            yield return null;
        }
        
        _a4Frequency = targetFrequency;
    }

    public void SetRootNote(Note rootNote)
    {
        _rootNote = rootNote;
    }

    public void SetScale(Scale scale)
    {
        _scale = scale;
    }

    public void HoldCurrentNotes(bool holdCurrentNotes)
    {
        _holdCurrentNotes = holdCurrentNotes;
    }

    public void MuteInstrument(int instrumentIndex)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        _tunedInstruments[instrumentIndex].Active =  false;
    }
    
    public void UnMuteInstrument(int instrumentIndex)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        _tunedInstruments[instrumentIndex].Active =  true;
    }
    
    public void SetInstrumentVolume(int instrumentIndex, float targetVolume, float changeDuration)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        targetVolume = Mathf.Clamp01(targetVolume);
        if (changeDuration <= 0f)
        {
            _tunedInstruments[instrumentIndex].Volume = targetVolume;
            return;
        }
        
        StartCoroutine(SetInstrumentVolumeCoroutine(instrumentIndex, targetVolume, changeDuration));
    }

    private IEnumerator SetInstrumentVolumeCoroutine(int instrumentIndex, float targetVolume, float changeDuration)
    {
        TunedInstrument instrument = _tunedInstruments[instrumentIndex];
        float startVolume = instrument.Volume;
        
        float startTime = Time.time;
        float endTime = startTime + changeDuration;

        while (Time.time < endTime)
        {
            float percentageOfTime = Mathf.Clamp01((Time.time - startTime) / changeDuration);
            instrument.Volume = Mathf.Lerp(startVolume, targetVolume, percentageOfTime);
            yield return null;
        }
        
        instrument.Volume = targetVolume;
    }

    public void SetInstrument(int instrumentIndex, TunedInstrumentType instrumentType)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        _tunedInstruments[instrumentIndex].InstrumentType =  instrumentType;
    }

    public void SetInstrumentProbability(int instrumentIndex, float probability)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        _tunedInstruments[instrumentIndex].Probability = probability;
    }

    public void SetInstrumentSpeed(int instrumentIndex, Speed speed)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        _tunedInstruments[instrumentIndex].Speed = speed;
    }

    public void SetInstrumentNoteLength(int instrumentIndex, float noteLength)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        _tunedInstruments[instrumentIndex].NoteLength = noteLength;
    }

    public void SetInstrumentRange(int instrumentIndex, int rangeStart, int rangeEnd)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        if (rangeStart > rangeEnd || rangeStart < 0 || rangeEnd > 8)
        {
            Debug.LogError("Invalid Range. Range start must be smaller than range end. All values have to be between 0 and 8.");
            return;
        }

        _tunedInstruments[instrumentIndex].Range = new(rangeStart, rangeEnd);
    }
    
    public void SetReverbAmount(float reverbAmount)
    {
        _reverbAmount =  reverbAmount;
    }

    public void SetReverbTime(float reverbTime)
    {
        _reverbTime =  reverbTime;
    }

    private bool IsInstrumentIndexValid(int instrumentIndex)
    {
        if (instrumentIndex >= 0 && instrumentIndex < _tunedInstruments.Count) return true;
        
        Debug.LogError("Invalid index.");
        return false;
    }
}