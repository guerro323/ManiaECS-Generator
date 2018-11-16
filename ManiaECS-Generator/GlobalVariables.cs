using System;
using System.Collections.Generic;
using System.Text;

namespace ManiaECS_Generator
{
    public static class GlobalVariables
    {
        public const string LibraryMsUnitPathKey = "MsUnitPath";
        public const string LibraryMsUnitPathDefaultValue = "Libs/Nerpson/MsUnit.Script.txt";

        public static Dictionary<string, string> Values;

        static GlobalVariables()
        {
            Values = new Dictionary<string, string>();

            Set(LibraryMsUnitPathKey, LibraryMsUnitPathDefaultValue);
        }

        public static void Set(string key, string value)
        {
            Values[key] = value;
        }
    }
}
