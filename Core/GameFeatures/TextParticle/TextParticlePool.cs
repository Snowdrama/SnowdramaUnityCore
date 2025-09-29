using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snowdrama.Core;
using TMPro;
using System.Xml.Serialization;

/// <summary>
/// Displays a text partice at a position in world space
/// 
/// Params are Position, Duration, Size, Color
/// </summary>
public class PlayTextParticle : AMessage<Vector3, float, float, Color> { }
public class TextParticlePool2D : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float size;
    [SerializeField] private float something;

    private PlayTextParticle playTextParticle;

}
