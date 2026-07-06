using UnityEngine;

public class ReverbZone : MonoBehaviour
{
    [SerializeField] private float _reverbAmount;
    [SerializeField] private float _reverbTime;
    [SerializeField] private float _transitionTime;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        MusicGenerator.Instance.SetReverbAmount(_reverbAmount, _transitionTime);
        MusicGenerator.Instance.SetReverbTime(_reverbTime, _transitionTime);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        MusicGenerator.Instance.SetReverbAmount(0, _transitionTime);
        MusicGenerator.Instance.SetReverbTime(0, _transitionTime);
    }
}