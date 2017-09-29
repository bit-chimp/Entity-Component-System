using System;
using System.Collections.Generic;
using Assets.Scripts.Utilities.MessageHandler;
using btcp.ECS.utils;

namespace btcp.ECS.core
{
    public class ECSComponentManager : IMessageListener
    {

        private List<Type> m_componentIdentifiers;
        private Dictionary<int, Bag<ECSComponent>> m_entityComponents;

        public ECSComponentManager()
        {
            m_componentIdentifiers = new List<Type>();
            m_entityComponents = new Dictionary<int, Bag<ECSComponent>>();

            MessageDispatcher.Instance.BindListener(this, (int)MessageID.EVENT_ECS_ENTITY_DESTROYED);
        }

        public ECSComponent AddComponent(Entity entity, ECSComponent component)
        {
            return AddComponent(entity.EntityID, component);
        }

        public ECSComponent AddComponent(int entityID, ECSComponent component)
        {
            SafeGetComponentBag(entityID).Set(SafeGetComponentID(component.GetType()), component);

            Message msg = new Message((int)MessageID.EVENT_ECS_COMPONENT_ADDED);
            msg.SetArgInt("entity_id", entityID);
            msg.SetArgInt("component_id", GetComponentID(component.GetType()));
            MessageDispatcher.Instance.QueueMessage(msg);

            return component;
        }

        private Bag<ECSComponent> SafeGetComponentBag(int entityID)
        {
            if (m_entityComponents.ContainsKey(entityID) == false)
            {
                m_entityComponents.Add(entityID, new Bag<ECSComponent>());
            }

            return GetComponentBag(entityID);
        }

        private Bag<ECSComponent> GetComponentBag(int entityID)
        {
            return m_entityComponents[entityID];
        }

        private bool HasComponentBag(int entityID)
        {
            return m_entityComponents.ContainsKey(entityID);
        }

        public T SafeGetComponent<T>(int entityID) where T : ECSComponent
        {
            return SafeGetComponentBag(entityID).Get(SafeGetComponentID(typeof(T))) as T;
        }

        public T GetComponent<T>(int entityID) where T : ECSComponent
        {
            return GetComponentBag(entityID).Get(GetComponentID(typeof(T))) as T;
        }

        public ECSComponent[] GetComponents(Entity entity)
        {
            return SafeGetComponentBag(entity.EntityID).GetAll();
        }


        public int SafeGetComponentID(Type type)
        {
            if (m_componentIdentifiers.IndexOf(type) == -1)
            {
                m_componentIdentifiers.Add(type);
            }

            return GetComponentID(type);
        }

        public int GetComponentID(Type type)
        {
            return m_componentIdentifiers.IndexOf(type);
        }

        public Entity RemoveComponent<T>(Entity entity) where T : ECSComponent
        {
            ECSDebug.Assert(entity != null, "Entity does not exist");

            Message msg = new Message((int)MessageID.EVENT_ECS_COMPONENT_REMOVED);
            msg.SetArgInt("entity_id", entity.EntityID);
            msg.SetArgInt("component_id", GetComponentID(typeof(T)));
            MessageDispatcher.Instance.QueueMessage(msg);

            GetComponentBag(entity.EntityID).Set(GetComponentID(typeof(T)), null);

            return entity;
        }

        private void RemoveAllComponents(int v)
        {
            Bag<ECSComponent> componentBag = GetComponentBag(v);
            componentBag.Clear();
            m_entityComponents.Remove(v);
        }

        internal bool HasComponents(int id, Type[] args)
        {
            if (HasComponentBag(id) == false)
            {
                ECSDebug.LogWarning("Entity " + id + " does not have any components!");
                return false;
            }

            Bag<ECSComponent> bag = GetComponentBag(id);

            foreach (Type type in args)
            {
                if (GetComponentID(type) == -1)
                {
                    ECSDebug.LogWarning("Component not yet registered " + type.Name.ToString());
                    return false;
                }

                if (bag.Get(GetComponentID(type)) == null)
                {
                    ECSDebug.LogWarning("Entity " + id + " does not have component " + type.Name.ToString() + " (Component ID : " + GetComponentID(type) + ")");
                    return false;
                }
            }

            return true;
        }

        public void ReceiveMessage(Message message)
        {
            if (message.MessageID == (int)MessageID.EVENT_ECS_ENTITY_DESTROYED)
            {
                RemoveAllComponents(message.GetArgInt("entity_id"));
            }
        }

    }
}