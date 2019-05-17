# ManiaECS Generator (HEAVY WIP)

An utility to automatically generate ECS related files for maniascript related things.

Requirements:
- You need to download MsUnit from Nerpson: https://github.com/Nerpson/ManiaScripts/blob/master/Libs/MsUnit.Script.txt

To do list (most important):
- Component Type Manager
- Add Archetypes (it would help a lot for querying entities with better performance)
- Renaming most of the functions to be less 'generic' (ex: Has(SPosition {}) -> Has_SPosition())
- performance performance
- cleaner generation result

I also got interested in making a C# to maniascript transcompiler (with basic functionalities) with this project in mind.

---
### Command Arguments:  

`-i "path"` Set the input path, all files will be scanned inside of the folder.   
`-o "path"` Set the output path, all files that was created inside the application will be in the folder.  
`-t "path"` The path to the template folder.  

---
### How I can use this tool?

See [Quick Start](Documentations/quick_start.md)


[Documentation](Documentations/)

### Is there any real (trackmania/shootmania) gamemode made with ECS?

Yes, I'm currently making an Assault gamemode with the ECS paradigm, once it will be a bit cleaned, I will post the link here.