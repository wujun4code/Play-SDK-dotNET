using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeanCloud
{
    internal static class EventNotification
    {
        public static HashSet<GameObject> TargetGameObjects;

        public static Type SendMonoMessageTargetType = typeof(MonoBehaviour);

        public static void CacheSendMonoMessageTargets(Type type)
        {
            if (type == null) type = SendMonoMessageTargetType;
            TargetGameObjects = FindGameObjectsWithComponent(type);
        }

        public static HashSet<GameObject> FindGameObjectsWithComponent(Type type)
        {
            HashSet<GameObject> objectsWithComponent = new HashSet<GameObject>();

            Component[] targetComponents = (Component[])GameObject.FindObjectsOfType(type);
            for (int index = 0; index < targetComponents.Length; index++)
            {
                if (targetComponents[index] != null)
                {
                    objectsWithComponent.Add(targetComponents[index].gameObject);
                }
            }

            return objectsWithComponent;
        }

        public static void NotifyUnityGameObjects(string methodString, params object[] parameters)
        {
            HashSet<GameObject> objectsToCall;
            if (EventNotification.TargetGameObjects != null)
            {
                objectsToCall = EventNotification.TargetGameObjects;
            }
            else
            {
                objectsToCall = EventNotification.FindGameObjectsWithComponent(EventNotification.SendMonoMessageTargetType);
            }

            string methodName = methodString.ToString();
            object callParameter = (parameters != null && parameters.Length == 1) ? parameters[0] : parameters;
            foreach (GameObject gameObject in objectsToCall)
            {
                if (gameObject != null)
                {
                    gameObject.SendMessage(methodName, callParameter, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
