using System;
using System.Collections.Generic;
using System.Text;

namespace Automation_Framework.Utilities
{
    public static class StringUtils
    {
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        public static string substringBetween(string attr, string a, string b)
        {
            return attr.Between(a, b);
        }
        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string Before(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }

        /// <summary>
        /// This method will throw an exception if the length is greater than 11, if we run into this then update 
        /// the method so that we call the GetRandomFileName multiple times to past the required lenth.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomWord(int length)
        {
            //string path = System.IO.Path.GetRandomFileName();
            //path = path.Replace(".", ""); // Remove period.
            //return path.Substring(0, length);
            return Automation_Framework.Helpers.Randomness.RandomStrings.GenerateRandomWord(length);
        }

        public static string concat(this string value, string a )
        {
            return value + a;
        }
    }
}
