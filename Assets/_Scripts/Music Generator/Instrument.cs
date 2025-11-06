using System;
using UnityEngine;

[Serializable]
public class Instrument
{
    private enum InstrumentType { Kick, Snare }

    [SerializeField] private bool _mute;
    [SerializeField] private InstrumentType _instrumentType;
    [SerializeField][Range(0, 1)] private float _volume = 1f;
}