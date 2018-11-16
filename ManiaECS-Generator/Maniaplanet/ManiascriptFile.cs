using System;
using System.Collections.Generic;
using System.Text;

namespace ManiaECS_Generator
{
    public class ManiaScriptFile
    {
        public string Path;
        public string OriginalPath;
        public string Text;

        private ManiaScriptFile()
        {
            throw new Exception();
        }

        public ManiaScriptFile(string path, string originalPath, string text)
        {
            Path = path.Replace('\\', '/');
            OriginalPath = originalPath.Replace('\\', '/');
            Text = text;
        }
    }
}
