using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public int range;
    public int damage;
    public float firerate; // tirs par secondes
    public float laserLifeTime;
}
