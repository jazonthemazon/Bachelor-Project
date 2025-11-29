using System;
using UnityEngine;

[Serializable]
public class TunedInstrument : Instrument
{
    [Range(0, 1)] public float _probability = 1f;
    public Speed _speed;
    public Vector2Int _range;
}