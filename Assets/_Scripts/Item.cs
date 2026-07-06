using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _maxVolume;
    [SerializeField] private float _minVolume;
    [SerializeField] private float _range;
    [SerializeField] private Color _gizmoColor;

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
        float volume = Mathf.Max(_minVolume, _maxVolume - distanceToPlayer * (_maxVolume / _range));
        MusicGenerator.Instance.SetGlobalVolume(volume, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}