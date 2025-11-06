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
        AudioEmitter audioEmitter = AudioManager.Instance.GetAudioEmitter();
        audioEmitter.Initialize(_audioData);
        audioEmitter.transform.position = _position;

        audioEmitter.Play();
    }
    
    public void PlayScheduled(double time)
    {
        AudioEmitter audioEmitter = AudioManager.Instance.GetAudioEmitter();
        audioEmitter.Initialize(_audioData);
        audioEmitter.transform.position = _position;

        audioEmitter.PlayScheduled(time);
    }
}