using System;
using btcp.ECS.helpers;
using btcp.ECS.utils;

namespace btcp.ECS.core
{
    public class ECSSystem
    {
        private string SYSTEM_LOG_PREFIX { get { return "[" + GetType().Name.ToString() + "] "; } }

        private ECSQueryManager m_queryManager;
        private ECSEntityManager m_entityManager;
        internal void Provide(ECSQueryManager queryManager, ECSEntityManager entityManager)
        {
            m_queryManager = queryManager;
            m_entityManager = entityManager;
        }

        protected int[] GetEntitiesWithComponents(params Type[] args)
        {
            return m_queryManager.GetEntitiesWithComponents(args);
        }

        protected T GetComponent<T>(int entityID) where T : ECSComponent
        {
            return m_queryManager.GetComponent<T>(entityID);
        }

        protected bool IsEntityValid(int entityID)
        {
            return m_queryManager.IsEntityValid(entityID);
        }

        protected Entity CreateEntity()
        {
            return m_entityManager.CreateEntity();
        }

        protected Entity CreateEntity(string archetype)
        {
            return m_entityManager.CreateEntity(archetype);
        }
        protected void DestroyEntity(int entityID)
        {
            m_entityManager.DestroyEntity(entityID);
        }

        protected bool SafeHasComponent<T>(int entityID) where T : ECSComponent
        {
            return m_queryManager.SafeHasComponent<T>(entityID);
        }

        internal virtual void Update()
        {
        }

        internal virtual void LateUpdate()
        {
        }

        internal virtual void FixedUpdate()
        {
        }


        protected void Log(string v)
        {
            ECSDebug.Log(SYSTEM_LOG_PREFIX + v);
        }

        protected void LogWarning(string v)
        {
            ECSDebug.LogWarning(SYSTEM_LOG_PREFIX + v);
        }

        protected void LogError(string v)
        {
            ECSDebug.LogError(SYSTEM_LOG_PREFIX + v);
        }

        protected void LogForce(string v)
        {
            ECSDebug.LogForce(v);
        }

        protected void Assert(bool condition, string v)
        {
            ECSDebug.Assert(condition, SYSTEM_LOG_PREFIX + v);
        }
    }
}