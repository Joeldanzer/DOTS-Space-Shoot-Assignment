using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private PlayerScript m_player;
    private EnemyPooling m_enemyPool;

    [SerializeField] private float m_spawnRange = 10.0f;
    [SerializeField] private float m_spawnTimer = 0.1f;

    private float m_currentTimer = 0.0f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_spawnRange);
    }

    void Awake()
    {
        m_player    = FindAnyObjectByType<PlayerScript>();
        m_enemyPool = FindAnyObjectByType<EnemyPooling>();
    }

    void Update()
    {
        transform.position = m_player.transform.position;
        
        m_currentTimer -= Time.deltaTime;

        if(m_currentTimer <= 0.0f)
        {
            SpawnEnemyAtLocation();
            m_currentTimer = m_spawnTimer; 
        }
    }

    void SpawnEnemyAtLocation()
    {
        float randomRotation = Random.Range(0.0f, 360.0f);

        Transform newTransform = transform;
        newTransform.rotation = Quaternion.Euler(new Vector3(0.0f, randomRotation, 0.0f));
        
        Vector3 newPosition = newTransform.forward * m_spawnRange; 
        newTransform.position = newPosition + transform.position;
        newPosition.y = 1.0f;

        m_enemyPool.SpawnEnemy(newTransform, "Enemy One");
    }
}
