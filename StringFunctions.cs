using System;
using System.Globalization;
using System.Linq;

/// <summary>
/// Класс дополнительных функций для строковых переменных
/// </summary>
public static class StringFunctions
{
    private const char DateDay = 'd';
    private const char DateMonth = 'M';
    private const char DateYear = 'y';

    public static string GetClearlyDefinedString(string s)
    {
        return s.Aggregate("", (current, c) => current + GetClearDefinedStringChar(c));
    }

    public static DateTime GetDate(string s, string format)
    {
        int day = 1, month = 1;
        string sYear = "";
        string rS = Reverse(s);
        string rFormat = Reverse(format);
        string predicat = "";
        bool first = true;
        int length = format.Length > s.Length
                             ? s.Length
                             : format.Length;
        for (int i = 0; i < length; i++)
        {
            if (first)
            {
                predicat += rFormat[i];
                first = false;
            }
            else
            {
                if (rFormat[i] == predicat[0])
                {
                    predicat += rFormat[i];
                }
                else
                {
                    SetDayMonthYear(predicat, rS, ref day, ref month, ref sYear);
                    if (predicat.Length > 0)
                    {
                        rS = rS.Substring(predicat.Length);
                    }
                    predicat = rFormat[i].ToString();
                }
                if (i >= length - 1)
                {
                    SetDayMonthYear(predicat, rS, ref day, ref month, ref sYear);
                }
            }
        }
        if (sYear.Length == 1)
        {
            sYear = string.Format("200{0}", sYear);
        }
        if (sYear.Length == 2)
        {
            sYear = string.Format("20{0}", sYear);
        }
        if (sYear.Length == 3)
        {
            sYear = string.Format("2{0}", sYear);
        }
        int year = int.Parse(sYear);
        return new DateTime(year, month, day);
    }

    public static string Reverse(string s)
    {
        string result = "";
        for (int i = s.Length - 1; i >= 0; i--)
        {
            result += s[i];
        }
        return result;
    }

    public static double? TryParse(string s)
    {
        double d;
        bool tryParse = double.TryParse(s.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d);
        if (tryParse)
        {
            return d;
        }
        return null;
    }

    private static string GetClearDefinedStringChar(char c)
    {
        string ss = c.ToString().ToUpper();
        if (string.IsNullOrWhiteSpace(ss))
        {
            return " ";
        }
        switch (ss)
        {
            case "К":
                return "K";
            case "Е":
                return "E";
            case "Н":
                return "H";
            case "З":
                return "3";
            case "Х":
                return "X";
            case "В":
                return "B";
            case "А":
                return "A";
            case "Р":
                return "P";
            case "О":
                return "O";
            case "С":
                return "C";
            case "М":
                return "M";
            case "Т":
                return "T";
            case "1":
                return "L";
            case "8":
                return "S";
            case "0":
                return "O";
        }
        return ss;
    }

    private static void SetDayMonthYear(string predicat, string reverseDate, ref int day, ref int month, ref string sYear)
    {
        switch (predicat[0])
        {
            case DateDay:
                day = int.Parse(Reverse(reverseDate.Substring(0, predicat.Length)));
                break;
            case DateMonth:
                month = int.Parse(Reverse(reverseDate.Substring(0, predicat.Length)));
                break;
            case DateYear:
                sYear = Reverse(reverseDate.Substring(0, predicat.Length));
                break;
        }
    }
}

