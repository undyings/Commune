using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Basis
{
  public class ByteLink
  {
    public static void InitializeLinks(int packageBytesCount, params ByteLink[] fieldLinks)
    {
      int offsetInBytes = 0;
      foreach (ByteLink field in fieldLinks)
      {
        field.byteOffset = offsetInBytes;
        offsetInBytes += field.ByteSize;
      }

      if (packageBytesCount != offsetInBytes)
        throw new Exception(string.Format(
          "Длина байтового пакета '{0}' не равна длине составляющих его полей '{1}'",
          packageBytesCount, offsetInBytes));
    }

    public readonly int ByteSize;

    int byteOffset = -1;
    public int ByteOffset
    {
      get { return byteOffset; }
    }

    public ByteLink(int byteSize)
    {
      this.ByteSize = byteSize;
    }

    public ByteField ToField(uint value)
    {
      return new ByteField(ByteSize, value);
    }
  }

  public struct ByteField
  {
    public readonly int ByteSize;
    public readonly uint Value;

    public ByteField(int byteSize, uint value)
    {
      this.ByteSize = byteSize;
      this.Value = value;
    }
  }

  public struct LEBytePackage
  {
    readonly byte[] bytes;
    readonly int packageOffset;

    public LEBytePackage(byte[] bytes, int packageOffset)
    {
      this.bytes = bytes;
      this.packageOffset = packageOffset;
    }

    public LEBytePackage(byte[] bytes) :
      this(bytes, 0)
    {
    }

    public uint Get(ByteLink field)
    {
      int offset = packageOffset + field.ByteOffset;
      uint result = 0;
      for (int i = 0; i < field.ByteSize; ++i)
      {
        result += ((uint)bytes[offset + i] << (i * 8));
      }
      return result;
    }

    public void Set(ByteLink field, uint value)
    {
      int offset = packageOffset + field.ByteOffset;
      byte[] valueBytes = BitConverter.GetBytes(value);
      Array.Copy(valueBytes, 0, bytes, offset, field.ByteSize);
    }

    public void Set(ByteLink field, int value)
    {
      Set(field, (uint)value);
    }

    public int GetInt(ByteLink field)
    {
      return (int)Get(field);
    }

    public byte GetByte(ByteLink field)
    {
      return (byte)Get(field);
    }

    public ushort GetUShort(ByteLink field)
    {
      return (ushort)Get(field);
    }

    public ByteField GetField(ByteLink field)
    {
      return new ByteField(field.ByteSize, this.Get(field));
    }
  }

  public struct BytePackage
  {
    public static byte[] ToRawPackage(int packageBytesCount, params ByteField[] packageFields)
    {
      byte[] bytes = new byte[packageBytesCount];
      int byteOffset = 0;
      foreach (ByteField field in packageFields)
      {
        for (int i = 0; i < field.ByteSize; ++i)
        {
          bytes[byteOffset + i] = (byte)(field.Value >> ((field.ByteSize - 1 - i) * 8));
        }
        byteOffset += field.ByteSize;
      }
      return bytes;
    }

    readonly byte[] bytes;
    readonly int packageOffset;

    public BytePackage(byte[] bytes, int packageOffset)
    {
      this.bytes = bytes;
      this.packageOffset = packageOffset;
    }

    public BytePackage(byte[] bytes) :
      this(bytes, 0)
    {
    }

    public uint Get(ByteLink field)
    {
      int offset = packageOffset + field.ByteOffset;
      uint result = 0;
      for (int i = 0; i < field.ByteSize; ++i)
      {
        result += ((uint)bytes[offset + i] << ((field.ByteSize - 1 - i) * 8));
      }
      return result;
    }

    public int GetInt(ByteLink field)
    {
      return (int)Get(field);
    }

    public byte GetByte(ByteLink field)
    {
      return (byte)Get(field);
    }

    public ushort GetUShort(ByteLink field)
    {
      return (ushort)Get(field);
    }

    public ByteField GetField(ByteLink field)
    {
      return new ByteField(field.ByteSize, this.Get(field));
    }

    public byte[] GetBytes(ByteLink field)
    {
      return ArrayHlp.GetRange(bytes, packageOffset + field.ByteOffset, field.ByteSize);
    }
  }
}
