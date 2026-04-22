using System;
using UnityEngine;

public class ScaleZone : MonoBehaviour
{
    [SerializeField] private Scale _scale;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MusicGenerator.Instance.SetScale(_scale);
        }
    }
}