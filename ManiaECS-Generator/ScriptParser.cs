using ManiaECS_Generator.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManiaECS_Generator
{
    /// <summary>
    /// A very basic parser
    /// </summary>
    public struct ScriptParser
    {
        public enum ParserStatus
        {
            /// <summary>
            /// We are currently doing nothing, except searching for stuff
            /// </summary>
            None,
            /// <summary>
            /// We are currently searching for the struct tag (#Struct)
            /// </summary>
            SearchStructTag,
            /// <summary>
            /// We are currently writing the struct tag (#Struct) to the result
            /// </summary>
            WriteStructTag,
            /// <summary>
            /// We are currently searching for the name of the struct (after #Struct)
            /// </summary>
            SearchName,
            /// <summary>
            /// We are currently writing the name of the struct to the result
            /// </summary>
            WriteName,
            /// <summary>
            /// We finished writing the struct
            /// </summary>
            FinishWriting
        }

        /// <summary>
        /// The content that was given for parsing.
        /// </summary>
        public readonly string Content;

        /// <summary>
        /// The include that we found in the <see cref="Content"/> variable.
        /// </summary>
        public string FoundIncludes;

        /// <summary>
        /// All the structures found from the <see cref="Content"/> variable.
        /// </summary>
        public List<ManiaPlanetStruct> Structures;

        /// <summary>
        /// A proper constructor.
        /// </summary>
        /// <param name="content">The content that will be used for parsing</param>
        public ScriptParser(string content)
        {
            Content = content;
            Structures = new List<ManiaPlanetStruct>(8);
            FoundIncludes = null;
        }

        /// <summary>
        /// Parse <see cref="Content"/>.
        /// </summary>
        /// <returns>Return true if the parsing was successful.</returns>
        public bool Parse()
        {
            // This shouldn't happen in a perfect world.
            if (Content == null) return false;

            // Initialize variables to default values
            var str = string.Empty;
            var structure = default(ManiaPlanetStruct);
            var status = ParserStatus.None;

            // Loop into the content string
            for (int i = 0; i != Content.Length; i++)
            {
                var ch = Content[i];

                // We are only allowed to add the char to the string 'str' if we are in a 'none' status.
                if (status != ParserStatus.None) str += ch;
                else if (ch == '#')
                {
                    // We found a '#' char, so we start searching for a struct
                    status = ParserStatus.SearchStructTag;
                    str += ch;

                    continue;
                }

                // This will be done later
                if (str.EndsWith("#Include") && status == ParserStatus.SearchStructTag)
                {
                    // todo

                    continue;
                }
                
                // If we found a struct tag, and that we currently are searching for a struct tag and we have no structure
                // in work, we begin our work for this new structure.
                else if (str.EndsWith("#Struct") && structure == null && status == ParserStatus.SearchStructTag)
                {
                    structure = new ManiaPlanetStruct();
                    structure.Data += "#Struct";
                    str = string.Empty;

                    status = ParserStatus.SearchName;

                    continue;
                }

                // We don't make structure related work if we have no structure.
                if (structure == null) continue;
                
                // If we have no name and we are currently searching, we switch the status to 'WriteName'
                if (structure.Name == null && !string.IsNullOrWhiteSpace(str) && status == ParserStatus.SearchName)
                {
                    structure.Name = ch.ToString();
                    structure.Data += str;

                    str = string.Empty;

                    status = ParserStatus.WriteName;
                }
                // If we are currently writing the name, we write it to the result
                else if (status == ParserStatus.WriteName)
                {
                    structure.Data += str;

                    // If it's a whitespace ' ' or a opening bracket '{', we finish writing the name to the result.
                    if (string.IsNullOrWhiteSpace(str) || ch == '{')
                    {
                        str = string.Empty;

                        status = ParserStatus.FinishWriting;

                        continue;
                    }

                    structure.Name += str;

                    str = string.Empty;
                }
                // We are now finishing writing the struct to the result
                else if (status == ParserStatus.FinishWriting)
                {
                    structure.Data += str;

                    // If it's a closed bracket '}', we finished writing the struct.
                    if (ch == '}')
                    {
                        //Logger.WriteInfo("Parser", $"{structure.Data}", new LogOption(true, ConsoleColor.DarkGreen));

                        Structures.Add(structure);
                        structure = null;

                        status = ParserStatus.None;
                    }

                    str = string.Empty;
                }
            }

            return true;
        }
    }
}
