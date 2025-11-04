using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AudioManager : Singleton<AudioManager>
{
    private IObjectPool<AudioEmitter> _audioEmitterPool;
    private List<AudioEmitter> _activeAudioEmitters = new();
    public readonly Dictionary<AudioData, int> Counts = new();

    [SerializeField] private AudioEmitter _audioEmitterPrefab;
    [SerializeField] private bool _collectionCheck = true;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxPoolSize = 100;
    [SerializeField] private int _maxSoundInstance = 30;

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        _audioEmitterPool = new ObjectPool<AudioEmitter>(
            CreateAudioEmitter,
            OnTakeFromPool,
            OnReturnToPool,
            OnDestroyPoolObject,
            _collectionCheck,
            _defaultCapacity,
            _maxPoolSize);
    }

    AudioEmitter CreateAudioEmitter()
    {
        AudioEmitter audioEmitter = Instantiate(_audioEmitterPrefab);
        audioEmitter.gameObject.SetActive(false);
        return audioEmitter;
    }

    private void OnTakeFromPool(AudioEmitter audioEmitter)
    {
        audioEmitter.gameObject.SetActive(true);
        _activeAudioEmitters.Add(audioEmitter);
    }

    private void OnReturnToPool(AudioEmitter audioEmitter)
    {
        audioEmitter.gameObject.SetActive(false);
        _activeAudioEmitters.Remove(audioEmitter);
    }

    private void OnDestroyPoolObject(AudioEmitter audioEmitter)
    {
        Destroy(audioEmitter.gameObject);
    }

    public AudioEmitter GetAudioEmitter()
    {
        return _audioEmitterPool.Get();
    }

    public void ReturnToPool(AudioEmitter audioEmitter)
    {
        _audioEmitterPool.Release(audioEmitter);
    }

    public bool CanPlayAudio(AudioData data)
    {
        return !Counts.TryGetValue(data, out int count) || count  < _maxSoundInstance;
    }
}