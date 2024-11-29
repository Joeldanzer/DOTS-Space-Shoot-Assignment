using Unity.Entities;
using UnityEngine;

public class BulletEntityHandler : MonoBehaviour
{
    // Since BulletEntityHandler will be in a SubScene it has no access to any objects outside of it so we need manually place them in.
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




