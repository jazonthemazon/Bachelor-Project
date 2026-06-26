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
    
    [Header("Chorus")]
    [SerializeField] [Range(0, 1)] private float _chorusAmount;
    
    [Header("Echo")]
    [SerializeField] [Range(0, 1)] private float _echoVolume;
    [SerializeField] [Range(0.01f, 5)] private float _echoDelayTime;
    [SerializeField] [Range(0, 1)] private float _echoDecay;
    
    [Header("Reverb")]
    [SerializeField] [Range(0, 1)] private float _reverbAmount;
    [SerializeField] [Range(0.1f, 20)] private float _reverbTime;
    
    [Header("Filter")]
    [SerializeField] [Range(0, 1)] private float _filterAmount;
    [SerializeField] [Range(10, 22000)] private float _filterCutoff;
    
    [Header("Instruments")]
    [SerializeField] private List<Instrument> _instruments;
    
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

        if (_instruments.Count > 8)
        {
            Debug.LogError("Too many instruments! Maximum is 8.");
            return;
        }
        
        // update parameters per instrument
        for (var i = 0; i < _instruments.Count; i++)
        {
            Instrument instrument = _instruments[i];
            
            _csound.SetChannel($"active{i}", instrument.Active && !_globalMute ? 1 : 0);
            if (!instrument.Active) continue;
            
            _csound.SetChannel($"prob{i}", instrument.Probability);
            
            _csound.SetChannel($"instrument{i}", (int)instrument.InstrumentType + 2);

            int speedDivider = instrument.Speed switch
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
            _csound.SetChannel($"length{i}",  instrument.NoteLength);

            if (!_holdCurrentNotes)
            {
                Note randomNote = instrument.PlayRootNoteOnly ? _rootNote : GetRandomNoteInScale(_rootNote, _scale);

                if (instrument.Range.x > instrument.Range.y)
                {
                    Debug.LogError("Lower limit of range must not be higher than upper limit!");
                    continue;
                }
                int octave = Random.Range(instrument.Range.x, instrument.Range.y + 1);
                
                double frequency = GetFrequency(randomNote, octave);
                
                _csound.SetChannel($"pitch{i}",  frequency);
            }
            
            _csound.SetChannel($"volume{i}", instrument.Volume * _globalVolume);
        }
        
        // Chorus
        _mixer.SetFloat("ChorusAmount", _chorusAmount);
        
        //Echo
        _mixer.SetFloat("EchoVolume", _echoVolume);
        _mixer.SetFloat("EchoDelayTime", _echoDelayTime * 1000f);
        _mixer.SetFloat("EchoDecay", _echoDecay);
        
        // Reverb
        _mixer.SetFloat("ReverbAmount", Mathf.Pow(_reverbAmount, 0.2f) * 10000f - 10000f);
        _mixer.SetFloat("ReverbLength", _reverbTime);
        
        // Filter
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
        targetVolume = Mathf.Clamp01(targetVolume);
        
        ChangeValue(
            () => _globalVolume,
            value => _globalVolume = value,
            targetVolume,
            changeDuration);
    }

    public void SetTempo(float targetTempo, float changeDuration)
    {
        targetTempo = Mathf.Max(0f,  targetTempo);
        
        ChangeValue(
            () => _beatsPerMinute,
            value => _beatsPerMinute = value,
            targetTempo,
            changeDuration);
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
        
        ChangeValue(
            () => _a4Frequency,
            value => _a4Frequency = value,
            targetFrequency,
            changeDuration);
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
        
        _instruments[instrumentIndex].Active =  false;
    }
    
    public void UnMuteInstrument(int instrumentIndex)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        _instruments[instrumentIndex].Active =  true;
    }
    
    public void SetInstrumentVolume(int instrumentIndex, float targetVolume, float changeDuration)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        targetVolume = Mathf.Clamp01(targetVolume);
        
        ChangeValue(
            () => _instruments[instrumentIndex].Volume,
            value => _instruments[instrumentIndex].Volume = value,
            targetVolume,
            changeDuration);
    }

    public void SetInstrument(int instrumentIndex, InstrumentType instrumentType)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        _instruments[instrumentIndex].InstrumentType =  instrumentType;
    }

    public void SetInstrumentProbability(int instrumentIndex, float probability, float changeDuration)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        ChangeValue(
            () => _instruments[instrumentIndex].Probability,
            value => _instruments[instrumentIndex].Probability = value,
            probability,
            changeDuration);
    }

    public void SetInstrumentSpeed(int instrumentIndex, Speed speed)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        _instruments[instrumentIndex].Speed = speed;
    }

    public void SetInstrumentNoteLength(int instrumentIndex, float noteLength, float changeDuration)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        ChangeValue(
            () => _instruments[instrumentIndex].NoteLength,
            value => _instruments[instrumentIndex].NoteLength = value,
            noteLength,
            changeDuration);
    }

    public void SetInstrumentRange(int instrumentIndex, int rangeStart, int rangeEnd)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;
        
        if (rangeStart > rangeEnd || rangeStart < 0 || rangeEnd > 8)
        {
            Debug.LogError("Invalid Range. Range lower limit must be smaller than upper limit. All values have to be between 0 and 8.");
            return;
        }

        _instruments[instrumentIndex].Range = new(rangeStart, rangeEnd);
    }

    public void SetInstrumentPlayRootNoteOnly(int instrumentIndex, bool playRootNoteOnly)
    {
        if (!IsInstrumentIndexValid(instrumentIndex)) return;

        _instruments[instrumentIndex].PlayRootNoteOnly = playRootNoteOnly;
    }

    public void SetChorusAmount(float chorusAmount, float changeDuration)
    {
        ChangeValue(
            () => _chorusAmount,
            value => _chorusAmount = value,
            chorusAmount,
            changeDuration);
    }

    public void SetEchoVolume(float echoAmount, float changeDuration)
    {
        ChangeValue(
            () => _echoVolume,
            value => _echoVolume = value,
            echoAmount,
            changeDuration);
    }

    public void SetEchoDelayTime(float echoDelayTime, float changeDuration)
    {
        ChangeValue(
            () => _echoDelayTime,
            value => _echoDelayTime = value,
            echoDelayTime,
            changeDuration);
    }

    public void SetEchoDecay(float echoDecay, float changeDuration)
    {
        ChangeValue(
            () => _echoDecay,
            value => _echoDecay = value,
            echoDecay,
            changeDuration);
    }
    
    public void SetReverbAmount(float reverbAmount, float changeDuration)
    {
        ChangeValue(
            () => _reverbAmount,
            value => _reverbAmount = value,
            reverbAmount,
            changeDuration);
    }

    public void SetReverbTime(float reverbTime, float changeDuration)
    {
        ChangeValue(
            () => _reverbTime,
            value => _reverbTime = value,
            reverbTime,
            changeDuration);
    }

    public void SetFilterAmount(float filterAmount, float changeDuration)
    {
        ChangeValue(
            () => _filterAmount,
            value => _filterAmount = value,
            filterAmount,
            changeDuration);
    }

    public void SetFilterCutoff(float filterFrequency, float changeDuration)
    {
        ChangeValue(
            ()  => _filterCutoff,
            value => _filterCutoff = value,
            filterFrequency,
            changeDuration);
    }

    private void ChangeValue(
        Func<float> getter,
        Action<float> setter,
        float targetValue,
        float duration)
    {
        if (duration <= 0f)
        {
            setter(targetValue);
            return;
        }
        
        StartCoroutine(ChangeValueOverTime(getter, setter, targetValue, duration));
    }
    
    private IEnumerator ChangeValueOverTime(
        Func<float> getter,
        Action<float> setter,
        float targetValue,
        float duration)
    {
        float startValue = getter();
        
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / duration);
            setter(Mathf.Lerp(startValue, targetValue, t));
            
            yield return null;
        }
        
        setter(targetValue);
    }

    private bool IsInstrumentIndexValid(int instrumentIndex)
    {
        if (instrumentIndex >= 0 && instrumentIndex < _instruments.Count) return true;
        
        Debug.LogError("Invalid instrument index.");
        return false;
    }
}