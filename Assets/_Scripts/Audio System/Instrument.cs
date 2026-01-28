using System;
using UnityEngine;

[Serializable]
public class Instrument
{
    public bool Active;
    [Range(0, 1)] public float Volume = 1f;
}