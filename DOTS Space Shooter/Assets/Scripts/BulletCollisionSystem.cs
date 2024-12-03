using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct EnemyCollision : ISystem
{
    EntityQuery m_bulletsQuery;
    EntityQuery m_enemyQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //If no enemies exist we wont be running this.
        state.RequireForUpdate<EnemyData>();
        state.RequireForUpdate<BulletData>();

        m_bulletsQuery = SystemAPI.QueryBuilder().WithAll<CharacterData, BulletData, LocalTransform>().Build();
        m_enemyQuery   = SystemAPI.QueryBuilder().WithAll<CharacterData, EnemyData, LocalTransform>().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EnemyCollisionJob job = new EnemyCollisionJob()
        {
            m_bulletTransform     = m_bulletsQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob),
            m_bulletCharacterData = m_bulletsQuery.ToComponentDataArray<CharacterData>(Allocator.TempJob),
            m_bulletData          = m_bulletsQuery.ToComponentDataArray<BulletData>(Allocator.TempJob)
        };
        
        state.Dependency = job.ScheduleParallel(m_enemyQuery, state.Dependency);
    }
}

[BurstCompile]
partial struct EnemyCollisionJob : IJobEntity
{
    // This doesnt seem like the most optimal way to do this...
    [DeallocateOnJobCompletion][ReadOnly] public NativeArray<CharacterData>  m_bulletCharacterData;
    [DeallocateOnJobCompletion][ReadOnly] public NativeArray<LocalTransform> m_bulletTransform;
    [DeallocateOnJobCompletion][ReadOnly] public NativeArray<BulletData>     m_bulletData;

    public void Execute(ref CharacterData data, ref LocalTransform transform)
    {
        if (data.m_isAlive)
        {
            for (int i = 0; i < m_bulletCharacterData.Length; i++)
            {
                CharacterData bulletCharData = m_bulletCharacterData[i];
                if (bulletCharData.m_isAlive)
                {
                    BulletData     bulletData      = m_bulletData[i];
                    LocalTransform bulletTransform = m_bulletTransform[i];
                    if(CheckCollision(transform.Position, bulletTransform.Position, data.m_collisionRadius * bulletCharData.m_collisionRadius))
                    {
                        data.m_health -= bulletData.m_damage;
                        
                        if(data.m_health <= 0)
                        {
                            data.m_isAlive       = false;
                            transform.Position.y = -1000.0f;
                            break;
                        }
                    } 
                }
                
            }
        }
    }

    bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
    {
        float3 delta = posA - posB;
        float distanceSquare = delta.x * delta.x + delta.z * delta.z;

        return distanceSquare <= radiusSqr;
    }
}
