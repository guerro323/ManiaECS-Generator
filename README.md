# ManiaECS Generator

An utility to automatically generate ECS related files for maniascript related things.

Why was this tool made?
- Each time I create a component structure, I needed to modify the file for it.
- Each time I needed to modify a little feature, I needed to redo everything.
    

---
### Command Arguments:  

`-i "path"` Set the input path, all files will be scanned inside of the folder.   
`-o "path"` Set the output path, all files that was created inside the application will be in the folder.  
`-t "path"` The path to the template folder.  

---
### How I can use this tool?

First, you need to make your components structures, let's create one for the example.

`{Input Path}` is the path to the Input folder.

`{Input Path}/Libs/ComponentsDefinition/ScoreData.Script.txt`
```ruby
#Struct ScoreData
{
    Integer RoundPoints;
    Integer Points;

    Real RatioKd;
}
```

Now, run the program by opening the command line, and enter:  
`dotnet ManiaECS-Generator.dll {arguments}`

If you have the input, output and template folder unchanged, don't add any arguments.

In the end, the program will generate a file like that:
```csharp
// This script was automatically generated

#Include "Libs/ECS/EntityManager.Script.txt" as EntityManager
#Include "Libs/Nerpson/MsUnit.Script.txt" as MsUnit
#Include "Libs/ComponentsDefinition/ScoreData.Script.txt" as Lib_ScoreData

#Struct Lib_ScoreData::ScoreData as ScoreData
#Struct EntityManager::Entity as SEntity
/// --------------- --------------- --------------- --------------- ///
/// ScoreData
/// --------------- --------------- --------------- --------------- ///

declare ScoreData[Integer] G_ComponentsScoreData;

// Template from dynamic script: No

ScoreData Get(Integer _Index, ScoreData _Empty)
{
    MsUnit::AssertTrue(EntityManager::Exists(_Index), "EntityManager::Exists(" ^ _Index ^ ")");

    return G_ComponentsScoreData[_Index];
}

ScoreData Get(SEntity _Entity, ScoreData _Empty)
{
    return Get(_Entity.Index, _Empty);
}

// Template from dynamic script: No

Void Set(Integer _Index, ScoreData _Value)
{
    MsUnit::AssertTrue(EntityManager::Exists(_Index), "EntityManager::Exists(" ^ _Index ^ ")");

    G_ComponentsScoreData[_Index] = _Value;
}

Void Set(SEntity _Entity, ScoreData _Value)
{
    Set(_Entity.Index, _Value);
}

// Template from dynamic script: No

Boolean Has(Integer _Index, ScoreData _Empty)
{
    MsUnit::AssertTrue(EntityManager::Exists(_Index), "EntityManager::Exists(" ^ _Index ^ ")");

    return G_ComponentsScoreData.existskey(_Index);
}

Boolean Has(SEntity _Entity, ScoreData _Empty)
{
    return Has(_Entity.Index, _Empty);
}
```

---
### How I can use ECS in maniascript?

todo