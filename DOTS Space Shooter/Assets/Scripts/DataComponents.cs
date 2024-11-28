using Unity.Entities;
using UnityEngine;

public struct CharacterData : IComponentData
{
    public float m_health;
    public float m_moveSpeed;

    public void DamageIntake(float damage) { m_health -= damage; }

    public bool  m_isAlive;
}

public struct BulletData : IComponentData
{
    public float m_damage;
    public float m_duration;
}

public struct PlayerData : IComponentData {}