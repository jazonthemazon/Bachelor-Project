using System;
using UnityEngine;

public class MusicGenerator : MonoBehaviour
{
    [Header("Tempo")]
    [Range(20f, 360f)] public float _bpm = 120f;

    [Header("Samples")]
    public AudioClip _kickClip;
    public AudioClip _snareClip;

    [Header("Patterns (16 steps)")]
    public bool[] _kickPattern = new bool[16];
    public bool[] _snarePattern = new bool[16];

    private AudioSource _kickSource;
    private AudioSource _snareSource;

    private double _stepInterval;
    private double _nextStepTime;
    private int _stepIndex = 0;

    void Start()
    {
        // Create audio sources
        _kickSource = gameObject.AddComponent<AudioSource>();
        _snareSource = gameObject.AddComponent<AudioSource>();

        _kickSource.clip = _kickClip;
        _snareSource.clip = _snareClip;

        // First step at current dspTime
        _nextStepTime = AudioSettings.dspTime;

        // Start scheduling
        ScheduleNextStep();
    }

    private void ScheduleNextStep()
    {
        // Schedule kick
        if (_kickPattern[_stepIndex])
        {
            _kickSource.PlayScheduled(_nextStepTime);
        }

        // Schedule snare
        if (_snarePattern[_stepIndex])
        {
            _snareSource.PlayScheduled(_nextStepTime);
        }

        // Advance to next step
        _stepIndex = (_stepIndex + 1) % 16;

        // Recalculate interval in case bpm changed
        _stepInterval = 60.0f / _bpm;

        // Calculate exact timing of next beat
        _nextStepTime = AudioSettings.dspTime + _stepInterval;

        // Recursion
        Invoke(nameof(ScheduleNextStep), (float)(_nextStepTime - AudioSettings.dspTime));
    }
}