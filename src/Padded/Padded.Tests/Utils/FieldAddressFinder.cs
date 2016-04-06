using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Sigil;

namespace Padded.Tests.Utils
{
    public static class FieldAddressFinder
    {
        public static Dictionary<FieldInfo, int> GetFieldOffsets(Type t)
        {
            try
            {
                var fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                var getAddrs = t.IsValueType
                    ? BuildGetValueTypeFieldOffsets(t, fields)
                    : BuildGetRefernceTypeFieldOffsets(t, fields);
                var obj = FormatterServices.GetUninitializedObject(t);
                var addrs = new ulong[fields.Length];
                getAddrs(obj, addrs);

                if (addrs.Length == 0)
                {
                    return new Dictionary<FieldInfo, int>();
                }

                var min = addrs.Min();

                var ret = new Dictionary<FieldInfo, int>();

                for (var i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];

                    var addr = addrs[i];
                    var offset = addr - min;

                    ret[field] = (int) offset;
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Cannot get offset for type {t}", ex);
            }
        }

        private static Action<object, ulong[]> BuildGetRefernceTypeFieldOffsets(Type t, FieldInfo[] fields)
        {
            var emit = Emit<Action<object, ulong[]>>.NewDynamicMethod("_GetOffsetsInMemory" + t.FullName);

            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                emit.LoadArgument(1); // ulong[]
                emit.LoadConstant(i); // ulong[] ulong

                emit.LoadArgument(0); // ulong[] ulong param#0
                emit.CastClass(t); // ulong[] ulong param#0

                emit.LoadFieldAddress(field); // ulong[] ulong field&
                emit.Convert<ulong>(); // ulong[] ulong ulong

                emit.StoreElement<ulong>(); // --empty--
            }

            emit.Return(); // --empty--

            return emit.CreateDelegate();
        }

        public static Action<object, ulong[]> BuildGetValueTypeFieldOffsets(Type t, FieldInfo[] fields)
        {
            var emit = Emit<Action<object, ulong[]>>.NewDynamicMethod("_GetOffsetsInMemory" + t.FullName);
            var val = emit.DeclareLocal(t);

            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                emit.LoadArgument(1); // ulong[]
                emit.LoadConstant(i); // ulong[] ulong                      Ldc_I4_S

                emit.LoadLocal(val); // ulong[] ulong param#0               Ldloc_0
                emit.LoadFieldAddress(field); // ulong[] ulong field&       Ldflda
                emit.Convert<ulong>(); // ulong[] ulong ulong               Conv_U8

                emit.StoreElement<ulong>(); // --empty--                    Stelem_I8
            }

            emit.Return(); // --empty--

            return emit.CreateDelegate();
        }
    }
}