using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEmitter : MonoBehaviour
{
    public AudioData Data { get; private set; }
    
    private AudioSource _audioSource;
    private Coroutine _playingCoroutine;

    private void Awake()
    {
        _audioSource = gameObject.GetOrAddComponent<AudioSource>();
    }

    public void Play()
    {
        if (_playingCoroutine != null) StopCoroutine(_playingCoroutine);

        _audioSource.Play();
        _playingCoroutine = StartCoroutine(WaitForSoundToEnd());
    }
    
    public void PlayScheduled(double time)
    {
        if (_playingCoroutine != null) StopCoroutine(_playingCoroutine);

        _audioSource.PlayScheduled(time);
        _playingCoroutine = StartCoroutine(WaitForSoundToEnd());
    }

    private IEnumerator WaitForSoundToEnd()
    {
        yield return new WaitWhile(()  => _audioSource.isPlaying);
        AudioManager.Instance.ReturnToPool(this);
    }

    public void Stop()
    {
        if (_playingCoroutine != null)
        {
            StopCoroutine(_playingCoroutine);
            _playingCoroutine = null;
        }
        
        _audioSource.Stop();
        AudioManager.Instance.ReturnToPool(this);
    }

    public void Initialize(AudioData data)
    {
        Data = data;
        _audioSource.clip = data.clip;
        _audioSource.outputAudioMixerGroup = data.mixerGroup;
    }
}