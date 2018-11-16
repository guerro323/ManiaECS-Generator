using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ManiaECS_Generator.Systems
{
    public class ScriptFileSystem : SystemBase
    {
        public List<ManiaScriptFile> AllFiles;

        public ScriptFileSystem()
        {
            AllFiles = new List<ManiaScriptFile>();
        }

        protected override void OnGeneratorPass(bool calledOneTime)
        {
        }

        public ManiaScriptFile AddFile(ManiaScriptFile file)
        {
            AllFiles.Add(file);

            Logger.WriteInfo("File System", "Added Maniascript file: " + file.Path);

            return file;
        }

        public ManiaScriptFile AddFile(string stripPath, string path)
        {
            var msScript = new ManiaScriptFile(stripPath, path, File.ReadAllText(path));

            AddFile(msScript);

            return msScript;
        }
    }
}
