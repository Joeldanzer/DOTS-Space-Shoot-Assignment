using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyEntityHandler : MonoBehaviour
{
    // Since BulletEntityHandler will be in a SubScene it has no access to any objects outside of it so we need manually place them in.
    [SerializeField] private EnemyPooling m_enemyPooling;

    public class BulletBaker : Baker<EnemyEntityHandler>
    {
        public override void Bake(EnemyEntityHandler authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            for (int i = 0; authoring.m_enemyPooling.m_enemyPrefabs.Count > i; i++)
            {
                GameObject enemy = authoring.m_enemyPooling.m_enemyPrefabs[i];
                enemy.GetComponent<EnemyEntity>().RegisterEnemy(entity, this, enemy);
            }
        }

    }
}
