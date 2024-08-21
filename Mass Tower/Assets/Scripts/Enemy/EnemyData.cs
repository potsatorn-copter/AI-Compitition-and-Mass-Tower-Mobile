using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemy Move Speed Config", order = 1)]

public class EnemyData : ScriptableObject
{
    [Header("Speed")][SerializeField] private float _Speed = 3f; // ตัวแปร moveSpeed ที่สามารถเปลี่ยนแปลงได้

    public float MoveSpeed => _Speed;
    
    [Header("Health")][SerializeField] private int _health = 5; // ตัวแปร moveSpeed ที่สามารถเปลี่ยนแปลงได้

    public float Health => _health;
    
    

}
