﻿
using System;
using System.Collections.Generic;
using btcp.ECS.helpers;
using btcp.ECS.utils;

namespace btcp.ECS.core
{

    public class ECSSystemManager
    {

        private ECSQueryManager m_queryManager;
        private ECSEntityManager m_entityManager;

        private List<Type> m_systemIdentifiers;
        private Bag<ECSSystem> m_systems;

        public ECSSystemManager()
        {
            m_systemIdentifiers = new List<Type>();
            m_systems = new Bag<ECSSystem>();
        }

        public void Initialize(ECSQueryManager queryManager, ECSEntityManager entityManager)
        {
            m_queryManager = queryManager;
            m_entityManager = entityManager;
        }

        public void CreateSystem(ECSSystem system)
        {
            m_systems.Add(system);
            system.Provide(m_queryManager, m_entityManager);
        }

        public void RemoveSystem<T>() where T : ECSSystem
        {
            m_systems.Set(GetSystemID(typeof(T)), null);
        }


        private int SafeGetSystemID(Type systemType)
        {
            if (m_systemIdentifiers.IndexOf(systemType) == -1)
            {
                m_systemIdentifiers.Add(systemType);
            }

            return GetSystemID(systemType);
        }

        public int GetSystemID(Type systemType)
        {
            return m_systemIdentifiers.IndexOf(systemType);
        }

        public void Update()
        {
            foreach (ECSSystem system in m_systems)
            {
                system.Update();
            }
        }

        public void FixedUpdate()
        {
            foreach (ECSSystem system in m_systems)
            {
                system.FixedUpdate();
            }
        }

        public void LateUpdate()
        {
            foreach (ECSSystem system in m_systems)
            {
                system.LateUpdate();
            }
        }
    }
}