using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

    public override void RegisterBullet(Entity entity, IBaker baker, GameObject prefab)
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
            for (int i = 0; i < newBullets.Length; i++)
            {  
                LocalTransform transform = new LocalTransform()
                {
                    Position = new Vector3(0.0f, -100.0f, 0.0f),
                    Scale = 1.0f
                };
                manager.SetComponentData(newBullets[i], transform);
            }
            newBullets.Dispose();
        }
        
    }

    public override void FetchBulletEntities(EntityManager manager, out NativeArray<Entity> entityList)
    {
        entityList = manager.CreateEntityQuery(ComponentType.ReadOnly<PistolBullet>()).ToEntityArray(Allocator.TempJob);
    }
}
