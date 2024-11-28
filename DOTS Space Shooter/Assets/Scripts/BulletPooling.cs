using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class BulletPooling : MonoBehaviour
{
    public List<BulletEntity> m_bulletPrefabs;
    private EntityManager     m_entityManager;

    private Dictionary<string, int> m_bulletDict;

    private void Start()
    {
        m_bulletDict  = new Dictionary<string, int>();

        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < m_bulletPrefabs.Count; i++)
        {
            BulletEntity bullet = m_bulletPrefabs[i];
            bullet.InstantiateBulletPool(m_entityManager);

           
            m_bulletDict.Add(bullet.BulletName, i);
        }
    }

    public void ShootBullet(Transform shootLocation, string bulletName)
    {
        NativeArray<Entity> bulletEntities;
        m_bulletPrefabs[m_bulletDict[bulletName]].FetchBulletEntities(m_entityManager, out bulletEntities);
        for (int i = 0;i < bulletEntities.Length; i++)
        {
            CharacterData data = m_entityManager.GetComponentData<CharacterData>(bulletEntities[i]);
            if (data.m_isAlive)
                continue;

            data.m_isAlive = true;
            LocalTransform t = m_entityManager.GetComponentData<LocalTransform>(bulletEntities[i]);
            t = new LocalTransform
            {
                Position = shootLocation.localToWorldMatrix.GetPosition(),
                Rotation = shootLocation.rotation
            };

            m_entityManager.SetComponentData(bulletEntities[i], data);
            m_entityManager.SetComponentData(bulletEntities[i], t);

            break;
        }

        bulletEntities.Dispose();
    }
}

