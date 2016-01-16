#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion
using System.Collections;

namespace Commune.Basis
{
  public class StringHlp
  {
    #region Latin
    public static string Latin(string str)
    {
      str = str.Replace('à', 'a');
      str = str.Replace('á', 'a');
      str = str.Replace('â', 'a');
      str = str.Replace('ã', 'a');
      str = str.Replace('ä', 'a');
      str = str.Replace('å', 'a');
      str = str.Replace('è', 'e');
      str = str.Replace('é', 'e');
      str = str.Replace('ê', 'e');
      str = str.Replace('ë', 'e');
      str = str.Replace('ì', 'i');
      str = str.Replace('í', 'i');
      str = str.Replace('î', 'i');
      str = str.Replace('ï', 'i');
      str = str.Replace('ò', 'o');
      str = str.Replace('ó', 'o');
      str = str.Replace('ô', 'o');
      str = str.Replace('õ', 'o');
      str = str.Replace('ö', 'o');

      str = str.Replace('ù', 'u');
      str = str.Replace('ú', 'u');
      str = str.Replace('û', 'u');
      str = str.Replace('ü', 'u');

      str = str.Replace('À', 'A');
      str = str.Replace('Á', 'A');
      str = str.Replace('Â', 'A');
      str = str.Replace('Ã', 'A');
      str = str.Replace('Ä', 'A');
      str = str.Replace('Å', 'A');
      str = str.Replace('È', 'E');
      str = str.Replace('É', 'E');
      str = str.Replace('Ê', 'E');
      str = str.Replace('Ë', 'E');
      str = str.Replace('Ì', 'I');
      str = str.Replace('Í', 'I');
      str = str.Replace('Î', 'I');
      str = str.Replace('Ï', 'I');
      str = str.Replace('Ò', 'O');
      str = str.Replace('Ó', 'O');
      str = str.Replace('Ô', 'O');
      str = str.Replace('Õ', 'O');
      str = str.Replace('Ö', 'O');
      str = str.Replace('Ù', 'U');
      str = str.Replace('Ú', 'U');
      str = str.Replace('Û', 'U');
      str = str.Replace('Ü', 'U');
      return str;
  }
      #endregion

    #region Translit
    /// <summary>
    /// Очень не оптимальная
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Translit(string str)
    {
      str = str.Replace("а", "a");
      str = str.Replace("б", "b");
      str = str.Replace("в", "v");
      str = str.Replace("г", "g");
      str = str.Replace("д", "d");
      str = str.Replace("е", "e");
      str = str.Replace("ё", "e");
      str = str.Replace("ж", "zh");
      str = str.Replace("з", "z");
      str = str.Replace("и", "i");
      str = str.Replace("й", "y");
      str = str.Replace("к", "k");
      str = str.Replace("л", "l");
      str = str.Replace("м", "m");
      str = str.Replace("н", "n");
      str = str.Replace("о", "o");
      str = str.Replace("п", "p");
      str = str.Replace("р", "r");
      str = str.Replace("с", "s");
      str = str.Replace("т", "t");
      str = str.Replace("у", "u");
      str = str.Replace("ф", "f");
      str = str.Replace("х", "kh");
      str = str.Replace("ц", "ts");
      str = str.Replace("ч", "ch");
      str = str.Replace("ш", "sh");
      str = str.Replace("щ", "sch");
      str = str.Replace("ъ", "`");
      str = str.Replace("ы", "y");
      str = str.Replace("ь", "`");
      str = str.Replace("э", "e");
      str = str.Replace("ю", "yu");
      str = str.Replace("я", "ya");
      str = str.Replace("А", "A");
      str = str.Replace("Б", "B");
      str = str.Replace("В", "V");
      str = str.Replace("Г", "G");
      str = str.Replace("Д", "D");
      str = str.Replace("Е", "E");
      str = str.Replace("Ё", "E");
      str = str.Replace("Ж", "Zh");
      str = str.Replace("З", "Z");
      str = str.Replace("И", "I");
      str = str.Replace("Й", "Y");
      str = str.Replace("К", "K");
      str = str.Replace("Л", "L");
      str = str.Replace("М", "M");
      str = str.Replace("Н", "N");
      str = str.Replace("О", "O");
      str = str.Replace("П", "P");
      str = str.Replace("Р", "R");
      str = str.Replace("С", "S");
      str = str.Replace("Т", "T");
      str = str.Replace("У", "U");
      str = str.Replace("Ф", "F");
      str = str.Replace("Х", "Kh");
      str = str.Replace("Ц", "Ts");
      str = str.Replace("Ч", "Ch");
      str = str.Replace("Ш", "Sh");
      str = str.Replace("Щ", "Sch");
      str = str.Replace("Ъ", "`");
      str = str.Replace("Ы", "Y");
      str = str.Replace("Ь", "`");
      str = str.Replace("Э", "E");
      str = str.Replace("Ю", "Yu");
      str = str.Replace("Я", "Ya");
      return str;
  }
  #endregion

    public static string Join<T>(string separator, IEnumerable items, Getter<string, T> toStringConverter)
    {
      List<string> strs = new List<string>();
      foreach (T item in items)
        strs.Add(toStringConverter(item));

      return string.Join(separator, strs.ToArray());
    }

    public static string Join(string separator, string format, IEnumerable items)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        bool isFirst = true;
        foreach (object item in items)
        {
            if (!isFirst)
                builder.Append(separator);
            else
                isFirst = false;
            builder.AppendFormat(format, item);
        }
        return builder.ToString();
    }

    public static bool IsEmpty(string s)
    {
      return s == null || s.Length == 0;
    }
    public static string ToString(object item)
    {
      if (item == null)
        return null;
      return item.ToString();
    }

    public static bool Contains(string what, ICollection<string> where)
    {
        foreach (string item in where)
            if (what == item)
                return true;
        return false;
    }

    public static bool ContainsAny(ICollection<string> what, ICollection<string> where)
    {
        foreach (string item in what)
            foreach (string whereItem in where)
                if (item == whereItem)
                    return true;
        return false;
    }
    public static string Reverse(string s)
    {
        List<char> arr = new List<char>(s.ToCharArray());
        arr.Reverse();
        return new String(arr.ToArray());
    }

    public static string Koi8ToUnicode(string s)
    {
        //20866 - koi8r
        //1251 - windows
        //866 -  dos


        byte[] koi8_bytes = new byte[s.Length];
        /* 
         * Encoding enc = new Encoding(1251);
         * koi8_bytes = enc.GetBytes(s);
         */
        Encoder enc = Encoding.GetEncoding(1251).GetEncoder();
        enc.GetBytes(s.ToCharArray(), 0, s.Length, koi8_bytes, 0, true);

        char[] uni_chars = new char[s.Length];
        Decoder dec = Encoding.GetEncoding(20866).GetDecoder();
        dec.GetChars(koi8_bytes, 0, s.Length, uni_chars, 0);
        return new string(uni_chars);

    }

    public static string NotNull(string s)
    {
        if (s != null)
            return s;
        return "";
    }

    public static string ToCamel(string str)
    {
      if (str.Length > 0)
        return char.ToUpperInvariant(str[0]) + str.Substring(1).ToLowerInvariant();
      return "";
    }

    /// <summary>
    /// Возвращает string.Intern(str), если str не равно null, и null в противном случае.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Intern(string str)
    {
      return str != null ? string.Intern(str) : null;
    }
    /// <summary>
    /// Ишет в строке text строку value.
    /// </summary>
    /// <param name="text">Строка</param>
    /// <param name="value">Подстрока</param>
    /// <param name="ignoreCase">Игнорировать регистр</param>
    /// <returns></returns>
    public static bool Contains(string text, string value, bool ignoreCase)
    {
      text = StringHlp.NotNull(text);
      value = StringHlp.NotNull(value);
      if (ignoreCase)
      {
        text = text.ToLower();
        value = value.ToLower();
      }
      return text.Contains(value);
    }
    public static bool Contains(string text, string value)
    {
      return Contains(text, value, false);
    }
    public static string JoinNotEmpty(string separator, params string[] strings)
    {
      return string.Join(separator, Array.FindAll(strings,
        delegate(string str) { return !string.IsNullOrEmpty(str); }));
    }

    static string NormalizeWhiteSpace(string str)
    {
      return string.Join(" ", str.Split(Array<char>.Empty, StringSplitOptions.RemoveEmptyEntries));
    }

    private static string Transform(string str, int transformLength, Getter<string, string> transformer)
    {
      if (str.Length < transformLength)
        transformLength = str.Length;
      char[] chArray1 = str.ToCharArray();
      char[] chArray2 = transformer(new string(chArray1, 0, transformLength)).ToCharArray();
      Array.Copy(chArray2, chArray1, chArray2.Length);
      return new string(chArray1);
    }

    public static string ToLower(string str, int lowerLength)
    {
      return StringHlp.Transform(str, lowerLength, (Getter<string, string>)(s => s.ToLower()));
    }

    public static string ToUpper(string str, int upperLength)
    {
      return StringHlp.Transform(str, upperLength, (Getter<string, string>)(s => s.ToUpper()));
    }

  }
}
