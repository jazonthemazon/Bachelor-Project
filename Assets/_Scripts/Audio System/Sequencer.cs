using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CsoundUnity))]
public class Sequencer : MonoBehaviour
{
    [SerializeField] private List<GameObject> _cubes;
    [SerializeField] [Range(0, 1000)] private float _beatsPerMinute;

    private CsoundUnity _csound;
    private int _currentBeat;

    private void Start()
    {
        _csound = GetComponent<CsoundUnity>();
        _csound.SetChannel("tempo", _beatsPerMinute / 60f);
    }

    private void Update()
    {
        _csound.SetChannel("tempo", _beatsPerMinute / 60f);
        if (_currentBeat == _csound.GetChannel("beat"))
        {
            ResizeCube(_currentBeat);
            _currentBeat = _currentBeat < 7 ? _currentBeat + 1 : 0;
        }
    }

    private void ResizeCube(int index)
    {
        for (int i = 0; i < _cubes.Count; i++)
        {
            float scale = i == index ? 1.5f : 1;
            _cubes[i].transform.localScale = new Vector3(1, 1, 1) * scale;
        }
    }
}