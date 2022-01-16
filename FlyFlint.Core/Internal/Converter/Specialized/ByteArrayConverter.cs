////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

namespace FlyFlint.Internal.Converter.Specialized
{
    internal static class ByteArrayConverter
    {
        public static byte[] Convert(ConversionContext context, object value)
        {
            if (value is string str)
            {
                return context.encoding.GetBytes(str);
            }
            else if (value is char c)
            {
                return BitConverter.GetBytes(c);
            }
            else if (value is bool b)
            {
                return BitConverter.GetBytes(b);
            }
            else if (value is byte b8)
            {
                return BitConverter.GetBytes(b8);
            }
            else if (value is short i16)
            {
                return BitConverter.GetBytes(i16);
            }
            else if (value is int i32)
            {
                return BitConverter.GetBytes(i32);
            }
            else if (value is long i64)
            {
                return BitConverter.GetBytes(i64);
            }
            else if (value is float f)
            {
                return BitConverter.GetBytes(f);
            }
            else if (value is double d)
            {
                return BitConverter.GetBytes(d);
            }
            else if (value is Guid g)
            {
                return g.ToByteArray();
            }
            else if (value is DateTime dt)
            {
                return BitConverter.GetBytes(dt.ToBinary());
            }
            else
            {
                return context.encoding.GetBytes(System.Convert.ToString(value, context.fp)!);
            }
        }
    }
}
