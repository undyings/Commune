using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Commune.Basis
{
  public static class BinaryHlp
  {
    public static int RoundUp(int value, int divisor)
    {
      int result = value / divisor;
      if (value % divisor == 0)
        return result;

      return result + 1;
    }

    public static byte GetBits(byte value, int bitShift, int bitSize)
    {
      return (byte)GetBits((uint)value, bitShift + 24, bitSize);
    }

    public static ushort GetBits(ushort value, int bitShift, int bitSize)
    {
      return (ushort)GetBits((uint)value, bitShift + 16, bitSize);
    }

    public static uint GetBits(uint value, int bitShift, int bitSize)
    {
      return (uint)(value << bitShift >> (32 - bitSize));
    }

    public static uint AsLittleUInt(byte[] bytes, int startIndex)
    {
      uint value = 0;
      for (int i = 0; i < 4; ++i)
        value += ((uint)bytes[startIndex + i] << (i * 8));
      return value;
    }

    public static byte[] UIntToLittle(uint value)
    {
      byte[] bytes = new byte[4];
      for (int i = 0; i < bytes.Length; ++i)
      {
        bytes[i] = (byte)(value >> (i * 8));
      }
      return bytes;
    }

    public static byte[] UShortToLittle(ushort value)
    {
      byte[] bytes = new byte[2];
      for (int i = 0; i < bytes.Length; ++i)
      {
        bytes[i] = (byte)(value >> (i * 8));
      }
      return bytes;
    }

    public static ushort AsLittleUShort(byte[] bytes, int startIndex)
    {
      ushort value = 0;
      for (int i = 0; i < 2; ++i)
        value += (ushort)(bytes[startIndex + i] << (i * 8));
      return value;
    }

    public static byte[] UShortToBig(ushort value)
    {
      byte[] bytes = new byte[2];
      for (int i = 0; i < bytes.Length; ++i)
      {
        bytes[bytes.Length - 1 - i] = (byte)(value >> (i * 8));
      }
      return bytes;
    }

    public static byte[] UIntToBig(uint value)
    {
      byte[] bytes = new byte[4];
      for (int i = 0; i < bytes.Length; ++i)
      {
        bytes[bytes.Length - 1 - i] = (byte)(value >> (i * 8));
      }
      return bytes;
    }

    public static byte[] GetBytesFromHexString(string hexEncoding)
    {
      return GetBytesFromHexString(hexEncoding, 0, hexEncoding.Length);
    }

    public static byte[] GetBytesFromHexString(string hexEncoding, int startIndex, int count)
    {
      byte[] result = new byte[count / 2];
      int charIndex = startIndex;
      for (int i = 0; i < result.Length; ++i)
      {
        byte ch = (byte)hexEncoding[charIndex];
        if (ch < 0x3A)
          result[i] = (byte)((ch - 0x30) << 4);
        else
          result[i] = (byte)((ch - 0x37) << 4);

        charIndex++;

        ch = (byte)hexEncoding[charIndex];
        if (ch < 0x3A)
          result[i] += (byte)(ch - 0x30);
        else
          result[i] += (byte)(ch - 0x37);

        charIndex++;
      }
      return result;
    }

    public static byte[] GetBytesFromHexBytes(byte[] hexBytes)
    {
      return GetBytesFromHexBytes(hexBytes, 0, hexBytes.Length);
    }

    public static byte[] GetBytesFromHexBytes(byte[] hexBytes, int startIndex, int count)
    {
      byte[] result = new byte[count / 2];
      int charIndex = startIndex;
      for (int i = 0; i < result.Length; ++i)
      {
        byte ch = hexBytes[charIndex];
        if (ch < 0x3A)
          result[i] = (byte)((ch - 0x30) << 4);
        else
          result[i] = (byte)((ch - 0x37) << 4);

        charIndex++;

        ch = hexBytes[charIndex];
        if (ch < 0x3A)
          result[i] += (byte)(ch - 0x30);
        else
          result[i] += (byte)(ch - 0x37);

        charIndex++;
      }
      return result;
    }


    public static byte[] BytesToHexBytes(byte[] bytes)
    {
      byte[] hexBytes = new byte[bytes.Length * 2];
      int hexIndex = 0;
      for (int i = 0; i < bytes.Length; ++i)
      {
        byte ch = (byte)(bytes[i] >> 4);
        if (ch < 0x0A)
          hexBytes[hexIndex] = (byte)(ch + 0x30);
        else
          hexBytes[hexIndex] = (byte)(ch + 0x37);

        hexIndex++;

        ch = (byte)(bytes[i] & 0x0F);
        if (ch < 0x0A)
          hexBytes[hexIndex] = (byte)(ch + 0x30);
        else
          hexBytes[hexIndex] = (byte)(ch + 0x37);

        hexIndex++;
      }
      return hexBytes;
    }

    public static string BytesToHexString(byte[] bytes)
    {
      return Encoding.ASCII.GetString(BytesToHexBytes(bytes));
    }

    public static T[] BytesToStructs<T>(byte[] bytes, int offset, int structCount)
    {
      int overlaySize = Marshal.SizeOf(typeof(T));
      return BytesToStructs<T>(ArrayHlp.GetRange(bytes, offset, structCount * overlaySize));
    }

    public static T[] BytesToStructs<T>(byte[] operativeData)
    {
      int overlaySize = Marshal.SizeOf(typeof(T));
      T[] result = new T[operativeData.Length / overlaySize];
      GCHandle pinnedRawData = GCHandle.Alloc(result, GCHandleType.Pinned);
      try
      {
        IntPtr pinnedRawDataPtr = pinnedRawData.AddrOfPinnedObject();
        Marshal.Copy(operativeData, 0, pinnedRawDataPtr, result.Length * overlaySize);
      }
      finally
      {
        pinnedRawData.Free();
      }

      return result;
    }

    public static byte[] StructsToBytes<T>(T[] obj)
    {
      int overlaySize = Marshal.SizeOf(typeof(T));
      byte[] result = new byte[obj.Length * overlaySize];

      GCHandle pinnedRawData = GCHandle.Alloc(obj,
        GCHandleType.Pinned);
      try
      {
        IntPtr pinnedRawDataPtr =
          pinnedRawData.AddrOfPinnedObject();

        Marshal.Copy(pinnedRawDataPtr, result, 0, result.Length);
      }
      finally
      {
        pinnedRawData.Free();
      }

      return result;
    }
  }
}
