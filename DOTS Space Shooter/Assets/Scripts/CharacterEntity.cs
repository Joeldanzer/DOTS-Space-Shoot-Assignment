
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    [SerializeField] protected float m_health;
    [SerializeField] protected float m_moveSpeed;

    protected EntityManager m_enitiyManager;
    protected Entity        m_entity;

    private void Start()
    {
        m_enitiyManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        m_entity        = m_enitiyManager.CreateEntity();

        m_enitiyManager.AddComponent<PlayerData>(m_entity);
        m_enitiyManager.AddComponentData(m_entity, new LocalTransform {
            Position = transform.position,
            Rotation = transform.rotation
        });
        m_enitiyManager.AddComponentData(m_entity, new CharacterData {
            m_health    = m_health, 
            m_moveSpeed = m_moveSpeed,
            m_isAlive   = true
        });

        Initialize();
    }

    protected virtual void Initialize() { }
    protected virtual void UpdateGameObject(float dt) { }

    private void Update()
    {
        UpdateGameObject(Time.deltaTime);

        // Automaticall Update transform data for all CharacterEntities
        LocalTransform t = m_enitiyManager.GetComponentData<LocalTransform>(m_entity);
        
        t.Position = transform.position;
        t.Rotation = transform.rotation;
    }
}
