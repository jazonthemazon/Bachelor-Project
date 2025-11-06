using System.Collections.Generic;
using UnityEngine;

public class MusicGenerator : MonoBehaviour
{
    private enum MetronomeBeats { WholeNotes, HalfNotes, QuarterNotes, SixteenthNotes }
    
    [Header("Metronome")]
    [SerializeField] private bool _metronomeActive;
    [SerializeField] private MetronomeBeats _metronomeBeats;
    [SerializeField] private AudioData _metronomeAudioOnFirstBeat;
    [SerializeField] private AudioData _metronomeAudio;

    [Header("Tempo")]
    [SerializeField][Range(10, 500)] private double _bpm = 120;
    [SerializeField][Min(2)] private int _timeSignature;
    [SerializeField][Range(50f, 99f)] private double _swing;

    [Header("Instruments")]
    [SerializeField] private List<Instrument> _instruments;

    private double _stepInterval;
    private double _nextStepTime;
    private int _stepIndex;
    
    private void Start()
    {
        StartBeat();
    }

    private void StartBeat()
    {
        _stepIndex = 0;
        _nextStepTime = AudioSettings.dspTime;
        ScheduleNextStep();
    }

    private void ScheduleNextStep()
    {
        // Play Metronome Audio
        if (_metronomeActive)
        {
            AudioData metronomeAudio = _stepIndex % _timeSignature == 0 ? _metronomeAudioOnFirstBeat : _metronomeAudio;
            AudioManager.Instance.CreateAudio(metronomeAudio).PlayScheduled(_nextStepTime);
        }

        // Advance to next step
        _stepIndex++;

        // Recalculate base interval in case bpm changed
        double baseInterval = 60.0 / _bpm;
        
        // calculate swing
        if (_timeSignature % 2 == 0)
        {
            _stepInterval = baseInterval * ((_stepIndex % 2 != 0 ? _swing : 100.0 - _swing) / 50.0);
        }
        else
        {
            _stepInterval = baseInterval;
        }

        // Calculate exact timing of next beat
        _nextStepTime = AudioSettings.dspTime + _stepInterval;

        // Recursion
        Invoke(nameof(ScheduleNextStep), (float)(_nextStepTime - AudioSettings.dspTime));
    }
}