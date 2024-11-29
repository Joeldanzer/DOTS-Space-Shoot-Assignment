using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Default component for enemies with regular movement behaviour.
public struct EnemyDefault : IComponentData { }

// This class will be handles Identical to BulletEntity as we will be pooling entities as well.
public abstract class EnemyEntity : CharacterEntity
{
    // Add default components to enemy, these will be used by ALL enemies.
    protected void AddDefaultComponents(Entity entity, IBaker baker, EnemyEntity data)
    {
        baker.AddComponent(entity, new CharacterData  { m_health = data.m_health, m_moveSpeed = data.m_moveSpeed, m_isAlive = false });
        baker.AddComponent(entity, new LocalTransform { Position = new Vector3(0.0f, -100.0f, 0.0f) });
        baker.AddComponent(entity, new EnemyData { m_damage = data.m_damage, m_range = data.m_attackRange });

        if (data.m_defaultBehaviour)
            baker.AddComponent(entity, new EnemyDefault { }); // Check if this enemy will be using the default behaviour
    }

    [SerializeField] protected bool  m_defaultBehaviour = true;
    [SerializeField] protected float m_damage           = 5.0f;
    [SerializeField] protected float m_attackRange      = 1.5f;

    // Register the bullet to BullerEntityHandler so we save it's entity for use of pooling
    public abstract void RegisterEnemy(Entity entity, IBaker baker, GameObject prefab);
    // Instantiate pooling of this bullet specific directory, all enemies will have their own directory component that are attached to EnemyHandler.
    public abstract void InstantiateEnemyPool(EntityManager manager);

    public abstract void FetchEnemyEntities(EntityManager manager, out NativeArray<Entity> entityList);
}

[BurstCompile]
partial struct EnemyDefaultBehaviour : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyDefault>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float3 playerPosition = float3.zero; // if there is not player just walk to center of the world.
        bool chasePlayer      = false;       // if player is dead we stop moving. 
        foreach(var (transform, data, player) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<CharacterData>, RefRO<PlayerData>>()) // Fetch if player is alive and their position.
        {
            chasePlayer    = data.ValueRO.m_isAlive; 
            playerPosition = transform.ValueRO.Position;
        }

        MoveDefaultEnemy moveEnemy = new MoveDefaultEnemy
        {
            m_playerPosition = playerPosition,
            m_chasePlayer = chasePlayer,
            m_dt = SystemAPI.Time.DeltaTime
        };
        moveEnemy.ScheduleParallel();
    }
}

[BurstCompile]
[WithAll(typeof(EnemyDefault))]
public partial struct MoveDefaultEnemy : IJobEntity
{
    public float3 m_playerPosition;
    public bool   m_chasePlayer;
    public float  m_dt;

    public void Execute(in CharacterData data, ref LocalTransform transform)
    {
        if (m_chasePlayer && data.m_isAlive)
        {
            Vector3 newPosition = transform.Position - m_playerPosition;
            transform.Rotation = Quaternion.LookRotation(newPosition.normalized, Vector3.up); // Look at the player and move it in the forward direction. 

            transform.Position = transform.Position + m_dt * data.m_moveSpeed * transform.Forward();
        }
    }
}