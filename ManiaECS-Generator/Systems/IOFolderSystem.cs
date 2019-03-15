using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ManiaECS_Generator.Systems
{
    public class IOFolderSystem : SystemBase
    {
        private string inputDirectory, outputDirectory;

        public string InputDirectory => inputDirectory;
        public string OutputDirectory => outputDirectory;

        protected override void OnGeneratorPass(bool calledOneTime)
        {
            // don't care
        }

        public void SetDirectories(string input, string output)
        {
            inputDirectory = input;
            outputDirectory = output;

            if (!Directory.Exists(input))
            {
                Logger.WriteInfo("Attention!", $"A new input directory will be created: {input}");

                Directory.CreateDirectory(input);
            }

            if (!Directory.Exists(output))
            {
                Logger.WriteInfo("Attention!", $"A new output directory will be created: {output}");

                Directory.CreateDirectory(output);
            }
        }

        public string[] GetScriptsFromPath(string path, string ext = "*")
        {
            return Directory.EnumerateFiles(path, $"*.{ext}", SearchOption.AllDirectories).ToArray();
        }
    }
}
