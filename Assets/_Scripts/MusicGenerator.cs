using System.Collections.Generic;
using UnityEngine;

public class MusicGenerator : MonoBehaviour
{
    [Header("Tempo")]
    [SerializeField][Range(20f, 360f)] private double _bpm = 120f;

    [Header("Samples")]
    [SerializeField] private AudioClip _kickClip;
    [SerializeField] private AudioClip _snareClip;

    [Header("Patterns (16 steps)")]
    [SerializeField] private bool[] _kickPattern = new bool[16];
    [SerializeField] private bool[] _snarePattern = new bool[16];

    [Header("Instruments")]
    [SerializeField] private List<Instrument> _instruments;

    [Header("Audio Source Pooling")]
    [SerializeField] private int _initialPoolSize;

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

        RandomizeSequence();

        // First step at current dspTime
        _nextStepTime = AudioSettings.dspTime;
    }

    [ContextMenu("Randomize Sequence")]
    private void RandomizeSequence()
    {
        for (int x = 0; x < _snarePattern.Length; x++)
        {
            _snarePattern[x] = UnityEngine.Random.value < 0.5f;
        }
    }

    [ContextMenu("Start Beat")]
    public void StartBeat()
    {
        _stepIndex = 0;
        _nextStepTime = AudioSettings.dspTime;
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
        _stepInterval = 60.0 / _bpm;

        // Calculate exact timing of next beat
        _nextStepTime = AudioSettings.dspTime + _stepInterval;

        // Recursion
        Invoke(nameof(ScheduleNextStep), (float)(_nextStepTime - AudioSettings.dspTime));
    }
}