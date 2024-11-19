using Unity.Entities;

public struct CharacterData : IComponentData
{
    public float m_health;
    public float m_moveSpeed;

    public void DamageIntake(float damage) { m_health -= damage; }

    public bool  m_isAlive;
}

public struct PlayerTag : IComponentData { }