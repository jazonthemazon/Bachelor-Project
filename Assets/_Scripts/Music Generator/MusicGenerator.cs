using System.Collections.Generic;
using UnityEngine;

public class MusicGenerator : MonoBehaviour
{
    [Header("Metronome")]
    [SerializeField] private bool _metronomeActive;
    [SerializeField] private AudioData _metronomeAudioOnFirstBeat;
    [SerializeField] private AudioData _metronomeAudio;
    
    [Header("Tempo")]
    [SerializeField][Range(10, 500)] private double _bpm = 120;
    [SerializeField][Min(1)] private int _timeSignature;
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
            AudioData metronomeAudio = _stepIndex == 0 ? _metronomeAudioOnFirstBeat : _metronomeAudio;
            AudioManager.Instance.CreateAudio(metronomeAudio).PlayScheduled(_nextStepTime);
        }

        // Advance to next step
        _stepIndex = (_stepIndex + 1) % _timeSignature;

        // Recalculate interval in case bpm changed
        _stepInterval = 60.0 / _bpm;
        
        // calculate swing
        double stepIntervalWithSwing = _stepInterval * (_swing / 50.0);
        
        // add swing to interval
        _stepInterval = _stepIndex % 2 != 0 ? stepIntervalWithSwing : _stepInterval * 2 - stepIntervalWithSwing;

        // Calculate exact timing of next beat
        _nextStepTime = AudioSettings.dspTime + _stepInterval;

        // Recursion
        Invoke(nameof(ScheduleNextStep), (float)(_nextStepTime - AudioSettings.dspTime));
    }
}