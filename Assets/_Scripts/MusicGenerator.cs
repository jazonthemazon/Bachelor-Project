using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicGenerator : MonoBehaviour
{
    [Header("Beat Active")]
    [SerializeField] private bool _beatActive;
    
    [Header("Tempo")]
    [SerializeField][Min(1)] private double _bpm = 120;

    [Header("Samples")]
    [SerializeField] private AudioData _kickAudio;

    [Header("Patterns (16 steps)")]
    [SerializeField] private bool[] _kickPattern = new bool[16];
    [SerializeField] private bool[] _snarePattern = new bool[16];

    [Header("Instruments")]
    [SerializeField] private List<Instrument> _instruments;

    private double _stepInterval;
    private double _nextStepTime;
    private int _stepIndex;

    private void Start()
    {
        StartBeat();
    }

    [ContextMenu("Randomize Sequence")]
    private void RandomizeSequence()
    {
        for (int x = 0; x < _snarePattern.Length; x++)
        {
            _snarePattern[x] = Random.value < 0.5f;
        }
    }

    public void StartBeat()
    {
        _stepIndex = 0;
        _nextStepTime = AudioSettings.dspTime;
        ScheduleNextStep();
    }

    private void ScheduleNextStep()
    {
        // Schedule kick
        if (_beatActive)
        {
            if (_kickPattern[_stepIndex])
            {
                AudioManager.Instance.CreateAudio(_kickAudio).PlayScheduled(_nextStepTime);
            }
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