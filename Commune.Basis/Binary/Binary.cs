using System;
using System.Collections.Generic;
using System.Text;

namespace Commune.Basis
{
  public struct OffsetArray
  {
    readonly byte[] array;
    public readonly int Offset;
    public readonly int Length;

    public OffsetArray(byte[] array, int offset, int length)
    {
      this.array = array;
      this.Offset = offset;
      this.Length = length;
    }

    public byte this[int index]
    {
      get { return array[Offset + index]; }
    }

    public byte[] ToArray()
    {
      return ArrayHlp.GetRange(array, Offset, Length);
    }
  }

  public class BinaryLink
  {
    public static void InitializeLinks(int packageBytesCount, params BinaryLink[] fieldLinks)
    {
      int offsetInBits = 0;
      foreach (BinaryLink field in fieldLinks)
      {
        field.byteOffset = offsetInBits / 8;
        field.bitOffset = offsetInBits % 8;
        offsetInBits += field.BitSize;
      }

      if (packageBytesCount * 8 != offsetInBits)
        throw new Exception(string.Format(
          "Длина бинарного пакета '{0}' не равна длине составляющих его полей '{1}'",
          packageBytesCount * 8, offsetInBits));
    }

    public readonly int BitSize;

    int byteOffset = -1;
    public int ByteOffset
    {
      get { return byteOffset; }
    }

    int bitOffset = -1;
    public int BitOffset
    {
      get { return bitOffset; }
    }

    public BinaryLink(int bitSize)
    {
      this.BitSize = bitSize;
    }

    public BinaryField ToField(uint value)
    {
      return new BinaryField(BitSize, value);
    }

    public BinaryField ToField(int value)
    {
      return ToField((uint)value);
    }
  }

  public struct BinaryField
  {
    public readonly int BitSize;
    public readonly uint Value;

    public BinaryField(int bitSize, uint value)
    {
      this.BitSize = bitSize;
      this.Value = value;
    }
  }

  public struct LEBinaryPackage
  {
    readonly byte[] bytes;
    public readonly int packageOffset;

    public uint Get(BinaryLink field)
    {
      int offset = packageOffset + field.ByteOffset;

      uint result;
      if (offset + 4 > bytes.Length)
      {
        byte[] bs = new byte[4];
        Array.Copy(bytes, offset, bs, 0, bytes.Length - offset);
        result = BitConverter.ToUInt32(bs, 0);
      }
      else
      {
        result = BitConverter.ToUInt32(bytes, offset);
      }

      return result << (32 - (field.BitOffset + field.BitSize)) >> (32 - field.BitSize);
      //return result << field.BitOffset >> (32 - (field.BitOffset + field.BitSize));
    }

    public int GetInt(BinaryLink field)
    {
      return (int)Get(field);
    }

    public byte GetByte(BinaryLink field)
    {
      return (byte)Get(field);
    }

    public ushort GetUShort(BinaryLink field)
    {
      return (ushort)Get(field);
    }

    public BinaryField GetField(BinaryLink field)
    {
      return new BinaryField(field.BitSize, this.Get(field));
    }

    public void Set(BinaryLink field, int value)
    {
      Set(field, (uint)value);
    }

    public void Set(BinaryLink field, uint value)
    {
      if (field.BitOffset != 0)
      {
        value = (uint)(value << field.BitOffset);
        byte firstOld = bytes[packageOffset + field.ByteOffset];
        firstOld = BinaryHlp.GetBits(firstOld, 0, field.BitOffset);
        value = (uint)(value | firstOld);
      }

      int lastBitOffset = (field.BitOffset + field.BitSize) % 8;
      if (lastBitOffset != 0)
      {
        int lastByteOffset = (field.BitOffset + field.BitSize) / 8;
        uint lastOld = bytes[packageOffset + field.ByteOffset + lastByteOffset];
        lastOld = (uint)(BinaryHlp.GetBits(lastOld, lastBitOffset, 8 - lastBitOffset) << (lastBitOffset + 24));
        value = (uint)(value | lastOld);
      }

      int byteSize = BinaryHlp.RoundUp(field.BitOffset + field.BitSize, 8);
      byte[] valueBytes = BitConverter.GetBytes(value);
      Array.Copy(valueBytes, 0, bytes, packageOffset + field.ByteOffset, byteSize);
    }

    public LEBinaryPackage(byte[] bytes, int packageOffset)
    {
      this.bytes = bytes;
      this.packageOffset = packageOffset;
    }

    public LEBinaryPackage(byte[] bytes) :
      this(bytes, 0)
    {
    }
  }

  public struct BinaryPackage
  {
    public static byte[] ToRawPackage(int packageBytesCount, params BinaryField[] packageFields)
    {
      byte[] bytes = new byte[packageBytesCount];
      int byteOffset = 0;
      int bitOffset = 0;
      foreach (BinaryField field in packageFields)
      {
        uint rest = field.Value;
        int bitSize = field.BitSize;
        {
          int bitRest = bitSize - (8 - bitOffset);
          if (bitRest > 0)
          {
            uint value = rest >> bitRest;
            bytes[byteOffset] += (byte)value;
            byteOffset++;
            bitOffset = 0;
            int shift = 32 - bitRest;
            rest = (rest << shift) >> shift;
            bitSize = bitRest;
          }
        }

        for (int i = 0; i < (bitSize >> 3); ++i)
        {
          int bitRest = bitSize - 8;
          uint value = rest >> bitRest;
          bytes[byteOffset] = (byte)value;
          byteOffset++;
          int shift = 32 - bitRest;
          rest = (rest << shift) >> shift;
          bitSize = bitRest;
        }

        if (bitSize != 0)
        {
          int shift = (8 - bitOffset) - bitSize;
          uint value = rest << shift;
          bytes[byteOffset] += (byte)value;
          bitOffset += bitSize;
          if (bitOffset == 8)
          {
            byteOffset++;
            bitOffset = 0;
          }
        }
      }

      return bytes;
    }

    readonly byte[] bytes;
    readonly int packageOffset;

    public uint Get(BinaryLink field)
    {
      int offset = packageOffset + field.ByteOffset;
      uint result = ((uint)bytes[offset]) << (24 + field.BitOffset) >> (32 - field.BitSize);
      int rest = field.BitSize - (8 - field.BitOffset);
      int i = 1;
      for (; i <= rest >> 3; ++i)
      {
        result += ((uint)bytes[offset + i]) << (rest - 8 * i);
      }
      rest = rest & 0x07;
      if (rest != 0)
        result += ((uint)bytes[offset + i]) >> (32 - rest);

      return result;
    }

    public int GetInt(BinaryLink field)
    {
      return (int)Get(field);
    }

    public byte GetByte(BinaryLink field)
    {
      return (byte)Get(field);
    }

    public ushort GetUShort(BinaryLink field)
    {
      return (ushort)Get(field);
    }

    public BinaryField GetField(BinaryLink field)
    {
      return new BinaryField(field.BitSize, this.Get(field));
    }

    public BinaryPackage(byte[] bytes, int packageOffset)
    {
      this.bytes = bytes;
      this.packageOffset = packageOffset;
    }

    public BinaryPackage(byte[] bytes) :
      this(bytes, 0)
    {
    }
  }
}
