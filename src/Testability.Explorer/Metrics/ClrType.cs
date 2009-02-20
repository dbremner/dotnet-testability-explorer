using System;
using Mono.Cecil;

namespace Thinklouder.Testability.Metrics
{
    public class ClrType : Type
    {
        /// <summary>
        /// Primitive Data Types
        /// </summary>

        public const string SYSTEM_VOID = "System.Void";
        public const string SYSTEM_BOOLEAN = "System.Boolean";
        public const string SYSTEM_CHAR = "System.Char";
        public const string SYSTEM_SBYTE = "System.SByte";
        public const string SYSTEM_BYTE = "System.Byte";
        public const string SYSTEM_INT16 = "System.Int16";
        public const string SYSTEM_UINT16 = "System.UInt16";
        public const string SYSTEM_INT32 = "System.Int32";
        public const string SYSTEM_UINT32 = "System.UInt32";
        public const string SYSTEM_INT64 = "System.Int64";
        public const string SYSTEM_UINT64 = "System.UInt64";
        public const string SYSTEM_SINGLE = "System.Single";
        public const string SYSTEM_DOUBLE = "System.Double";
        public const string SYSTEM_TYPEDREFERENCE = "System.TypedReference";
        public const string SYSTEM_INTPTR = "System.IntPtr";
        public const string SYSTEM_UINTPTR = "System.UIntPtr";

        public const string SYSTEM_OBJECT = "System.Object";
        public const string SYSTEM_STRING = "System.String";

        public static readonly Type Void = FromClr(SYSTEM_VOID);
        public static readonly Type Boolean = FromClr(SYSTEM_BOOLEAN);
        public static readonly Type Char = FromClr(SYSTEM_CHAR);
        public static readonly Type SByte = FromClr(SYSTEM_SBYTE);
        public static readonly Type Byte = FromClr(SYSTEM_BYTE);
        public static readonly Type Int16 = FromClr(SYSTEM_INT16);
        public static readonly Type UInt16 = FromClr(SYSTEM_UINT16);
        public static readonly Type Int32 = FromClr(SYSTEM_INT32);
        public static readonly Type UInt32 = FromClr(SYSTEM_UINT32);
        public static readonly Type Int64 = FromClr(SYSTEM_INT64);
        public static readonly Type UInt64 = FromClr(SYSTEM_UINT64);
        public static readonly Type Single = FromClr(SYSTEM_SINGLE);
        public static readonly Type Double = FromClr(SYSTEM_DOUBLE);
        public static readonly Type TypedReference = FromClr(SYSTEM_TYPEDREFERENCE);
        public static readonly Type IntPtr = FromClr(SYSTEM_INTPTR);
        public static readonly Type UIntPtr = FromClr(SYSTEM_UINTPTR);

        public static readonly Type Object = FromClr(SYSTEM_OBJECT);
        public static readonly Type String = FromClr(SYSTEM_STRING);

        private ClrType(string name, string ns)
            : base(ns+name)
        {
        }

        public static bool IsDoubleSlot(Type type)
        {
            return false;
            //return type == Double || type == Int64;  // TODO, if UInt64 double slot? by sunlw
        }

        // TODO, do we meed more info for the Type model
        private static Type FromClr(string clazz)
        {
            int dotIndex = clazz.LastIndexOf(".");
            string name;
            string ns;
            if (dotIndex == -1)
            {
                name = clazz;
                ns = string.Empty;
            }
            else
            {
                name = clazz.Substring(dotIndex, clazz.Length - dotIndex);
                ns = clazz.Substring(0, dotIndex);
            }

            return new Type(0, clazz, false);
        }

        public static Type FromClr(System.Type type)
        {
            return FromDescriptor(type.FullName);
        }

        public static Type FromClr<T>()
        {
            return FromDescriptor(typeof(T).FullName);
        }

        public static Type FromDescriptor(string descriptor)
        {
            switch(descriptor)
            {
                case SYSTEM_VOID:
                    return Void;
                case SYSTEM_BOOLEAN:
                    return Boolean;
                case SYSTEM_CHAR:
                    return Char;
                case SYSTEM_SBYTE:
                    return SByte;
                case SYSTEM_BYTE:
                    return Byte;
                case SYSTEM_INT16:
                    return Int16;
                case SYSTEM_UINT16:
                    return UInt16;
                case SYSTEM_INT32:
                    return Int32;
                case SYSTEM_UINT32:
                    return UInt32;
                case SYSTEM_INT64:
                    return Int64;
                case SYSTEM_UINT64:
                    return UInt64;
                case SYSTEM_SINGLE:
                    return Single;
                case SYSTEM_DOUBLE:
                    return Double;
                case SYSTEM_TYPEDREFERENCE:
                    return TypedReference;
                case SYSTEM_INTPTR:
                    return IntPtr;
                case SYSTEM_UINTPTR:
                    return UIntPtr;
                case SYSTEM_OBJECT:
                    return Object;
                case SYSTEM_STRING:
                    return String;
                default:
                    char ch = descriptor[descriptor.Length - 1];
                    if (ch.Equals("]"))
                    {
                        return FromClr(descriptor.Substring(0, descriptor.Length - 2)).ToArray();
                    }
                    break;
            }
            return new Type(descriptor);
        }
    }
}