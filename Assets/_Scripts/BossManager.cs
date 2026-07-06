using UnityEngine;

public class BossManager : MonoBehaviour
{
    [ContextMenu("Play normal music")]
    public void PlayNormalMusic()
    {
        MusicGenerator.Instance.SetTempo(90, 5);
        MusicGenerator.Instance.SetGlobalVolume(0.5f, 5);
        MusicGenerator.Instance.SetScale(Scale.MajorPentatonic);
    }
    
    [ContextMenu("Play boss music")]
    public void PlayBossMusic()
    {
        MusicGenerator.Instance.SetTempo(150, 5);
        MusicGenerator.Instance.SetGlobalVolume(1, 5);
        MusicGenerator.Instance.SetScale(Scale.PowerChord);
    }
}