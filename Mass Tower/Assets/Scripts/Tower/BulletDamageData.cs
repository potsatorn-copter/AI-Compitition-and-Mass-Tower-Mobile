using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Bullet Damage", menuName = "ScriptableObjects/Bullet Damage Config", order = 2)]

public class BulletDamageData : ScriptableObject
{
    [Header("Bullet Damage")][SerializeField] private int _damage = 1; // ตัวแปร moveSpeed ที่สามารถเปลี่ยนแปลงได้

    public int BulletDamage => _damage;
}
