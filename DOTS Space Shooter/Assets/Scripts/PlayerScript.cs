using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerScript : CharacterEntity
{
    public class BakePlayer : Baker<PlayerScript>
    {
        public override void Bake(PlayerScript script)
        {
            Entity entt = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entt, new PlayerData { });
            AddComponent(entt, new CharacterData  { m_health = script.m_health, m_moveSpeed = script.m_moveSpeed, m_isAlive = true });
            AddComponent(entt, new LocalTransform { Position = script.transform.position, Rotation = script.transform.rotation });
        }
    }
}

public partial struct PlayerRotation : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        Camera cam = GameObject.FindAnyObjectByType<PlayerScript>().GetComponentInChildren<Camera>();

        // Player doesn't need to schedule job as it will only be one instance of it so we run the player on the main thread 
        foreach (var (transform, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerData>>()){
            Vector3 target = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, cam.transform.position.z - transform.ValueRW.Position.z, Input.mousePosition.y));
            //Debug.Log(target);
            target.y = transform.ValueRO.Position.y;
            transform.ValueRW.Rotation = Quaternion.LookRotation(target, Vector3.up);
        }
    }
}

//[BurstCompile]
[UpdateAfter(typeof(PlayerRotation))]
public partial struct PlayerMove : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerData>();
    }

    //[BurstCompile]
    public void OnUpdate( ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        // Handled the same as PlayerRotation
        foreach (var (transform, data, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<CharacterData>, RefRO<PlayerData>>())
        {
            float z = Input.GetAxis("Vertical");
            float x = Input.GetAxis("Horizontal");

            Vector3 newPosition = transform.ValueRO.Position;

            Vector3 forward = transform.ValueRO.Forward();
            Vector3 right   = transform.ValueRO.Right();

            newPosition += (forward * data.ValueRO.m_moveSpeed * dt) * z; 
            newPosition += (right   * data.ValueRO.m_moveSpeed * dt) * x; 

            transform.ValueRW.Position = newPosition;
        }
    }
}


