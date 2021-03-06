﻿
using System;
using System.Collections.Generic;
using btcp.ECS.utils;

namespace btcp.ECS.core
{

    public class ECSSystemManager
    {

        private ECSQueryManager m_queryManager;
        private ECSEntityManager m_entityManager;
        private ECSComponentManager m_componentManager;
        private List<Type> m_systemIdentifiers;
        private Bag<ECSSystem> m_systems;
        private List<ECSSystem> m_sortedSystems;

        public ECSSystemManager()
        {
            m_systemIdentifiers = new List<Type>();
            m_systems = new Bag<ECSSystem>();
            m_sortedSystems = new List<ECSSystem>();
        }

        public void Initialize(ECSQueryManager queryManager, ECSEntityManager entityManager, ECSComponentManager componentManager)
        {
            m_queryManager = queryManager;
            m_entityManager = entityManager;
            m_componentManager = componentManager;
        }

        public void CreateSystem(ECSSystem system)
        {
            m_entityManager.OnEntityCreated += system.OnEntityCreated;
            m_entityManager.OnEntityDestroyedPre += system.OnEntityDestroyedPre;
            m_entityManager.OnEntityDestroyedPost += system.OnEntityDestroyedPost;
            m_componentManager.OnComponentAdded += system.OnComponentAdded;
            m_componentManager.OnComponentRemovedPre += system.OnComponentRemovedPre;
            m_componentManager.OnComponentRemovedPost += system.OnComponentRemovedPost;

            m_systems.Set(SafeGetSystemID(system.GetType()), system);
            system.Provide(m_queryManager, m_entityManager);
            SortSystems();
        }

        public void RemoveSystem<T>() where T : ECSSystem
        {
            int systemID = GetSystemID(typeof(T));
            ECSSystem system = m_systems.Get(systemID);
            m_entityManager.OnEntityCreated -= system.OnEntityCreated;
            m_entityManager.OnEntityDestroyedPre -= system.OnEntityDestroyedPre;
            m_entityManager.OnEntityDestroyedPost -= system.OnEntityDestroyedPost;
            m_componentManager.OnComponentAdded -= system.OnComponentAdded;
            m_componentManager.OnComponentRemovedPre -= system.OnComponentRemovedPre;
            m_componentManager.OnComponentRemovedPost -= system.OnComponentRemovedPost;
            m_systems.Set(systemID, null);
            SortSystems();
        }

        internal void Kill()
        {
            m_systems.Clear();
            m_systemIdentifiers.Clear();
            m_sortedSystems.Clear();
        }

        internal void SetSystemPriority<T>(int v) where T : ECSSystem
        {
            GetSystem<T>().UpdatePriority = v;
            SortSystems();
        }

        public T GetSystem<T>() where T : ECSSystem
        {
            return m_systems.Get(GetSystemID(typeof(T))) as T;
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
            foreach (ECSSystem system in m_sortedSystems)
            {
                if (system.IsEnabled() == false)
                {
                    break;
                }

                system.Update();
            }
        }

        public void FixedUpdate()
        {
            foreach (ECSSystem system in m_sortedSystems)
            {
                if (system.IsEnabled() == false)
                {
                    break;
                }

                system.FixedUpdate();
            }
        }

        public void LateUpdate()
        {
            foreach (ECSSystem system in m_sortedSystems)
            {
                if (system.IsEnabled() == false)
                {
                    break;
                }

                system.LateUpdate();
            }
        }

        private void SortSystems()
        {
            m_sortedSystems = new List<ECSSystem>();

            Bag<ECSSystem> bag = m_systems.Clone();
            bag.ResizeToFit();

            List<ECSSystem> systems = new List<ECSSystem>(bag.GetAll());
            List<ECSSystem> disabledSystems = new List<ECSSystem>();

            for (int i = systems.Count - 1; i >= 0; i--)
            {
                if (systems[i].IsEnabled() == false)
                {
                    disabledSystems.Add(systems[i]);
                    systems.RemoveAt(i);
                }
            }

            int highestPriority = int.MinValue;
            ECSSystem highestPriotitySystem = null;

            while (systems.Count > 0)
            {

                highestPriority = int.MinValue;
                highestPriotitySystem = null;

                foreach (ECSSystem system in systems)
                {
                    if (system.UpdatePriority > highestPriority)
                    {
                        highestPriority = system.UpdatePriority;
                        highestPriotitySystem = system;
                    }
                }

                systems.Remove(highestPriotitySystem);
                m_sortedSystems.Add(highestPriotitySystem);
            }

            foreach (ECSSystem disabledSystem in disabledSystems)
            {
                m_sortedSystems.Add(disabledSystem);
            }

            for (int i = 0; i < m_sortedSystems.Count; i++)
            {
                ECSDebug.Log(i + " - " + m_sortedSystems[i]);
            }
        }


        public void EnableSystem<T>() where T : ECSSystem
        {
            T system = GetSystem<T>();

            if (system.IsEnabled())
            {
                return;
            }

            system.Enable();
            SortSystems();
        }

        public void DisableSystem<T>() where T : ECSSystem
        {
            T system = GetSystem<T>();

            if (system.IsEnabled() == false)
            {
                return;
            }

            system.Disable();
            SortSystems();
        }
    }
}