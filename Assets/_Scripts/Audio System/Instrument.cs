using System;
using UnityEngine;

[Serializable]
public class Instrument
{
    public bool _active;
    [Range(0, 1)] public float _volume = 1f;
}