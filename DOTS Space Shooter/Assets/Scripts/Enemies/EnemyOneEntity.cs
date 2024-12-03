using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct EnemyOne          : IComponentData { }
public struct EnemyOneDirectory : IComponentData { public Entity m_entity; }

public class EnemyOneEntity : EnemyEntity
{
    public class EnemyOneBaker : Baker<EnemyOneEntity>
    {
        public override void Bake(EnemyOneEntity authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            authoring.AddDefaultComponents(entity, this, authoring);
            AddComponent(entity, new EnemyOne { });
        }
    }

    public override void RegisterEnemy(Entity entity, IBaker baker, GameObject prefab)
    {
        baker.AddComponent(entity, new EnemyOneDirectory { m_entity = baker.GetEntity(prefab, TransformUsageFlags.Dynamic) });
    }

    public override void InstantiateEnemyPool(EntityManager manager)
    {
        EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<EnemyOneDirectory>().Build(manager);
        if (query.HasSingleton<EnemyOneDirectory>())
        {
            NativeArray<Entity> newEnemies = new NativeArray<Entity>(m_poolSize, Allocator.TempJob);
            manager.Instantiate(query.GetSingleton<EnemyOneDirectory>().m_entity, newEnemies);
            for (int i = 0; i < newEnemies.Length; i++)
            {
                LocalTransform transform = new LocalTransform()
                {
                    Position = new Vector3(0.0f, -1000.0f, 0.0f),
                    Scale = 1.0f
                };
                manager.SetComponentData(newEnemies[i], transform);
            }
            newEnemies.Dispose();
        }

    }

    public override void FetchEnemyEntities(EntityManager manager, out NativeArray<Entity> entityList)
    {
        entityList = manager.CreateEntityQuery(ComponentType.ReadOnly<EnemyOne>()).ToEntityArray(Allocator.TempJob);
    }
}
