using Unity.Entities;

public struct CharacterData : IComponentData
{
    public float m_health;
    public float m_moveSpeed;

    public float m_collisionRadius;

    public void DamageIntake(float damage) { m_health -= damage; }

    public bool m_isAlive;
}

public struct EnemyData  : IComponentData
{
    public float m_damage;
    public float m_range;
    public float m_attackTimer;
    public float m_currentTimer;
}
public struct BulletData : IComponentData
{
    public float m_damage;
    public float m_duration;
}

public struct PlayerData : IComponentData {}