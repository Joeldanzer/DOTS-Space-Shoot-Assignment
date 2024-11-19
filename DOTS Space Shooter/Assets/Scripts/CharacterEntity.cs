using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    [SerializeField] protected float m_health;
    [SerializeField] protected float m_moveSpeed;

    protected EntityManager m_manager;
    protected Entity        m_entity;

    private void Awake()
    {
        m_manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Initialize();
    }

    protected virtual void Initialize() { }
}
