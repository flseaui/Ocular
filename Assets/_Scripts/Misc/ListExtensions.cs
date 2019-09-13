using System.Collections.Generic;
using UnityEngine;

namespace Misc
{
    public static class ListExtensions
    {
        public static T RandomElement<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count - 1)];
        }
    }
}