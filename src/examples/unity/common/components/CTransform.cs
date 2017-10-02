using btcp.ECS.core;
using UnityEngine;

namespace btcp.ECS.examples.unity.common.components
{
    public class CTransform : ECSComponent
    {
        public float X;
        public float Y;
        public float Z;

        public string Name;
        public GameObject GameObject;

        public int LayerID;

        ///The path of the prefab (must be located in Resources folder)
        public string PrefabID;

        public CTransform()
        {

        }

        public CTransform(GameObject gameObject, string name, float x, float y, float z)
        {
            Name = name;
            X = x;
            Y = y;
            Z = z;
        }
    }
}