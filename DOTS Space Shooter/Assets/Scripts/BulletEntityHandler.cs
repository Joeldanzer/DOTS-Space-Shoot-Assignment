using Unity.Entities;
using UnityEngine;

public class BulletEntityHandler : MonoBehaviour
{
    [SerializeField] private BulletPooling m_bulletPooling;

    public class BulletBaker : Baker<BulletEntityHandler>
    {
        public override void Bake(BulletEntityHandler authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            for (int i = 0; authoring.m_bulletPooling.m_bulletPrefabs.Count > i; i++)
            {
                GameObject bullet = authoring.m_bulletPooling.m_bulletPrefabs[i];                
                bullet.GetComponent<BulletEntity>().RegisterBullet(entity, this, bullet);
            }
        }
          
    }
}

public struct BulletDirectory : IComponentData
{
    public Entity m_bullet;
}




