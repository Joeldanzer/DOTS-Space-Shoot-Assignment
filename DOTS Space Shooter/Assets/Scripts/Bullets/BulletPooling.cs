using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public class BulletPooling : MonoBehaviour
{
    public List<GameObject>   m_bulletPrefabs;
    private EntityManager     m_entityManager;

    private Dictionary<string, int> m_bulletDict;
    private List<BulletEntity>      m_bullets;

    private void Start()
    {
        m_bulletDict  = new Dictionary<string, int>();
        m_bullets     = new List<BulletEntity>(m_bulletPrefabs.Capacity);

        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < m_bulletPrefabs.Count; i++)
        {
            GameObject   bullet     = m_bulletPrefabs[i];
            BulletEntity bulletEntt = bullet.GetComponent<BulletEntity>();
            bulletEntt.InstantiateBulletPool(m_entityManager);

            m_bullets.Add(bulletEntt);
            m_bulletDict.Add(bulletEntt.BulletName, i);
        }
    }

    public void ShootBullet(Transform shootLocation, string bulletName)
    {
        NativeArray<Entity> bulletEntities;
        int bulletIndex = m_bulletDict[bulletName];
        BulletEntity prefab = m_bullets[bulletIndex];
        prefab.FetchBulletEntities(m_entityManager, out bulletEntities);

        Entity nextBullet = bulletEntities[m_bullets[bulletIndex].m_nextInList];
        if (!ActivateBulletEntity(nextBullet, shootLocation, prefab))
        {
            for (int i = 0; i < bulletEntities.Length; i++)
            {
                prefab.m_nextInList++;
                prefab.m_nextInList = prefab.m_nextInList >= prefab.PoolSize ? 0 : prefab.m_nextInList;
                if (ActivateBulletEntity(bulletEntities[prefab.m_nextInList], shootLocation, prefab))
                    break;
            }
        }
        else
        {
            prefab.m_nextInList++;
            prefab.m_nextInList = prefab.m_nextInList >= prefab.PoolSize ? 0 : prefab.m_nextInList;
        }

        

        bulletEntities.Dispose();
    }

    private bool ActivateBulletEntity(Entity bullet, Transform shootLocation, BulletEntity prefab)
    {
        CharacterData data = m_entityManager.GetComponentData<CharacterData>(bullet);
        if (data.m_isAlive)
            return false;

        data.m_isAlive = true;
        LocalTransform t = new LocalTransform
        {
            Position = shootLocation.localToWorldMatrix.GetPosition(),
            Rotation = shootLocation.rotation,
            Scale = 1.0f
        };

        BulletData bulletData = m_entityManager.GetComponentData<BulletData>(bullet);
        bulletData.m_duration = prefab.Duration;

        m_entityManager.SetComponentData(bullet, bulletData);
        m_entityManager.SetComponentData(bullet, data);
        m_entityManager.SetComponentData(bullet, t);

        return true;
    }
}

