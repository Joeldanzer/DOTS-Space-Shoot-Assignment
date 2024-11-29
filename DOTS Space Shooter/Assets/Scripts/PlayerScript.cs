using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerScript : CharacterEntity
{
    [SerializeField] private float m_fireRate    = 0.05f;
    [SerializeField] private float m_camDistance = 10.0f;

    [SerializeField] private Transform[] m_barrels;

    private int   m_barrelSelector = 0;
    private float m_timeForNextShot = 0.0f;

    private Camera        m_camera;
    private BulletPooling m_bulletPool;

    protected override void Initialize()
    {
        m_camera     = Object.FindAnyObjectByType<Camera>();
        m_bulletPool = Object.FindAnyObjectByType<BulletPooling>();

        m_camera.transform.position = new Vector3(transform.position.x, transform.position.y + m_camDistance, transform.position.z);
        m_camera.transform.LookAt(transform.position);
    }

    protected override void UpdateGameObject(float dt)
    {
        RotatePlayer();
        MovePlayer(dt);
        Shoot(dt);
    }

    private void RotatePlayer()
    {
        Vector3 target = m_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_camera.transform.position.z));
        target.y = transform.position.y;
        transform.LookAt(target, Vector3.up);
    }

    void MovePlayer(float dt)
    {
        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        Vector3 newPosition = transform.position;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        CharacterData data = m_enitiyManager.GetComponentData<CharacterData>(m_entity);

        newPosition += (forward * data.m_moveSpeed * dt) * z;
        newPosition += (right * data.m_moveSpeed * dt) * x;

        transform.position = newPosition;

        newPosition.y = m_camera.transform.position.y;
        m_camera.transform.position = newPosition;
    }

    void Shoot(float dt)
    {

        if (Input.GetMouseButton(0))
        {
            if(m_timeForNextShot <= 0.0f)
            {
                m_barrelSelector++;
                m_barrelSelector = m_barrelSelector >= m_barrels.Length ? 0 : m_barrelSelector;
                Debug.Log(m_barrelSelector);
                m_bulletPool.ShootBullet(m_barrels[m_barrelSelector], "Pistol Bullet");

                m_timeForNextShot = m_fireRate;
            }
        }

        m_timeForNextShot -= dt;
    }
}

