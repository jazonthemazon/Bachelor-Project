using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Instrument
{
    public bool Active;
    [Range(0, 1)] public float Volume = 1f;
}