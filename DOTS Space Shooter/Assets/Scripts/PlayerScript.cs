using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using UnityEngine;

public class PlayerScript : CharacterEntity
{
    public class BakePlayer : Baker<PlayerScript>
    {
        public override void Bake(PlayerScript script)
        {
            Entity entt = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entt, new PlayerTag { });
            AddComponent(entt, new CharacterData { m_health = script.m_health, m_moveSpeed = script.m_moveSpeed, m_isAlive = true });
            AddComponent(entt, new LocalTransform { Position = script.transform.position, Rotation = script.transform.rotation });

            // Save entity reference
            script.m_entity = entt;
        }
    }

    private Camera m_camera;

    protected override void Initialize()
    {
        m_camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        CharacterData data = m_manager.GetComponentData<CharacterData>(m_entity);
        if (!data.m_isAlive)
            return;

        LocalTransform t = m_manager.GetComponentData<LocalTransform>(m_entity);

        LookAtMouse(t);
        MoveInDirection(Time.deltaTime, data.m_moveSpeed, t);
    }

    private void LookAtMouse(LocalTransform t)
    {
        Vector3 target = m_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_camera.transform.position.z - transform.position.z));

        transform.LookAt(target, Vector3.up);
        t.Rotation = transform.rotation;
    }

    [BurstCompile]
    private void MoveInDirection(float dt, float moveSpeed, LocalTransform transform){
  
        float x = Input.GetAxis("X");
        float z = Input.GetAxis("Y");

        Vector3 newPosition = transform.Position;

        newPosition.x += ((x * moveSpeed) * transform.Forward().x) * dt;
        newPosition.z += ((z * moveSpeed) * transform.Forward().z) * dt;

        transform.Position = newPosition;
    }

}


