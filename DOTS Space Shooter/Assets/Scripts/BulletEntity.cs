using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEngine;


// I want to automize Bullets & enemy pooling as much as possible so I make an abstract class with functions used to register specific types of bullets.
public struct BulletDefault : IComponentData {}
public abstract class BulletEntity   : CharacterEntity
{
    // Add default components to bullet
    protected void AddDefaultComponents(Entity entity, IBaker baker, BulletEntity data)
    {
        baker.AddComponent(entity, new CharacterData  { m_health = data.m_health, m_moveSpeed = data.m_moveSpeed, m_isAlive = false });
        baker.AddComponent(entity, new BulletData     { m_damage = data.m_damage, m_duration = data.m_duration });
        baker.AddComponent(entity, new LocalTransform { Position = new Vector3(0.0f, -100.0f, 0.0f) });

        if (data.m_defaultBehaviour)
            baker.AddComponent(entity, new BulletDefault { }); // Check if this bullet will use the default behaviour
    }

    [SerializeField] protected string m_bulletName       = "";
    [SerializeField] protected float  m_damage           = 10;
    [SerializeField] protected float  m_duration         = 2.0f;
    [SerializeField] protected bool   m_defaultBehaviour = true; // Tells us if this bullet will use default movement behaviour
    [SerializeField] protected int    m_poolSize         = 100;

    public int    PoolSize   { get { return m_poolSize; } }
    public string BulletName { get { return m_bulletName; } }

    // Register the bullet to BullerEntityHandler so we save it's entity for use of pooling
    public abstract void RegisterBullet(Entity entity, IBaker baker, BulletEntity prefab);
    // Instantiate pooling of this bullet specific directory 
    public abstract void InstantiateBulletPool(EntityManager manager);

    public abstract void FetchBulletEntities(EntityManager manager, out NativeArray<Entity> entityList);
}

[BurstCompile]
partial struct DefaultBulletSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BulletDefault>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        DefaultMoveBullet bulletJob = new DefaultMoveBullet
        {
            dt = SystemAPI.Time.DeltaTime
        };
        bulletJob.ScheduleParallel();
    }
}

//[BurstCompile]
[WithAll(typeof(BulletDefault))]
public partial struct DefaultMoveBullet : IJobEntity
{
    public float dt;
    void Execute(in CharacterData data, ref LocalTransform transform)
    {
        if (data.m_isAlive)
        {
            Debug.Log(data.m_isAlive);
            transform.Position = transform.Position + dt * data.m_moveSpeed * transform.Forward();

        }
    }
}