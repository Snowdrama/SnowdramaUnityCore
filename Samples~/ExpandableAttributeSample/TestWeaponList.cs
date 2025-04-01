using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class TestWeaponList : ScriptableObject
{
    [SerializeReference, Expandable]
    public List<TestWeapon> weapons;
}
