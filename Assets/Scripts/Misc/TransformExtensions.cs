using UnityEngine;

namespace Misc {
    public static class TransformExtensions
    {
        public static bool HasComponent<T>(this Transform transform) => transform.GetComponent<T>() != null;

        public static bool HasComponent<T>(this Transform transform, out T component)
        {
            component = transform.GetComponent<T>();
            return component != null;
        }

        public static bool ParentHasComponent<T>(this Transform transform) => transform.GetComponentInParent<T>() != null;

        public static bool ParentHasComponent<T>(this Transform transform, out T component)
        {
            component = transform.GetComponentInParent<T>();
            return component != null;
        }
    }
}