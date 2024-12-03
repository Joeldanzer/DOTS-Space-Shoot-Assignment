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

    private Dictionary<string, int> m_enemyDict;
    private List<EnemyEntity>       m_enemies;

    private void Start()
    {
        m_enemyDict = new Dictionary<string, int>();
        m_enemies = new List<EnemyEntity>(m_enemyPrefabs.Capacity);

        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < m_enemyPrefabs.Count; i++)
        {
            GameObject enemy = m_enemyPrefabs[i];
            EnemyEntity enemyEntt = enemy.GetComponent<EnemyEntity>();
            enemyEntt.InstantiateEnemyPool(m_entityManager);

            m_enemies.Add(enemyEntt);
            m_enemyDict.Add(enemyEntt.EnemyName, i);
        }
    }

    public void SpawnEnemy(Transform shootLocation, string enemyName)
    {
        NativeArray<Entity> enemyEntities;
        int enemyIndex     = m_enemyDict[enemyName];
        EnemyEntity prefab = m_enemies[enemyIndex];
        prefab.FetchEnemyEntities(m_entityManager, out enemyEntities);

        Entity nextEnemy = enemyEntities[m_enemies[enemyIndex].NextEnemy];
        if (!ActivateEnemy(nextEnemy, shootLocation, prefab))
        {
            for (int i = 0; i < enemyEntities.Length; i++)
            {
                prefab.NextEnemy++;
                prefab.NextEnemy = prefab.NextEnemy >= prefab.PoolSize ? 0 : prefab.NextEnemy;
                if (ActivateEnemy(enemyEntities[prefab.NextEnemy], shootLocation, prefab))
                    break;
            }
        }
        else
        {
            prefab.NextEnemy++;
            prefab.NextEnemy = prefab.NextEnemy >= prefab.PoolSize ? 0 : prefab.NextEnemy;
        }


        enemyEntities.Dispose();
    }

    private bool ActivateEnemy(Entity enemy, Transform spawnLocation, EnemyEntity prefab)
    {
        CharacterData data = m_entityManager.GetComponentData<CharacterData>(enemy);
        if (data.m_isAlive)
            return false;

        data.m_isAlive = true;
        data.m_health  = prefab.Health;
        LocalTransform t = new LocalTransform
        {
            Position = spawnLocation.position,
            Rotation = spawnLocation.rotation,
            Scale = 1.0f
        };

        m_entityManager.SetComponentData(enemy, data);
        m_entityManager.SetComponentData(enemy, t);

        return true;
    }
}
