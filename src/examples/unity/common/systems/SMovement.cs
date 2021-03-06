using System;
using btcp.ECS.core;
using btcp.ECS.examples.unity.common.components;
using UnityEngine;

namespace btcp.ECS.examples.unity.common.systems
{
    public class SMovement : ECSSystem
    {

        internal override void FixedUpdate()
        {
            int[] entities = GetEntitiesWithComponents(typeof(CMovement), typeof(CTransform));

            foreach (int eID in entities)
            {
                CMovement cMovement = GetComponent<CMovement>(eID);
                CTransform cTransform = GetComponent<CTransform>(eID);

                if (HasComponent<CRigidbody>(eID))
                {
                    CRigidbody cRigidbody = GetComponent<CRigidbody>(eID);
                    if(cRigidbody.RigidBody.isKinematic == false)
                    {
                    cRigidbody.RigidBody.AddForce(cMovement.VelocityX, cMovement.VelocityY, cMovement.VelocityZ, ForceMode.VelocityChange);
                    }else{
                        Vector3 newPos = cRigidbody.RigidBody.position + new Vector3(cMovement.VelocityX, cMovement.VelocityY, cMovement.VelocityZ);
                        cRigidbody.RigidBody.MovePosition(newPos);
                    }
                }
                else
                {
                   cTransform.GameObject.transform.Translate(cMovement.VelocityX, cMovement.VelocityY, cMovement.VelocityZ);
                }

                cMovement.VelocityX = 0;
                cMovement.VelocityY = 0;
                cMovement.VelocityZ = 0;
            }
        }
    }
}