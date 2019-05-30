using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManiaECS_Generator.Systems
{
    public class InputDataSystem : SystemBase
    {
        public List<ManiaPlanetStruct> AllStructures;

        public InputDataSystem()
        {
            AllStructures = new List<ManiaPlanetStruct>();
        }

        protected override void OnGeneratorPass(bool calledOneTime)
        {
            // don't care
        }

        public ManiaPlanetStruct[] AddFile(ManiaScriptFile file)
        {
            var parser = new ScriptParser(file.Text);
            Assert.IsTrue(parser.Parse(), $"parser.Parse({file.Path})");

            for (int i = 0; i != parser.Structures.Count; i++)
            {
                var structure = parser.Structures[i];
                structure.OriginalManiaScriptFile = file;
                
                var info = string.Empty;
                info += $"\tName={structure.Name}\n";
                info += $"\tNickname={structure.Nickname}\n";
                info += $"\tOriginalType={structure.OriginalType}\n";
                info += $"\tFile={structure.OriginalManiaScriptFile?.Path}\n";
                        
                Logger.WriteInfo("Parser", $"Finished writing struct ({structure.Name})\n{{\n{info}}}", new LogOption(ConsoleColor.Green));

                AllStructures.Add(structure);
                parser.Structures[i] = structure;
            }

            return parser.Structures.ToArray();
        }

        /*public ManiaPlanetStruct AddInput(string input)
        {
            var maniaplanetStruct = new ManiaPlanetStruct
            {
                Render = input
            };

            AllStructures.Add(maniaplanetStruct);

            return maniaplanetStruct;
        }*/

        public void AddInputFromZero(Type type, string outputStructName = null)
        {
            throw new NotImplementedException("TODO");

            if (outputStructName == null) outputStructName = type.Name;

            AllStructures.Add(ConstructStruct(type, outputStructName));

            Logger.WriteInfo("Input System", "Added struct " + outputStructName);
        }

        public ManiaPlanetStruct ConstructStruct(Type type, string outputStructName)
        {
            throw new NotImplementedException("TODO");

            var maniaplanetStruct = new ManiaPlanetStruct { OriginalType = type };
            var render = string.Empty;

            render = $@"#Struct {outputStructName} 
{{";



            return maniaplanetStruct;
        }
    }
}
