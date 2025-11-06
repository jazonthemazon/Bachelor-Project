using UnityEngine;
using UnityEngine.Pool;

public class AudioManager : Singleton<AudioManager>
{
    private IObjectPool<AudioEmitter> _audioEmitterPool;

    [SerializeField] private AudioEmitter _audioEmitterPrefab;
    [SerializeField] private bool _collectionCheck = true;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxPoolSize = 100;

    private void Start()
    {
        InitializePool();
    }
    
    public AudioPlayer CreateAudio(AudioData audioData) => new AudioPlayer(audioData);

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

    private AudioEmitter CreateAudioEmitter()
    {
        AudioEmitter audioEmitter = Instantiate(_audioEmitterPrefab, transform, true);
        audioEmitter.gameObject.SetActive(false);
        return audioEmitter;
    }

    private void OnTakeFromPool(AudioEmitter audioEmitter)
    {
        audioEmitter.gameObject.SetActive(true);
    }

    private void OnReturnToPool(AudioEmitter audioEmitter)
    {
        audioEmitter.gameObject.SetActive(false);
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
}