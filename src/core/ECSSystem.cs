using System;
using btcp.ECS.utils;

namespace btcp.ECS.core
{
    public class ECSSystem
    {
        private string SYSTEM_LOG_PREFIX { get { return "[" + GetType().Name.ToString() + "] "; } }
        private bool m_isEnabled = true;
        private ECSQueryManager m_queryManager;
        private ECSEntityManager m_entityManager;

        public int UpdatePriority { get; set; }
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


        protected bool HasComponent<T>(int entityID) where T : ECSComponent
        {
            return m_queryManager.HasComponent<T>(entityID);
        }

        protected bool IsEntityValid(int entityID)
        {
            return m_queryManager.IsEntityValid(entityID);
        }

        protected ECSEntity CreateEntity()
        {
            return m_entityManager.CreateEntity();
        }

        protected ECSEntity CreateEntity(string archetype)
        {
            return m_entityManager.CreateEntity(archetype);
        }

        protected void DestroyEntity(int entityID)
        {
            m_entityManager.DestroyEntity(entityID);
        }


        public virtual void OnEntityCreated(ECSEntity entity)
        {
        }

        public virtual void OnEntityDestroyedPre(ECSEntity entity)
        {
        }

        public virtual void OnEntityDestroyedPost(ECSEntity entity)
        {
        }

        public virtual void OnComponentAdded(int entityID, ECSComponent component)
        {
        }

        public virtual void OnComponentRemovedPre(int entityID, ECSComponent component)
        {
        }

        public virtual void OnComponentRemovedPost(int entityID, ECSComponent component)
        {
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


        protected void Log(object v)
        {
            ECSDebug.Log(SYSTEM_LOG_PREFIX + v);
        }

        protected void LogWarning(object v)
        {
            ECSDebug.LogWarning(SYSTEM_LOG_PREFIX + v);
        }

        protected void LogError(object v)
        {
            ECSDebug.LogError(SYSTEM_LOG_PREFIX + v);
        }

        protected void LogForce(object v)
        {
            ECSDebug.LogForce(v);
        }

        protected void Assert(bool condition, object v)
        {
            ECSDebug.Assert(condition, SYSTEM_LOG_PREFIX + v);
        }

        internal bool IsEnabled()
        {
            return m_isEnabled;
        }

        internal void Enable()
        {
            m_isEnabled = true;
        }

        internal void Disable()
        {
            m_isEnabled = false;
        }
    }
}