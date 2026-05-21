using System;
using UnityEngine;

public enum InstrumentType
{
    Synth,
    Piano,
    Guitar,
    Bass
}

[Serializable]
public class Instrument
{
    public bool Active = true;
    [Range(0, 1)] public float Volume = 1f;
    public InstrumentType InstrumentType;
    [Range(0, 1)] public float Probability = 1f;
    public Speed Speed;
    [Range(0, 10)] public float NoteLength = 1f;
    public Vector2Int Range = new(0, 0);
    public bool PlayRootNoteOnly;
}