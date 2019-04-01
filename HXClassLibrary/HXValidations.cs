using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HXClassLibrary
{
    //Validations Class for validating and formating attributes
    public static class HXValidations
    {

        // capitalize the first character of each word in a string
        public static string HXCapitalize(string value)
        {
            if (value == null)
            {
                value = "";
            }
            else
            {
                value = value.ToLower().Trim();
            }
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            value = textInfo.ToTitleCase(value);
            return value;
        }

        //extract all digits from a string
        public static string HXExtractDigits(string value)
        {
            string phoneNumber = "";
            if (value == null)
            {
                value = "";
            }
            var result = value.Where(char.IsDigit).ToArray();
            foreach (char item in result)
            {
                phoneNumber += item;
            }
            //string s = Regex.Replace("(123) 455-2344", @"\D",""); //return only numbers from string
            return phoneNumber;
        }

        //validate the postal code in Canada
        public static Boolean HXPostalCodeValidation(string value)
        {
            Regex pattern = new Regex(@"^([ABCEGHJKLMNPRSTVXY]\d[ABCEGHJKLMNPRSTVWXYZ])\ {0,1}(\d[ABCEGHJKLMNPRSTVWXYZ]\d)$", RegexOptions.IgnoreCase);
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            if (pattern.IsMatch(value))
                return true;
            else
                return false;
        }

        //format the postal code that was input
        public static string HXPostalCodeFormat(string value)
        {
            if (value == null)
            {
                value = "";
            }
            value = value.Trim();
            if (value.Contains(" "))
            {
                value = value.Insert(3, " ").ToUpper();
            }
            return value;
        }


        //validate the zip code in US
        public static Boolean HXZipCodeValidation (ref string value)
        {
            string valueDigits = HXExtractDigits(value);
            if (string.IsNullOrEmpty(value))
            {
                value = "";
                return true;
            }
            else if (valueDigits.Length == 5)
            {
                value = valueDigits;
                return true;
            }
            else if (valueDigits.Length == 9)
            {
                valueDigits = valueDigits.Insert(5, "-");
                value = valueDigits;
                return true;
            }
            else
                return false;

        }
    }
}
