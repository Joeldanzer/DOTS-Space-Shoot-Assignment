using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class EnemyPooling : MonoBehaviour
{
    public List<GameObject> m_enemyPrefabs;
    private EntityManager m_entityManager;

    private Dictionary<string, int> m_bulletDict;
    private List<EnemyEntity>       m_enemies;

    private void Start()
    {
        m_bulletDict = new Dictionary<string, int>();
        m_enemies = new List<EnemyEntity>(m_enemyPrefabs.Capacity);

        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < m_enemyPrefabs.Count; i++)
        {
            GameObject enemy = m_enemyPrefabs[i];
            EnemyEntity enemyEntt = enemy.GetComponent<EnemyEntity>();
            enemyEntt.InstantiateEnemyPool(m_entityManager);

            m_enemies.Add(enemyEntt);
            //m_bulletDict.Add(enemyEntt.BulletName, i);
        }
    }

    public void ShootBullet(Transform shootLocation, string enemyName)
    {
        NativeArray<Entity> enemyEntities;
        int enemyIndex = m_bulletDict[enemyName];
        EnemyEntity prefab = m_enemies[enemyIndex];
        prefab.FetchEnemyEntities(m_entityManager, out enemyEntities);

        //Entity nextBullet = bulletEntities[m_enemies[enemyIndex].m_nextInList];
        //if (!ActivateBulletEntity(nextBullet, shootLocation, prefab))
        //{
        //    for (int i = 0; i < bulletEntities.Length; i++)
        //    {
        //        prefab.m_nextInList++;
        //        prefab.m_nextInList = prefab.m_nextInList >= prefab.PoolSize ? 0 : prefab.m_nextInList;
        //        if (ActivateBulletEntity(bulletEntities[prefab.m_nextInList], shootLocation, prefab))
        //            break;
        //    }
        //}
        //else
        //{
        //    prefab.m_nextInList++;
        //    prefab.m_nextInList = prefab.m_nextInList >= prefab.PoolSize ? 0 : prefab.m_nextInList;
        //}


        enemyEntities.Dispose();
    }

    private bool ActivateBulletEntity(Entity bullet, Transform spawnLocation, EnemyEntity prefab)
    {
        CharacterData data = m_entityManager.GetComponentData<CharacterData>(bullet);
        if (data.m_isAlive)
            return false;

        data.m_isAlive = true;
        LocalTransform t = new LocalTransform
        {
            Position = spawnLocation.localToWorldMatrix.GetPosition(),
            Rotation = spawnLocation.rotation,
            Scale = 1.0f
        };

        m_entityManager.SetComponentData(bullet, data);
        m_entityManager.SetComponentData(bullet, t);

        return true;
    }
}
