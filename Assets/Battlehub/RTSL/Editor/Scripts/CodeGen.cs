using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityObject = UnityEngine.Object;

namespace Battlehub.RTSL
{
    /// <summary>
    /// This class is responsible for code generation of persistent objects (surrogates) 
    /// </summary>
    public class CodeGen
    {
        /// <summary>
        /// Automatically generated fields have ProtoMember tag offset = 256. 1 - 256 is reserved for user defined fields.
        /// User defined fields should be located in auto-generated partial class.
        /// </summary>
        private const int AutoFieldTagOffset = 256;

        /// <summary>
        /// Subclass offset which is used in TypeModel creator code. 
        /// (1024 value means, that there is 1024 - 256 - 1 = 767 slots available for auto-generated fields
        /// </summary>
        private const int SubclassOffset = 1024;

        /// <summary>
        /// For text formatting
        /// </summary>
        private static readonly string BR = Environment.NewLine;
        private static readonly string END = BR + BR;
        private static readonly string TAB = "    ";
        private static readonly string TAB2 = "        ";
        private static readonly string TAB3 = "            ";
        private static readonly string SEMICOLON = ";";

        /// <summary>
        /// Default namespaces which will be included in all auto-generated classes
        /// </summary>
        private static string[] DefaultNamespaces =
        {
            "System.Collections.Generic",
            "ProtoBuf",
            "Battlehub.RTSL"
        };

        //Templates
        private static readonly string PersistentClassTemplate =
            "{0}" + BR +
            "using UnityObject = UnityEngine.Object;" + BR +
            "namespace {1}" + BR +
            "{{" + BR +
            "    [ProtoContract]" + BR +
            "    public partial class {2} : {3}" + BR +
            "    {{" + BR +
            "        {4}" +
            "    }}" + BR +
            "}}" + END;

        private static readonly string UserDefinedClassTemplate =
           "#if !RTSL_MAINTENANCE" + BR +
           "{0}" + BR +
           "namespace {1}" + BR +
           "{{" + BR +
           "    [CustomImplementation]" + BR +
           "    public partial class {2}" + BR +
           "    {{" +
           "        {3}" + BR +
           "    }}" + BR +
           "}}" + BR +
           "#endif" + END;

        private static readonly string UserDefinedEmptyClassTemplate =
            "#if !RTSL_MAINTENANCE" + BR +
            "{0}" + BR +
            "namespace {1}" + BR +
            "{{" + BR +
            "    [CustomImplementation]" + BR +
            "    public partial class {2}" + BR +
            "    {{" + BR +
            "        /*" + BR +
            "        public override void ReadFrom(object obj)" + BR +
            "        {{" + BR +
            "            base.ReadFrom(obj);" + BR +
            "        }}" + BR + BR +
            "        public override object WriteTo(object obj)" + BR +
            "        {{" + BR +
            "            return base.WriteTo(obj);" + BR +
            "        }}" + BR + BR +
            "        public override void GetDeps(GetDepsContext context)" + BR +
            "        {{" + BR +
            "            base.GetDeps(context);" + BR +
            "        }}" + BR + BR +
            "        public override void GetDepsFrom(object obj, GetDepsFromContext context)" + BR +
            "        {{" + BR +
            "            base.GetDepsFrom(obj, context);" + BR +
            "        }}" + BR +
            "        */" + BR +
            "    }}" + BR +
            "}}" + BR +
            "#endif" + END;

        private static readonly string FieldTemplate =
            "[ProtoMember({0})]" + BR + TAB2 +
            "public {1} {2};" + END + TAB2;

        private static readonly string ReadFromMethodTemplate =
            "protected override void ReadFromImpl(object obj)" + BR + TAB2 +
            "{{" + BR + TAB2 +
            "    base.ReadFromImpl(obj);" + BR + TAB2 +
            "    {1} uo = ({1})obj;" + BR + TAB2 +
            "{0}" +
            "}}" + BR;

        private static readonly string WriteToMethodTemplate =
            "protected override object WriteToImpl(object obj)" + BR + TAB2 +
            "{{" + BR + TAB2 +
            "    obj = base.WriteToImpl(obj);" + BR + TAB2 +
            "    {1} uo = ({1})obj;" + BR + TAB2 +
            "{0}" +
            "    return uo;" + BR + TAB2 +
            "}}" + BR;

        private static readonly string GetDepsMethodTemplate =
            "protected override void GetDepsImpl(GetDepsContext context)" + BR + TAB2 +
            "{{" + BR + TAB2 +
            "    base.GetDepsImpl(context);" + BR + TAB2 +
            "{0}" +
            "}}" + BR;

        private static readonly string GetDepsFromMethodTemplate =
            "protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)" + BR + TAB2 +
            "{{" + BR + TAB2 +
            "    base.GetDepsFromImpl(obj, context);" + BR + TAB2 +
            "    {1} uo = ({1})obj;" + BR + TAB2 +
            "{0}" +
            "}}" + BR;


        private static readonly string ImplicitOperatorsTemplate =
            "public static implicit operator {0}({1} surrogate)" + BR + TAB2 +
            "{{" + BR + TAB2 +
            "    if(surrogate == null) return default({0});" + BR + TAB2 +
            "    return ({0})surrogate.WriteTo(new {0}());" + BR + TAB2 +
            "}}" + BR + TAB2 +
            BR + TAB2 +
            "public static implicit operator {1}({0} obj)" + BR + TAB2 +
            "{{" + BR + TAB2 +
            "    {1} surrogate = new {1}();" + BR + TAB2 +
            "    surrogate.ReadFrom(obj);" + BR + TAB2 +
            "    return surrogate;" + BR + TAB2 +
            "}}" + BR;

        private static readonly string TypeModelCreatorTemplate =
            "using ProtoBuf.Meta;" + BR +
            "{0}" + BR +
            "using UnityObject = UnityEngine.Object;" + BR +
            "namespace Battlehub.RTSL" + BR +
            "{{" + BR +
            "   public static partial class TypeModelCreator" + BR +
            "   {{" + BR +
            "       static partial void RegisterAutoTypes(RuntimeTypeModel model)" + BR +
            "       {{" + BR +
            "            m_createDefaultTypeModel = false;" + BR +
            "            {1}" + BR +
            "       }}" + BR +
            "   }}" + BR +
            "}}" + BR +
            "{2}" + END;

        private static readonly string NamespaceDefinitionTemplate =
            "namespace {0} {{}}";

        private static readonly string AddTypeTemplate =
            "model.Add(typeof({0}), {1}){2}";

        private static readonly string AddSubtypeTemplate =
            ".AddSubType({1}, typeof({0}))";

        private static readonly string SetSerializationSurrogate =
            ".SetSurrogate(typeof({0}))";

        private static readonly string TypeMapTemplate =
            "{0}" + BR +
            "using UnityObject = UnityEngine.Object;" + BR +
            "namespace Battlehub.RTSL" + BR +
            "{{" + BR +
            "    public partial class TypeMap" + BR +
            "    {{" + BR +
            "        partial void RegisterAutoTypes()" + BR +
            "        {{" + BR +
            "            m_registerDefault = false;" + BR +
            "            {1}" +
            "        }}" + BR +
            "    }}" + BR +
            "}}" + END;

        
        
            

        private static readonly string AddToPersistentTypeTemplate = 
            "m_toPeristentType.Add(typeof({0}), typeof({1}));" + BR;
        private static readonly string AddToUnityTypeTemplate =
            "m_toUnityType.Add(typeof({0}), typeof({1}));" + BR;
        private static readonly string AddToTypeTemplate =
            "m_toType.Add(new System.Guid(\"{0}\"), typeof({1}));" + BR;
        private static readonly string AddToGuidTemplate =
            "m_toGuid.Add(typeof({0}), new System.Guid(\"{1}\"));" + BR;


        /// <summary>
        /// Short names for primitive types
        /// </summary>
        private static Dictionary<Type, string> m_primitiveNames = new Dictionary<Type, string>
        {
            { typeof(string), "string" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(short), "short" },
            { typeof(byte), "byte" },
            { typeof(ulong), "ulong" },
            { typeof(uint), "uint" },
            { typeof(ushort), "ushort" },
            { typeof(char), "char" },
            { typeof(object), "object" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(bool), "bool" },
            { typeof(string[]), "string[]" },
            { typeof(long[]), "long[]" },
            { typeof(int[]), "int[]" },
            { typeof(short[]), "short[]" },
            { typeof(byte[]), "byte[]" },
            { typeof(ulong[]), "ulong[]" },
            { typeof(uint[]), "uint[]" },
            { typeof(ushort[]), "ushort[]" },
            { typeof(char[]), "char[]" },
            { typeof(object[]), "object[]" },
            { typeof(float[]), "float[]" },
            { typeof(double[]), "double[]" },
            { typeof(bool[]), "bool[]" },
        };

        /// <summary>
        /// Get all public properties with getter and setter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            return GetAllProperties(type).Where(p => p.GetGetMethod() != null && p.GetSetMethod() != null).ToArray();
        }

        /// <summary>
        /// Get all public instance declared only properties (excluding indexers)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetAllProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => (!p.PropertyType.IsGenericType || IsGenericList(p.PropertyType)) && p.GetIndexParameters().Length == 0).ToArray();
        }

        /// <summary>
        /// Get all public instance declared only fields
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo[] GetFields(Type type)
        {
            if(type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                return type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Union(
                    type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(f => f.FieldType.IsPublic && f.GetCustomAttributes(typeof(SerializeField), true).Length > 0)).ToArray();
            }

            return type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToArray();
        }

        public static MethodInfo[] GetMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static bool IsGenericList(Type type)
        {
            bool isList = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
            return isList;
        }

        /// <summary>
        /// Get type which is not subclass of UnityObject and "suitable" to be persistent object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetSurrogateType(Type type)
        {
            if (type.IsArray)
            {
                type = type.GetElementType();
            }

            if(IsGenericList(type))
            {
                type = type.GetGenericArguments()[0];
            }

            if (!type.IsSubclassOf(typeof(UnityObject)) &&
                 type != typeof(UnityObject) &&
                !type.IsEnum &&
                !type.IsGenericType &&
                !type.IsArray &&
                !IsGenericList(type) &&
                !type.IsPrimitive &&
                (type.IsPublic || type.IsNestedPublic) &&
                (type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null) &&
                type != typeof(string))
            {
                return type;
            }
            return null;
        }

        public Type GetPersistentType(string fullTypeName)
        {
            string assemblyName = typeof(PersistentSurrogate).Assembly.FullName;
            string assemblyQualifiedName = Assembly.CreateQualifiedName(assemblyName, fullTypeName);
            Type persistentType = Type.GetType(assemblyQualifiedName);
            return persistentType;
        }


        /// <summary>
        /// Returns true if type has fields or properties referencing UnityObjects. Search is done recursively.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasDependencies(Type type)
        {
            return HasDependenciesRecursive(type, new HashSet<Type>());
        }

        private bool HasDependencies(Type type, HashSet<Type> inspectedTypes)
        {
            if (type.IsArray)
            {
                type = type.GetElementType();
            }

            if(IsGenericList(type))
            {
                type = type.GetGenericArguments()[0];
            }

            if (inspectedTypes.Contains(type))
            {
                return false;
            }

            inspectedTypes.Add(type);

            if(type.IsSubclassOf(typeof(UnityEventBase)))
            {
                return true;
            }

            PropertyInfo[] properties = GetProperties(type);
            for (int i = 0; i < properties.Length; ++i)
            {
                PropertyInfo property = properties[i];
                if (HasDependenciesRecursive(property.PropertyType, inspectedTypes))
                {
                    return true;
                }
            }

            FieldInfo[] fields = GetFields(type);
            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo field = fields[i];
                if (HasDependenciesRecursive(field.FieldType, inspectedTypes))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasDependenciesRecursive(Type type, HashSet<Type> inspectedTypes)
        {
            if (type.IsArray)
            {
                type = type.GetElementType();
            }

            if (IsGenericList(type))
            {
                type = type.GetGenericArguments()[0];
            }

            if (type.IsSubclassOf(typeof(UnityObject)))
            {
                return true;
            }

            if (type.IsSubclassOf(typeof(UnityEventBase)))
            {
                return true;
            }

            Type surrogateType = GetSurrogateType(type);
            if (surrogateType != null)
            {
                return HasDependencies(surrogateType, inspectedTypes);
            }
            return false;
        }

        /// <summary>
        /// Returns type name (including names of nested types)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string TypeName(Type type)
        {
            if (type.DeclaringType == null)
            {
                return _TypeName(type);
            }

            return TypeName(type.DeclaringType) + "+" + _TypeName(type);
        }

        private static string _TypeName(Type type)
        {
            if (type.IsGenericType)
            {
                string name = type.FullName;
                if (!string.IsNullOrEmpty(type.Namespace))
                {
                    name = name.Remove(0, type.Namespace.Length + 1);
                }
                return name;
            }
            return type.Name;
        }

        public string PreparePersistentTypeName(string typeName)
        {
            return typeName.Replace("+", "Nested");
        }

        public string PrepareMappedTypeName(string typeName)
        {
            return typeName.Replace("+", ".");
        }

        //Generate C# code of TypeMap for selected mappings
        public string CreateTypeMap(PersistentClassMapping[] mappings)
        {
            string usings = CreateUsings(mappings);
            string body = CreateTypeMapBody(mappings);

            return string.Format(TypeMapTemplate, usings, body);
        }

        private string CreateTypeMapBody(PersistentClassMapping[] mappings)
        {
            StringBuilder sb = new StringBuilder();
            for (int m = 0; m < mappings.Length; ++m)
            {
                PersistentClassMapping mapping = mappings[m];
                if (mapping == null)
                {
                    continue;
                }

                if (!mapping.IsOn)
                {
                    continue;
                }

                Type mappingType = Type.GetType(mapping.MappedAssemblyQualifiedName);
                if(mappingType == null)
                {
                    continue;
                }

                string mappedTypeName = PrepareMappedTypeName(TypeName(mappingType));
                string persistentTypeName = PreparePersistentTypeName(mapping.PersistentTypeName);
                if (mappedTypeName == "Object")
                {
                    mappedTypeName = "UnityObject";
                }
                sb.AppendFormat(AddToPersistentTypeTemplate, mappedTypeName, persistentTypeName);
                sb.Append(TAB3);
                sb.AppendFormat(AddToUnityTypeTemplate, persistentTypeName, mappedTypeName);
                sb.Append(TAB3);
                sb.AppendFormat(AddToGuidTemplate, persistentTypeName, mapping.PersistentTypeGUID);
                sb.Append(TAB3);
                sb.AppendFormat(AddToGuidTemplate, mappedTypeName, mapping.MappedTypeGUID);
                sb.Append(TAB3);
                sb.AppendFormat(AddToTypeTemplate, mapping.PersistentTypeGUID, persistentTypeName);
                sb.Append(TAB3);
                sb.AppendFormat(AddToTypeTemplate, mapping.MappedTypeGUID, mappedTypeName);
                sb.Append(TAB3);
            }
            sb.Append(BR);
            return sb.ToString();
        }

        //Generate C# code of TypeModelCreator for selected mappings
        public string CreateTypeModelCreator(PersistentClassMapping[] mappings)
        {
            string usings = CreateUsings(mappings);
            string body = CreateTypeModelCreatorBody(mappings);
            string nsDefinitions = CreateNamespaceDefinitions(mappings);
            return string.Format(TypeModelCreatorTemplate, usings, body, nsDefinitions);
        }

        private string CreateNamespaceDefinitions(PersistentClassMapping[] mappings)
        {
            StringBuilder sb = new StringBuilder();

            HashSet<string> nsHs = new HashSet<string>();
            for (int m = 0; m < mappings.Length; ++m)
            {
                PersistentClassMapping mapping = mappings[m];
                if(!mapping.IsOn)
                {
                    continue;
                }

                if (!nsHs.Contains(mapping.PersistentNamespace))
                {
                    nsHs.Add(mapping.PersistentNamespace);
                }

                for (int i = 0; i < mapping.PropertyMappings.Length; ++i)
                {
                    PersistentPropertyMapping pMapping = mapping.PropertyMappings[i];
                    if(!pMapping.IsEnabled || pMapping.HasPropertyInTemplate)
                    {
                        continue;
                    }
                    if(!nsHs.Contains(pMapping.PersistentNamespace))
                    {
                        nsHs.Add(pMapping.PersistentNamespace);
                    }
                }
            }

            foreach(string ns in nsHs)
            {
                sb.AppendFormat(NamespaceDefinitionTemplate, ns);
                sb.Append(BR);
            }
            
            return sb.ToString();
        }

        private string CreateTypeModelCreatorBody(PersistentClassMapping[] mappings)
        {
            StringBuilder sb = new StringBuilder();
            for (int m = 0; m < mappings.Length; ++m)
            {
                PersistentClassMapping mapping = mappings[m];
                if (mapping == null)
                {
                    continue;
                }

                if (!mapping.IsOn)
                {
                    continue;
                }
                string endOfLine = string.Empty;
                if (mapping.Subclasses != null && mapping.Subclasses.Where(s => s.IsEnabled).Count() > 0)
                {
                    endOfLine = CreateAddSubtypesBody(mapping);
                }

                Type mappingType = Type.GetType(mapping.MappedAssemblyQualifiedName);
                if (mappingType == null)
                {
                    Debug.LogWarning("Type " + mapping.MappedAssemblyQualifiedName + " was not found");
                }
                else
                {
                    //if (mappingType.IsSubclassOf(typeof(UnityObject)) || mappingType == typeof(UnityObject) || mappingType.IsSubclassOf(typeof(UnityEventBase)))
                    {
                        sb.AppendFormat(AddTypeTemplate, PreparePersistentTypeName(mapping.PersistentTypeName), "true", endOfLine + SEMICOLON + BR + TAB3);
                    }

                    if (GetSurrogateType(mappingType) != null)
                    {
                        if (!mappingType.IsSubclassOf(typeof(UnityEventBase)))
                        {
                            endOfLine = string.Format(SetSerializationSurrogate, PreparePersistentTypeName(mapping.PersistentTypeName));
                            sb.AppendFormat(AddTypeTemplate, PrepareMappedTypeName(mapping.MappedTypeName), "false", endOfLine + SEMICOLON + BR + TAB3);
                        }
                    }
                    
                }
            }

            return sb.ToString();
        }

        private string CreateAddSubtypesBody(PersistentClassMapping mapping)
        {
            StringBuilder sb = new StringBuilder();
            PersistentSubclass[] subclasses = mapping.Subclasses.Where(sc => sc.IsEnabled).ToArray();
            for (int i = 0; i < subclasses.Length - 1; ++i)
            {
                PersistentSubclass subclass = subclasses[i];
                
                Type mappingType = Type.GetType(subclass.MappedAssemblyQualifiedName);
                if (mappingType == null)
                {
                    Debug.LogWarning("Type " + subclass.MappedAssemblyQualifiedName + " was not found");
                }
                else
                {
                    sb.Append(BR + TAB3 + TAB);
                    sb.AppendFormat(AddSubtypeTemplate, PreparePersistentTypeName(subclass.TypeName), subclass.PersistentTag + SubclassOffset);
                }
            }

            if (subclasses.Length > 0)
            {
                if(subclasses[subclasses.Length - 1].IsEnabled)
                {
                    PersistentSubclass subclass = subclasses[subclasses.Length - 1];

                    Type mappingType = Type.GetType(subclass.MappedAssemblyQualifiedName);
                    if (mappingType == null)
                    {
                        Debug.LogWarning("Type " + subclass.MappedAssemblyQualifiedName + " was not found");
                    }
                    else
                    {
                        sb.Append(BR + TAB3 + TAB);
                        sb.AppendFormat(AddSubtypeTemplate, PreparePersistentTypeName(subclass.TypeName), subclass.PersistentTag + SubclassOffset);
                    }
                }
            }

            return sb.ToString();
        }

        public static bool TryGetTemplateUsings(string template, out string result)
        {
            result = string.Empty;

            int startIndex = template.IndexOf("//<TEMPLATE_USINGS_START>");
            int endIndex = template.IndexOf("//<TEMPLATE_USINGS_END>");

            if (startIndex < 0 || endIndex < 0 || startIndex >= endIndex)
            {
                return false;
            }

            template = template.Substring(startIndex, endIndex - startIndex);
            template = template.Replace("//<TEMPLATE_USINGS_START>", string.Empty);
            
            result = template;
            return true;
        }

        public static bool TryGetTemplateBody(string template, out string result)
        {
            result = string.Empty;

            int startIndex = template.IndexOf("//<TEMPLATE_BODY_START>");
            int endIndex = template.IndexOf("//<TEMPLATE_BODY_END>");

            if(startIndex < 0 || endIndex < 0 || startIndex >= endIndex)
            {
                return false;
            }

            template = template.Substring(startIndex, endIndex - startIndex);
            template = template.Replace("//<TEMPLATE_BODY_START>", string.Empty);
            template = template.Replace("#if RTSL_COMPILE_TEMPLATES", string.Empty);
            template = template.Replace("#endif", string.Empty);
            template = template.Replace("_RTSL_Template", string.Empty);

            result = template;
            return true;
        }

        public string CreatePersistentClassCustomImplementation(string ns, string persistentTypeName, PersistentTemplateInfo template = null)
        {
            string usings = "using Battlehub.RTSL;";
            string className = PreparePersistentTypeName(persistentTypeName);
            if(template != null)
            {
                usings += template.Usings; 
                return string.Format(UserDefinedClassTemplate, usings, ns, className, template.Body.TrimEnd());
            }
         
            return string.Format(UserDefinedEmptyClassTemplate, usings, ns, className);
        }


        /// <summary>
        /// Generate C# code of persistent class using persistent class mapping
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public string CreatePersistentClass(PersistentClassMapping mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }
            string usings = CreateUsings(mapping);
            string ns = mapping.PersistentNamespace;
            string className = PreparePersistentTypeName(mapping.PersistentTypeName);
            string baseClassName = mapping.PersistentBaseTypeName != null ?
                 PreparePersistentTypeName(mapping.PersistentBaseTypeName) : null;
            string body = mapping.IsOn ? CreatePersistentClassBody(mapping) : string.Empty;
            return string.Format(PersistentClassTemplate, usings, ns, className, baseClassName, body);
        }

        private string CreatePersistentClassBody(PersistentClassMapping mapping)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < mapping.PropertyMappings.Length; ++i)
            {
                PersistentPropertyMapping prop = mapping.PropertyMappings[i];
                if (!prop.IsEnabled || prop.HasPropertyInTemplate)
                {
                    continue;
                }

                if (prop.MappedType == null)
                {
                    continue;
                }

                string typeName = GetTypeName(prop);

                sb.AppendFormat(
                    FieldTemplate, i + AutoFieldTagOffset,
                    typeName,
                    prop.PersistentName);
            }

            string readMethodBody = CreateReadMethodBody(mapping);
            string writeMethodBody = CreateWriteMethodBody(mapping);
            string getDepsMethodBody = CreateDepsMethodBody(mapping);
            string getDepsFromMethodBody = CreateDepsFromMethodBody(mapping);

            string mappedTypeName = PrepareMappedTypeName(mapping.MappedTypeName);
            if (mappedTypeName == "Object")
            {
                mappedTypeName = "UnityObject";
            }

            if (!string.IsNullOrEmpty(readMethodBody))
            {
                sb.AppendFormat(ReadFromMethodTemplate, readMethodBody, mappedTypeName);
            }
        
            if (!string.IsNullOrEmpty(writeMethodBody))
            {
                sb.Append(BR + TAB2);
                sb.AppendFormat(WriteToMethodTemplate, writeMethodBody, mappedTypeName);
            }

            if (!string.IsNullOrEmpty(getDepsMethodBody))
            {
                sb.Append(BR + TAB2);
                sb.AppendFormat(GetDepsMethodTemplate, getDepsMethodBody);
            }
            if (!string.IsNullOrEmpty(getDepsFromMethodBody))
            {
                sb.Append(BR + TAB2);
                sb.AppendFormat(GetDepsFromMethodTemplate, getDepsFromMethodBody, mappedTypeName);
            }

            Type mappingType = Type.GetType(mapping.MappedAssemblyQualifiedName);
            if(mappingType != null)
            {
                if (mappingType.GetConstructor(Type.EmptyTypes) != null || mappingType.IsValueType)
                {
                    if(!mappingType.IsSubclassOf(typeof(UnityObject)))
                    {
                        sb.Append(BR + TAB2);
                        sb.AppendFormat(ImplicitOperatorsTemplate, mappedTypeName, PreparePersistentTypeName(mapping.PersistentTypeName));
                    }
                }
            }
          
            return sb.ToString();
        }

        private string GetTypeName(PersistentPropertyMapping prop, bool useReplacementType = true)
        {
            string typeName;
            Type repacementType = GetReplacementType(prop.MappedType);
            if (repacementType != null && useReplacementType)
            {
                string primitiveTypeName;
                if (m_primitiveNames.TryGetValue(repacementType, out primitiveTypeName))
                {
                    typeName = primitiveTypeName;
                }
                else
                {
                    typeName = PrepareMappedTypeName(repacementType.Name);
                }
            }
            else
            {
                string primitiveTypeName;
                if (m_primitiveNames.TryGetValue(prop.MappedType, out primitiveTypeName))
                {
                    typeName = primitiveTypeName;
                }
                else
                {
                    if (IsGenericList(prop.MappedType))
                    {
                        Type argType = prop.MappedType.GetGenericArguments()[0];
                        if (m_primitiveNames.TryGetValue(argType, out primitiveTypeName))
                        {
                            typeName = string.Format("List<{0}>", primitiveTypeName);
                        }
                        else
                        {
                            if (prop.UseSurrogate)
                            {
                                typeName = string.Format("List<Persistent{0}>", argType.Name);
                            }
                            else
                            {
                                typeName = string.Format("List<{0}>", argType.FullName);
                            }
                        }
                    }
                    else
                    {
                        if (prop.UseSurrogate)
                        {
                            typeName = "Persistent" + PreparePersistentTypeName(prop.PersistentTypeName);
                        }
                        else
                        {
                            typeName = PrepareMappedTypeName(prop.MappedTypeName);
                        }
                    }

                }
            }

            return typeName;
        }

        private string CreateReadMethodBody(PersistentClassMapping mapping)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < mapping.PropertyMappings.Length; ++i)
            {
                PersistentPropertyMapping prop = mapping.PropertyMappings[i];
                if (!prop.IsEnabled || prop.HasPropertyInTemplate)
                {
                    continue;
                }

                if(prop.MappedType == null)
                {
                    continue;
                }

                sb.Append(TAB);

                string get = "uo.{1}";
                if(prop.IsNonPublic)
                {
                    get = "GetPrivate<" + GetTypeName(prop, false) + ">(uo, \"{1}\")";
                }

                
                if(prop.MappedType.IsSubclassOf(typeof(UnityObject)) ||
                   prop.MappedType.IsArray && prop.MappedType.GetElementType().IsSubclassOf(typeof(UnityObject)) || 
                   IsGenericList(prop.MappedType) && prop.MappedType.GetGenericArguments()[0].IsSubclassOf(typeof(UnityObject)))
                {
                    //generate code which will convert unity object to identifier
                    sb.AppendFormat("{0} = ToID(" + get + ");", prop.PersistentName, prop.MappedName);
                }
                else
                {
                    if (prop.UseSurrogate)
                    {
                        if (IsGenericList(prop.MappedType))
                        {
                            sb.AppendFormat("{0} = Assign(" + get + ", v_ => ({2})v_);", prop.PersistentName, prop.MappedName, PreparePersistentTypeName("Persistent" + prop.MappedType.GetGenericArguments()[0].Name));
                        }
                        else if(prop.MappedType.IsArray)
                        {
                            sb.AppendFormat("{0} = Assign(" + get + ", v_ => ({2})v_);", prop.PersistentName, prop.MappedName, PreparePersistentTypeName("Persistent" + prop.MappedType.GetElementType().Name));
                        }
                        else
                        {
                            sb.AppendFormat("{0} = " + get + ";", prop.PersistentName, prop.MappedName);
                        }
                    }
                    else
                    {
                        sb.AppendFormat("{0} = " + get + ";", prop.PersistentName, prop.MappedName);
                    }
                }

                sb.Append(BR + TAB2);
            }

            return sb.ToString();
        }

        private string CreateWriteMethodBody(PersistentClassMapping mapping)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < mapping.PropertyMappings.Length; ++i)
            {
                PersistentPropertyMapping prop = mapping.PropertyMappings[i];
                if (!prop.IsEnabled || prop.HasPropertyInTemplate)
                {
                    continue;
                }
                if (prop.MappedType == null)
                {
                    continue;
                }

                sb.Append(TAB);

                string get = "uo.{0}";
                if (prop.IsNonPublic)
                {
                    get = "GetPrivate<" + GetTypeName(prop, false) + ">(uo, \"{0}\")";
                }

                if (prop.MappedType.IsSubclassOf(typeof(UnityObject)) || 
                    prop.MappedType.IsArray && prop.MappedType.GetElementType().IsSubclassOf(typeof(UnityObject)) ||
                    IsGenericList(prop.MappedType) && prop.MappedType.GetGenericArguments()[0].IsSubclassOf(typeof(UnityObject)))
                {
                    //generate code which will convert identifier to unity object

                    //Type mappedType = prop.MappedType.IsArray ? prop.MappedType.GetElementType() : prop.MappedType;
                    //sb.AppendFormat("uo.{0} = FromID<{2}>({1}, uo.{0});", prop.MappedName, prop.PersistentName, PrepareMappedTypeName(mappedType.Name));
                    if (prop.IsNonPublic)
                    {
                        sb.AppendFormat("SetPrivate(uo, \"{0}\", FromID({1}, " + get + "));", prop.MappedName, prop.PersistentName);
                    }
                    else
                    {
                        sb.AppendFormat("uo.{0} = FromID({1}, " + get + ");", prop.MappedName, prop.PersistentName);
                    }
                }
                else
                {
                    if (prop.UseSurrogate)
                    {
                        if (IsGenericList(prop.MappedType))
                        {
                            if (prop.IsNonPublic)
                            {
                                sb.AppendFormat("SetPrivate(uo, \"{0}\", Assign({1}, v_ => ({2})v_));", prop.MappedName, prop.PersistentName, prop.MappedType.GetGenericArguments()[0].Name);
                            }
                            else
                            {
                                sb.AppendFormat("uo.{0} = Assign({1}, v_ => ({2})v_);", prop.MappedName, prop.PersistentName, prop.MappedType.GetGenericArguments()[0].Name);
                            }
                        }
                        else if (prop.MappedType.IsArray)
                        {
                            if (prop.IsNonPublic)
                            {
                                sb.AppendFormat("SetPrivate(uo, \"{0}\", Assign({1}, v_ => ({2})v_));", prop.MappedName, prop.PersistentName, prop.MappedType.GetElementType().Name);
                            }
                            else
                            {
                                sb.AppendFormat("uo.{0} = Assign({1}, v_ => ({2})v_);", prop.MappedName, prop.PersistentName, prop.MappedType.GetElementType().Name);
                            }

                            
                        }
                        else
                        {
                            if (prop.IsNonPublic)
                            {
                                sb.AppendFormat("SetPrivate(uo, \"{0}\", {1});", prop.MappedName, prop.PersistentName);
                            }
                            else
                            {
                                sb.AppendFormat("uo.{0} = {1};", prop.MappedName, prop.PersistentName);
                            }
                        }
                    }
                    else
                    {
                        if (prop.IsNonPublic)
                        {
                            sb.AppendFormat("SetPrivate(uo, \"{0}\", {1});", prop.MappedName, prop.PersistentName);
                        }
                        else
                        {
                            sb.AppendFormat("uo.{0} = {1};", prop.MappedName, prop.PersistentName);
                        }
                    }
                    
                }

                sb.Append(BR + TAB2);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate method which will populate context with dependencies (referenced unity object identifiers)
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        private string CreateDepsMethodBody(PersistentClassMapping mapping)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mapping.PropertyMappings.Length; ++i)
            {
                PersistentPropertyMapping prop = mapping.PropertyMappings[i];
                if (!prop.IsEnabled || prop.HasPropertyInTemplate)
                {
                    continue;
                }
                if (prop.MappedType == null)
                {
                    continue;
                }

                if (prop.HasDependenciesOrIsDependencyItself)
                {
                    if (prop.UseSurrogate)
                    {
                        sb.Append(TAB);
                        sb.AppendFormat("AddSurrogateDeps({0}, context);", prop.PersistentName);
                        sb.Append(BR + TAB2);
                    }
                    else if (prop.MappedType.IsSubclassOf(typeof(UnityObject)) ||
                        prop.MappedType.IsArray && prop.MappedType.GetElementType().IsSubclassOf(typeof(UnityObject)) ||
                        IsGenericList(prop.MappedType) && prop.MappedType.GetGenericArguments()[0].IsSubclassOf(typeof(UnityObject)))
                    {
                        sb.Append(TAB);
                        sb.AppendFormat("AddDep({0}, context);", prop.PersistentName);
                        sb.Append(BR + TAB2);
                    }
                }    
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generate method which will extract and populate context with dependencies (referenced unity objects)
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        private string CreateDepsFromMethodBody(PersistentClassMapping mapping)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mapping.PropertyMappings.Length; ++i)
            {
                PersistentPropertyMapping prop = mapping.PropertyMappings[i];
                if (!prop.IsEnabled || prop.HasPropertyInTemplate)
                {
                    continue;
                }
                if (prop.MappedType == null)
                {
                    continue;
                }
                if (prop.HasDependenciesOrIsDependencyItself)
                {
                    string get = "uo.{0}";
                    if (prop.IsNonPublic)
                    {
                        get = "GetPrivate<" + GetTypeName(prop, false) + ">(uo, \"{0}\")";
                    }

                    if (prop.UseSurrogate)
                    {
                        
                        sb.Append(TAB);
                        sb.AppendFormat("AddSurrogateDeps(" + get + ", v_ => ({1})v_, context);", prop.MappedName, PreparePersistentTypeName("Persistent" + prop.PersistentTypeName));
                        sb.Append(BR + TAB2);
                    }
                    if (prop.MappedType.IsSubclassOf(typeof(UnityObject)) ||
                        prop.MappedType.IsArray && prop.MappedType.GetElementType().IsSubclassOf(typeof(UnityObject)) ||
                        IsGenericList(prop.MappedType) && prop.MappedType.GetGenericArguments()[0].IsSubclassOf(typeof(UnityObject)))
                    {
                        sb.Append(TAB);
                        sb.AppendFormat("AddDep(" + get + ", context);", prop.MappedName);
                        sb.Append(BR + TAB2);
                    }
                }
            }
            return sb.ToString();
        }

        private string CreateUsings(params PersistentClassMapping[] mappings)
        {
            StringBuilder sb = new StringBuilder();
            HashSet<string> namespaces = new HashSet<string>();
            for (int i = 0; i < DefaultNamespaces.Length; ++i)
            {
                namespaces.Add(DefaultNamespaces[i]);
            }

            for (int m = 0; m < mappings.Length; ++m)
            {
                PersistentClassMapping mapping = mappings[m];
                if (mapping == null)
                {
                    continue;
                }
                
                if(mapping.IsOn)
                {
                    if (!namespaces.Contains(mapping.MappedNamespace) && !string.IsNullOrEmpty(mapping.MappedNamespace))
                    {
                        namespaces.Add(mapping.MappedNamespace);
                    }

                    if (!namespaces.Contains(mapping.PersistentNamespace))
                    {
                        namespaces.Add(mapping.PersistentNamespace);
                    }

                    if (!namespaces.Contains(mapping.PersistentBaseNamespace))
                    {
                        namespaces.Add(mapping.PersistentBaseNamespace);
                    }

                    for (int i = 0; i < mapping.PropertyMappings.Length; ++i)
                    {
                        PersistentPropertyMapping propertyMapping = mapping.PropertyMappings[i];
                        if (!propertyMapping.IsEnabled || propertyMapping.HasPropertyInTemplate)
                        {
                            continue;
                        }
                        if (!namespaces.Contains(propertyMapping.MappedNamespace) && !string.IsNullOrEmpty(propertyMapping.MappedNamespace))
                        {
                            namespaces.Add(propertyMapping.MappedNamespace);
                        }

                        Type type = propertyMapping.MappedType;
                        if(type == null)
                        {
                            Debug.LogWarning("Unable to resolve type: " + propertyMapping.MappedAssemblyQualifiedName);
                            continue;
                        }
                        Type replacementType = GetReplacementType(type);
                        if (replacementType != null)
                        {
                            if (!namespaces.Contains(replacementType.Namespace))
                            {
                                namespaces.Add(replacementType.Namespace);
                            }
                        }
                        else
                        {
                            if(!type.FullName.Contains("System"))
                            {
                                if (!namespaces.Contains(propertyMapping.PersistentNamespace))
                                {
                                    namespaces.Add(propertyMapping.PersistentNamespace);
                                }
                            }
                        }
                    }
                }
            }
            foreach (string ns in namespaces)
            {
                sb.Append("using " + ns + ";" + BR);
            }

            return sb.ToString();
        }

        private Type GetReplacementType(Type type)
        {
            if(type.IsArray)
            {
                Type elementType = type.GetElementType();
                if(elementType.IsSubclassOf(typeof(UnityObject)))
                {
                    return typeof(long[]);
                }
            }

            if(IsGenericList(type))
            {
                Type elementType = type.GetGenericArguments()[0];
                if (elementType.IsSubclassOf(typeof(UnityObject)))
                {
                    return typeof(long[]);
                }
            }

            if(type.IsSubclassOf(typeof(UnityObject)))
            {
                return typeof(long);
            }
            return null;
        }

        public static void GetUOAssembliesAndTypes(out Assembly[] assemblies, out Type[] types)
        {
            //assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("UnityEngine")).OrderBy(a => a.FullName).ToArray();
            assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.Contains("UnityEditor") && !a.FullName.Contains("Assembly-CSharp-Editor")).OrderBy(a => a.FullName).ToArray();

            List<Type> allUOTypes = new List<Type>();
            List<Assembly> assembliesList = new List<Assembly>();

            for (int i = 0; i < assemblies.Length; ++i)
            {
                Assembly assembly = assemblies[i];
                Type[] uoTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(UnityObject)) && !t.IsGenericType).ToArray();
                if (uoTypes.Length > 0)
                {
                    assembliesList.Add(assembly);
                    allUOTypes.AddRange(uoTypes);
                }
            }

            types = allUOTypes.OrderByDescending(t => t.FullName.Contains("UnityEngine")).ToArray();
            assemblies = new Assembly[] { null }.Union(assembliesList.OrderBy(a => a.FullName)).ToArray();
        }

        private static void GetTypesRecursive(Type type, HashSet<Type> typesHS)
        {
            PropertyInfo[] properties = GetAllProperties(type);
            FieldInfo[] fields = GetFields(type);
            MethodInfo[] methods = GetMethods(type);

            for (int p = 0; p < properties.Length; ++p)
            {
                PropertyInfo pInfo = properties[p];
                if (!typesHS.Contains(pInfo.PropertyType))
                {
                    Type surrogateType = GetSurrogateType(pInfo.PropertyType);
                    if (surrogateType != null && !typesHS.Contains(surrogateType))
                    {
                        typesHS.Add(surrogateType);
                        GetTypesRecursive(surrogateType, typesHS);
                    }
                }
            }

            for (int f = 0; f < fields.Length; ++f)
            {
                FieldInfo fInfo = fields[f];
                if (!typesHS.Contains(fInfo.FieldType))
                {
                    Type surrogateType = GetSurrogateType(fInfo.FieldType);
                    if (surrogateType != null && !typesHS.Contains(surrogateType))
                    {
                        typesHS.Add(surrogateType);
                        GetTypesRecursive(surrogateType, typesHS);
                    }
                }
            }

            for(int m = 0; m < methods.Length; ++m)
            {
                MethodInfo mInfo = methods[m];
                ParameterInfo[] parameters = mInfo.GetParameters();
                if(parameters != null)
                {
                    for(int i = 0; i < parameters.Length; ++i)
                    {
                        ParameterInfo pInfo = parameters[i];
                        if(pInfo != null && pInfo.ParameterType != null)
                        {
                            Type surrogateType = GetSurrogateType(pInfo.ParameterType);
                            if (surrogateType != null &&  !string.IsNullOrEmpty(surrogateType.FullName) && surrogateType != typeof(object) && !typesHS.Contains(surrogateType))
                            {
                                typesHS.Add(surrogateType);
                                GetTypesRecursive(surrogateType, typesHS);
                            }
                        }
                    }
                }
            }
        }

        public static void GetSurrogateAssembliesAndTypes(Type[] uoTypes, out Dictionary<string, HashSet<Type>> declaredIn, out Type[] types)
        {
            HashSet<Type> allTypesHS = new HashSet<Type>();
            declaredIn = new Dictionary<string, HashSet<Type>>();

            for (int typeIndex = 0; typeIndex < uoTypes.Length; ++typeIndex)
            {
                Type uoType = uoTypes[typeIndex];

                HashSet<Type> typesHs = new HashSet<Type>();
                GetTypesRecursive(uoType, typesHs);
                declaredIn.Add(uoType.FullName, typesHs);

                foreach (Type type in typesHs)
                {
                    if (!allTypesHS.Contains(type))
                    {
                        allTypesHS.Add(type);
                    }
                }
            }

            types = allTypesHS.ToArray();
        }
    }
}
