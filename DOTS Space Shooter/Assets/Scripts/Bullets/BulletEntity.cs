using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


// I want to automize Bullets & enemy pooling as much as possible so I make an abstract class with functions used to register specific types of bullets.
public struct BulletDefault : IComponentData {}
public abstract class BulletEntity : CharacterEntity
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
    [SerializeField] protected bool   m_defaultBehaviour = true; // Should this bullet use default movement behaviour
    [SerializeField] protected int    m_poolSize         = 100;

    public int    m_nextInList = 0; // Predicts next bullet in pooling list, if it is not available we start from that point.
    public int    PoolSize   { get { return m_poolSize; } }
    public string BulletName { get { return m_bulletName; } }
    public float  Duration   { get { return m_duration; } }

    // Register the bullet to BullerEntityHandler so we save it's entity for use of pooling
    public abstract void RegisterBullet(Entity entity, IBaker baker, GameObject prefab);
    // Instantiate pooling of this bullet specific directory 
    public abstract void InstantiateBulletPool(EntityManager manager);

    public abstract void FetchBulletEntities(EntityManager manager, out NativeArray<Entity> entityList);
}

[BurstCompile]
partial struct DefaultBulletMovement: ISystem
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

[BurstCompile]
[WithAll(typeof(BulletDefault))]
public partial struct DefaultMoveBullet : IJobEntity
{
    public float dt;
    void Execute(in CharacterData data, ref LocalTransform transform)
    {
        if (data.m_isAlive)
        {
            transform.Position = transform.Position + dt * data.m_moveSpeed * transform.Forward();

        }
    }
}

[BurstCompile]
partial struct BulletDataHandler : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BulletData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BulletIsAlive isAliveJob = new BulletIsAlive
        {
            dt = SystemAPI.Time.DeltaTime
        };
        isAliveJob.ScheduleParallel();
    }
}

// Duration on all bullets are handled equally
[BurstCompile]
[WithAll(typeof(BulletData))]
public partial struct BulletIsAlive : IJobEntity
{
    public float dt;
    void Execute(ref CharacterData data, ref BulletData bullet, ref LocalTransform transform)
    {
        if (data.m_isAlive)
        {
            if(bullet.m_duration <= 0.0f)
            {
                data.m_isAlive       = false;
                bullet.m_duration    = 0.0f;
                transform.Position.y = -100.0f;
            }
            else
                bullet.m_duration -= dt;
        }
    }
}
