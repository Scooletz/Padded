using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace Padded.Fody
{
    public static class FieldAddressFinder
    {
        public static Dictionary<FieldInfo, int> GetFieldOffsets(Type t)
        {
            try
            {
                var fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                var dm = new DynamicMethod("_GetOffsetsInMemory" + t.FullName, typeof (long[]), new[] {typeof (object)});
                var il = dm.GetILGenerator();

                var retLoc = il.DeclareLocal(typeof (long[]));

                il.Emit(OpCodes.Ldind_I, fields.Length); // ulong
                il.Emit(OpCodes.Newarr, typeof (long[])); // ulong[]
                il.Emit(OpCodes.Stloc_0); // --empty--

                for (var i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];

                    il.Emit(OpCodes.Ldloc, retLoc); // ulong[]
                    il.Emit(OpCodes.Ldc_I4, i); // ulong[] int

                    il.Emit(OpCodes.Ldarg_0); // ulong[] int param#0
                    il.Emit(OpCodes.Castclass, t); // ulong[] int param#0

                    il.Emit(OpCodes.Ldflda, field); // ulong[] int field&
                    il.Emit(OpCodes.Conv_I8); // ulong[] int ulong

                    il.Emit(OpCodes.Stelem, typeof (long)); // --empty--
                }

                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                var getAddr = (Func<object, long[]>) dm.CreateDelegate(typeof (Func<object, long[]>));

                var obj = FormatterServices.GetUninitializedObject(t);
                var addrs = getAddr(obj);

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
    }
}