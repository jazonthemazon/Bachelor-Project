using System;
using UnityEngine;

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
    [Range(0, 10)] public float NoteLength;
    public Vector2Int Range;
}