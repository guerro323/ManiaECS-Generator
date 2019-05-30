using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ManiaECS_Generator.Systems
{
    public struct FileScriptTemplate
    {
        public string Content;
        public bool IsDynScript;
    }

    public class CreateComponentFileSystem : SystemBase
    {
        public const string _TplTypeDef = "_TplTypeDef";
        public const string _TplTypeCompList = "_TplTypeCompList";
        public const string _EntityManager_EntityType = "_EntityManager_EntityType";

        private string templateDirectory;
        private string ecsLibraryPath;

        public string TemplateDirectory
        {
            get => templateDirectory;
            set
            {
                templateDirectory = value;

                AllTemplates.Clear();
                foreach (var filePath in Directory.EnumerateFiles(templateDirectory, "*.*", SearchOption.AllDirectories))
                {
                    AllTemplates.Add(new FileScriptTemplate
                    {
                        Content = File.ReadAllText(filePath),
                        IsDynScript = filePath.EndsWith(".cs")
                    });
                }
            }
        }

        public string EcsLibraryPath
        {
            get => ecsLibraryPath;
            set
            {
                ecsLibraryPath = value;
            }
        }

        public string Prefix;

        public List<FileScriptTemplate> AllTemplates;

        public CreateComponentFileSystem()
        {
            AllTemplates = new List<FileScriptTemplate>();
        }

        protected override void OnGeneratorPass(bool calledOneTime)
        {
            // don't care
        }

        public string CreateContent(IEnumerable<ManiaPlanetStruct> structs, string structHeader, string bottom)
        {
            Assert.IsTrue(ecsLibraryPath != null, "ecsLibraryPath != null");
            Assert.IsTrue(structs != null, "structs != null");

            var str = "// This script was automatically generated\n\n";

            // Write Libraries
            str += $"#Include \"{ecsLibraryPath}/EntityManager.Script.txt\" as EntityManager\n";

            // Get librairies information from templates
            foreach (var template in AllTemplates)
            {
                var content = template.Content;

                foreach (var globalVar in GlobalVariables.Values)
                {
                    content = content.Replace($"_Globals[{globalVar.Key}]", globalVar.Value);
                }

                using (var reader = new StringReader(content))
                {
                    string line;
                    while((line = reader.ReadLine()) != null)
                    {
                        var isInclude = line.StartsWith("#Include");

                        if (isInclude && !str.Contains(line)) str += $"{line}\n";
                    }
                }
            }

            // --------------- --------------- --------------- --------------- //
            // Write Aliases
            // --------------- --------------- --------------- --------------- //
            var strStructLibrary = string.Empty;
            var strStructAliases = string.Empty;
            foreach (var mpStruct in structs)
            {
                var scriptPath = mpStruct.OriginalManiaScriptFile.Path;
                // TODO: In future, we should directly remove everything after a dot.
                var fileName = Path.GetFileNameWithoutExtension(scriptPath).Replace(".Script", "");
                if (scriptPath[0] == '\\' || scriptPath[0] == '/')
                {
                    scriptPath = scriptPath.Remove(0, 1);
                }

                var libraryStrToAdd = $"#Include \"{Prefix + scriptPath}\" as {GetLibName(fileName)}\n";

                // Avoid duplicate
                if (!strStructLibrary.Contains(libraryStrToAdd)) strStructLibrary += libraryStrToAdd;

                // Add aliase
                strStructAliases += $"#Struct {GetLibName(fileName)}::{mpStruct.Name} as {mpStruct.Name}\n";
            }

            strStructAliases += "#Struct EntityManager::SEntity as SEntity";

            str += strStructLibrary + "\n";
            str += strStructAliases + "\n";

            str += "// Struct Header generation...\n\n";
            str += structHeader;
            str += "\n";

            str += "declare Integer G_RuntimeVersion;";
            str += "\n";

            var destroyFunction = string.Empty;

            foreach (var mpStruct in structs)
            {
                var listName = GetComponentListVarName(mpStruct.Name);
                destroyFunction += $"\tif ({listName}.existskey(_Entity.Index)) {listName}.removekey(_Entity.Index);\n";
            }

            str += $@"

// ---------------------------------- //
/** Destroy an entity with along its remaining components
 *
 *	@param	_Entity		The entity
*/
Void Destroy(SEntity _Entity) 
{{
    EntityManager::Destroy(_Entity); 

{destroyFunction}}}

";

            foreach (var mpStruct in structs)
            {
                // --------------- --------------- --------------- --------------- //
                // Write Header
                // --------------- --------------- --------------- --------------- //
                str += "/// --------------- --------------- --------------- --------------- ///\n";
                str += $"/// {mpStruct.Name}\n";
                str += "/// --------------- --------------- --------------- --------------- ///\n";

                // --------------- --------------- --------------- --------------- //
                // Write component list
                // --------------- --------------- --------------- --------------- //
                str += $"declare {mpStruct.Name}[Integer] {GetComponentListVarName(mpStruct.Name)};\n";
                str += $"declare Integer G_{mpStruct.Name}Version;\n";
                str += $"declare Integer G_{mpStruct.Name}Version_Data;";
                str += "\n";

                // --------------- --------------- --------------- --------------- //
                // Write Templates
                // --------------- --------------- --------------- --------------- //
                foreach (var tpl in AllTemplates)
                {
                    //str += $"// Template from dynamic script: {(tpl.IsDynScript ? "Yes": "No")}\n";
                    WriteTemplate(ref str, tpl, mpStruct);
                    str += "\n";
                }
            }
            
            str += "// Bottom generation...\n";
            str += bottom;
            str += "\n";

            Logger.WriteInfo("Create Component File", "Component file created.", new LogOption(ConsoleColor.DarkMagenta));

            return str;
        }

        public void WriteTemplate(ref string str, FileScriptTemplate template, ManiaPlanetStruct maniaPlanetStruct)
        {
            if (template.IsDynScript) throw new NotImplementedException("Dynamic scripts are not yet implemented for templates");

            var content = template.Content; // copy

            foreach (var globalVar in GlobalVariables.Values)
            {
                content = content.Replace($"_Globals[{globalVar.Key}]", globalVar.Value);
            }

            using (var reader = new StringReader(content))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var isInclude = line.StartsWith("#Include");

                    if (isInclude) { content = content.Replace(line, string.Empty); }
                }
            }

            str += content
                .Replace(_TplTypeDef, maniaPlanetStruct.Name)
                .Replace(_TplTypeCompList, GetComponentListVarName(maniaPlanetStruct.Name))
                .Replace(_EntityManager_EntityType, "SEntity");
        }

        public string GetLibName(string fileName) => $"Lib_{fileName}";
        public string GetComponentListVarName(string structName) => $"G_Components{structName}";
    }
}
