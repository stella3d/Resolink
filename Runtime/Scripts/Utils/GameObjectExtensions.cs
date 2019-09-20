using UnityEngine;

namespace Resolink
{
    public static class GameObjectExtensions
    {
        public static void AddComponentIfAbsent<T>(this GameObject go)
            where T: Component
        {
            if (go.GetComponent<T>() == null)
                go.AddComponent<T>();
        }
        
        public static void AddComponentIfAbsent<T>(this Component component)
            where T: Component
        {
            if (component.gameObject.GetComponent<T>() == null)
                component.gameObject.AddComponent<T>();
        }
        
        public static void RemoveComponent<T>(this GameObject go)
            where T: Component
        {
            var component = go.GetComponent<T>();
            if (component != null)
                component.SafeDestroy();
        }

        public static void SafeDestroy(this Object obj)
        {
            if(Application.isPlaying)
                Object.Destroy(obj);
            else
                Object.DestroyImmediate(obj);
        }
    }
}