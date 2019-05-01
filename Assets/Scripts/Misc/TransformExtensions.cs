using System.Collections.Generic;
using UnityEngine;

namespace Misc
{
    public static class TransformExtensions
    {
        public static bool HasComponent<T>(this Transform transform)
        {
            return transform.GetComponent<T>() != null;
        }

        public static bool HasComponent<T>(this Transform transform, out T component)
        {
            component = transform.GetComponent<T>();
            return component != null;
        }

        public static bool ParentHasComponent<T>(this Transform transform)
        {
            return transform.GetComponentInParent<T>() != null;
        }

        public static bool ParentHasComponent<T>(this Transform transform, out T component)
        {
            component = transform.GetComponentInParent<T>();
            return component != null;
        }

        public static bool ChildrenHaveComponents<T>(this Transform transform, out IEnumerable<T> components)
        {
            var temp = new List<T>();
            foreach (Transform tr in transform)
                if (tr.HasComponent<T>(out var c))
                    temp.Add(c);

            components = temp;
            return temp.Count > 0;
        }

        public static bool ChildHasComponent<T>(this Transform transform, out T component)
        {
            foreach (Transform tr in transform)
                if (tr.HasComponent<T>(out var c))
                {
                    component = c;
                    return true;
                }

            component = default;
            return false;
        }

        public static T GetComponentInChildWithTag<T>(this Transform parent, string tag) where T : Component
        {
            foreach (Transform tr in parent)
                if (tr.CompareTag(tag))
                    return tr.GetComponent<T>();
            return null;
        }

        public static IEnumerable<T> GetComponentsInChildrenWithTag<T>(this Transform parent, string tag)
            where T : Component
        {
            var temp = new List<T>();
            foreach (Transform tr in parent)
                if (tr.CompareTag(tag))
                    temp.Add(tr.GetComponent<T>());

            return temp;
        }
    }
}