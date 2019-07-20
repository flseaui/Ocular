using Battlehub.RTCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityEngine.Battlehub.SL2
{ }
namespace Battlehub.RTSL
{
    public class CustomImplementationAttribute : Attribute
    {
    }

    [ProtoContract]
    public class IntArray
    {
        [ProtoMember(1)]
        public int[] Array;
    }

    public interface IPersistentSurrogate
    {
        void ReadFrom(object obj);

        object WriteTo(object obj);

        void GetDeps(GetDepsContext context);

        void GetDepsFrom(object obj, GetDepsFromContext context);
    }

    public class GetDepsContext
    {
        public readonly HashSet<long> Dependencies = new HashSet<long>();
        public readonly HashSet<object> VisitedObjects = new HashSet<object>();

        public void Clear()
        {
            Dependencies.Clear();
            VisitedObjects.Clear();
        }
    }

    public class GetDepsFromContext
    {
        public readonly HashSet<object> Dependencies = new HashSet<object>();
        public readonly HashSet<object> VisitedObjects = new HashSet<object>();

        public void Clear()
        {
            Dependencies.Clear();
            VisitedObjects.Clear();
        }
    }

    public abstract class PersistentSurrogate : IPersistentSurrogate
    {
        protected readonly IAssetDB m_assetDB;
        protected PersistentSurrogate()
        {
            m_assetDB = IOC.Resolve<IAssetDB>();
        }

        protected virtual void ReadFromImpl(object obj) { }
        protected virtual object WriteToImpl(object obj) { return obj; }
        protected virtual void GetDepsImpl(GetDepsContext context) { }
        protected virtual void GetDepsFromImpl(object obj, GetDepsFromContext context) { }

        public virtual bool CanInstantiate(Type type)
        {
            return false;
        }

        public virtual object Instantiate(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public virtual void ReadFrom(object obj)
        {
            if(obj == null)
            {
                return;
            }
            ReadFromImpl(obj);
        }

        public virtual object WriteTo(object obj)
        {
            if(obj == null)
            {
                return null;
            }
            obj = WriteToImpl(obj);
            return obj;
        }

        public virtual void GetDeps(GetDepsContext context)
        {
            if (context.VisitedObjects.Contains(this))
            {
                return;
            }
            context.VisitedObjects.Add(this);
            GetDepsImpl(context);
        }

        public virtual void GetDepsFrom(object obj, GetDepsFromContext context)
        {
            if (context.VisitedObjects.Contains(obj))
            {
                return;
            }
            context.VisitedObjects.Add(obj);
            GetDepsFromImpl(obj, context);
        }

        protected void WriteSurrogateTo(IPersistentSurrogate from, object to)
        {
            if(from == null)
            {
                return;
            }

            from.WriteTo(to);
        }

        protected T ReadSurrogateFrom<T>(object obj) where T : IPersistentSurrogate, new()
        {
            T surrogate = new T();
            surrogate.ReadFrom(obj);
            return surrogate;
        }

        protected void AddDep(long depenency, GetDepsContext context)
        {
            if (depenency > 0 && !m_assetDB.IsNullID(depenency) && !context.Dependencies.Contains(depenency))
            {
                context.Dependencies.Add(depenency);
            }
        }

        protected void AddDep(long[] depenencies, GetDepsContext context)
        {
            if(depenencies == null)
            {
                return;
            }

            for (int i = 0; i < depenencies.Length; ++i)
            {
                AddDep(depenencies[i], context);
            }
        }

        protected void AddDep(object obj, GetDepsFromContext context)
        {
            if (obj != null && !context.Dependencies.Contains(obj))
            {
                context.Dependencies.Add(obj);
            }
        }

        protected void AddDep<T>(T[] dependencies, GetDepsFromContext context)
        {
            if(dependencies == null)
            {
                return;
            }
            for (int i = 0; i < dependencies.Length; ++i)
            {
                AddDep(dependencies[i], context);
            }
        }

        protected void AddDep<T>(List<T> dependencies, GetDepsFromContext context)
        {
            if (dependencies == null)
            {
                return;
            }
            for (int i = 0; i < dependencies.Count; ++i)
            {
                AddDep(dependencies[i], context);
            }
        }

        protected void AddSurrogateDeps(PersistentSurrogate surrogate, GetDepsContext context)
        {
            if(surrogate == null)
            {
                return;
            }

            surrogate.GetDeps(context);
        }

        protected void AddSurrogateDeps<T>(T[] surrogateArray, GetDepsContext context) where T : PersistentSurrogate
        {
            if(surrogateArray == null)
            {
                return;
            }
            for (int i = 0; i < surrogateArray.Length; ++i)
            {
                PersistentSurrogate surrogate = surrogateArray[i];
                surrogate.GetDeps(context);
            }
        }

        protected void AddSurrogateDeps<T>(List<T> surrogateList, GetDepsContext context) where T : PersistentSurrogate
        {
            if(surrogateList == null)
            {
                return;
            }
            for (int i = 0; i < surrogateList.Count; ++i)
            {
                PersistentSurrogate surrogate = surrogateList[i];
                surrogate.GetDeps(context);
            }
        }

        protected void AddSurrogateDeps<T>(T obj, Func<T, PersistentSurrogate> convert, GetDepsContext context)
        {
            if (obj != null)
            {
                PersistentSurrogate surrogate = convert(obj);
                surrogate.GetDeps(context);
            }
        }

        protected void AddSurrogateDeps<T>(T[] objArray, Func<T, PersistentSurrogate> convert, GetDepsContext context)
        {
            if (objArray == null)
            {
                return;
            }
            for (int i = 0; i < objArray.Length; ++i)
            {
                T obj = objArray[i];
                if (obj != null)
                {
                    PersistentSurrogate surrogate = convert(obj);
                    surrogate.GetDeps(context);
                }
            }
        }


        protected void AddSurrogateDeps<T>(T obj, Func<T, PersistentSurrogate> convert, GetDepsFromContext context)
        {
            if (obj != null)
            {
                PersistentSurrogate surrogate = convert(obj);
                surrogate.GetDepsFrom(obj, context);
            }
        }

        protected void AddSurrogateDeps<T>(T[] objArray, Func<T, PersistentSurrogate> convert, GetDepsFromContext context)
        {
            if(objArray == null)
            {
                return;
            }
            for (int i = 0; i < objArray.Length; ++i)
            {
                T obj = objArray[i];
                if (obj != null)
                {
                    PersistentSurrogate surrogate = convert(obj);
                    surrogate.GetDepsFrom(obj, context);
                }
            }
        }

        protected void AddSurrogateDeps<T>(List<T> objList, Func<T, PersistentSurrogate> convert, GetDepsFromContext context)
        {
            if(objList == null)
            {
                return;
            }
            for (int i = 0; i < objList.Count; ++i)
            {
                T obj = objList[i];
                if (obj != null)
                {
                    PersistentSurrogate surrogate = convert(obj);
                    surrogate.GetDepsFrom(obj, context);
                }
            }
        }

        public List<T> Assign<V, T>(List<V> list, Func<V, T> convert)
        {
            if (list == null)
            {
                return null;
            }

            List<T> result = new List<T>(list.Count);
            for (int i = 0; i < list.Count; ++i)
            {
                result.Add(convert(list[i]));
            }
            return result;
        }

        public T[] Assign<V, T>(V[] arr, Func<V, T> convert)
        {
            if (arr == null)
            {
                return null;
            }

            T[] result = new T[arr.Length];
            for (int i = 0; i < arr.Length; ++i)
            {
                result[i] = convert(arr[i]);
            }
            return result;
        }


        protected long ToID(UnityObject uo)
        {
            return m_assetDB.ToID(uo);
        }

        protected long[] ToID(UnityObject[] uo)
        {
            return m_assetDB.ToID(uo);
        }

        protected long[] ToID<T>(List<T> uo) where T : UnityObject
        {
            return m_assetDB.ToID(uo);
        }

        protected T FromID<T>(long id, T fallback = null) where T : UnityObject
        {
            if(m_assetDB.IsNullID(id))
            {
                return default(T);
            }

            T value = m_assetDB.FromID<T>(id);
            if(value == default(T))
            {
                return fallback;
            }

            return value;
        }

        protected T[] FromID<T>(long[] id, T[] fallback = null) where T : UnityObject
        {
            if (id == null)
            {
                return null;
            }

            T[] objs = new T[id.Length];
            for (int i = 0; i < id.Length; ++i)
            {
                if(fallback != null && i < fallback.Length)
                {
                    objs[i] = FromID(id[i], fallback[i]);
                }
                else
                { 
                    objs[i] = FromID<T>(id[i]);
                }
            }
            return objs;
        }

        protected List<T> FromID<T>(long[] id, List<T> fallback = null) where T : UnityObject
        {
            if (id == null)
            {
                return null;
            }

            List<T> objs = new List<T>();
            for (int i = 0; i < id.Length; ++i)
            {
                if (fallback != null && i < fallback.Count)
                {
                    objs.Add(FromID(id[i], fallback[i]));
                }
                else
                {
                    objs.Add(FromID<T>(id[i]));
                }
            }
            return objs;
        }

        protected T GetPrivate<T>(object obj, string fieldName)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if(fieldInfo == null)
            {
                return default(T);
            }
            object val = fieldInfo.GetValue(obj);
            if(val is T)
            {
                return (T)val;
            }
            return default(T);
        }

        protected void SetPrivate<T>(object obj, string fieldName, T value)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (fieldInfo == null)
            {
                return;
            }

            if(!fieldInfo.FieldType.IsAssignableFrom(typeof(T)))
            {
                return;
            }

            fieldInfo.SetValue(obj, value);
        }

    }
}

