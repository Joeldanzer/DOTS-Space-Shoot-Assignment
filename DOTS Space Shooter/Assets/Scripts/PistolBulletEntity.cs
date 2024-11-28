using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct PistolBullet          : IComponentData { }
public struct PistolBulletDirectory : IComponentData 
{
    public Entity m_bulletEntity;
}
public class PistolBulletEntity : BulletEntity
{
    public class PistolBulletBaker : Baker<PistolBulletEntity>
    {
        public override void Bake(PistolBulletEntity authoring)
        {
            if (authoring.name != "")
                Debug.Assert(authoring.name != "", "Name of bullet has not been set!");
            
            Debug.Log("Add components to Bullet");

            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            authoring.AddDefaultComponents(entity, this, authoring);
            AddComponent(entity, new PistolBullet { });
        }
    }

    public override void RegisterBullet(Entity entity, IBaker baker, BulletEntity prefab)
    {
        baker.AddComponent(entity, new PistolBulletDirectory { m_bulletEntity = baker.GetEntity(prefab, TransformUsageFlags.Dynamic) });
    }

    public override void InstantiateBulletPool(EntityManager manager)
    {
        EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<PistolBulletDirectory>().Build(manager);
        if (query.HasSingleton<PistolBulletDirectory>())
        {
            NativeArray<Entity> newBullets = new NativeArray<Entity>(PoolSize, Allocator.TempJob);
            manager.Instantiate(query.GetSingleton<PistolBulletDirectory>().m_bulletEntity, newBullets);
            newBullets.Dispose();
        }
        
    }

    public override void FetchBulletEntities(EntityManager manager, out NativeArray<Entity> entityList)
    {
        entityList = manager.CreateEntityQuery(ComponentType.ReadOnly<PistolBullet>()).ToEntityArray(Allocator.TempJob);
    }
}
