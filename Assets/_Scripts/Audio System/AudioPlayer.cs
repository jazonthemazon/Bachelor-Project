using UnityEngine;

public class AudioPlayer
{
    private readonly AudioData _audioData;
    private Vector3 _position = Vector3.zero;

    public AudioPlayer(AudioData audioData)
    {
        _audioData = audioData;
    }

    public AudioPlayer WithPosition(Vector3 position)
    {
        _position = position;
        return this;
    }

    public void Play()
    {
        GetAndInitializeAudioEmitter().Play();
    }
    
    public void PlayScheduled(double time)
    {
        GetAndInitializeAudioEmitter().PlayScheduled(time);
    }

    private AudioEmitter GetAndInitializeAudioEmitter()
    {
        AudioEmitter audioEmitter = AudioManager.Instance.GetAudioEmitter();
        audioEmitter.Initialize(_audioData);
        audioEmitter.transform.position = _position;
        return audioEmitter;
    }
}