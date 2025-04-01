using UnityEngine;

[CreateAssetMenu]
public class TestWeapon : ScriptableObject
{
    public string weaponName;
    public float damageMin;
    public float damageMax;

    [EditorReadOnly]
    public float random = 1.5f;
    public TestElement element;
}
