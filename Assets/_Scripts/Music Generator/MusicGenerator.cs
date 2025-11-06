using System.Collections.Generic;
using UnityEngine;

public class MusicGenerator : MonoBehaviour
{
    [Header("Metronome")]
    [SerializeField] private bool _metronomeActive;
    [SerializeField] private AudioData _metronomeAudio;
    
    [Header("Tempo")]
    [SerializeField][Min(1)] private double _bpm = 120;

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
            AudioManager.Instance.CreateAudio(_metronomeAudio).PlayScheduled(_nextStepTime);
        }

        // Advance to next step
        _stepIndex = (_stepIndex + 1) % 16;

        // Recalculate interval in case bpm changed
        _stepInterval = 60.0 / _bpm;

        // Calculate exact timing of next beat
        _nextStepTime = AudioSettings.dspTime + _stepInterval;

        // Recursion
        Invoke(nameof(ScheduleNextStep), (float)(_nextStepTime - AudioSettings.dspTime));
    }
}