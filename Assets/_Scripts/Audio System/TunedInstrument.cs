using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum TunedInstrumentType
{
    Synth,
    Piano,
    Guitar,
    Bass
}

[Serializable]
public class TunedInstrument : Instrument
{
    public TunedInstrumentType InstrumentType;
    [Range(0, 1)] public float Probability = 1f;
    public Speed Speed;
    public Vector2Int Range;
}