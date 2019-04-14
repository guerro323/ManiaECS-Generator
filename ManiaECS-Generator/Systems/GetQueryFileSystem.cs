using System;
using System.IO;

namespace ManiaECS_Generator.Systems
{
	public class GetQueryFileSystem : SystemBase
	{		
		protected override void OnGeneratorPass(bool calledOneTime)
		{
			
		}

		public (string structHeader, string bottom) GetQueryResult()
		{
			var inputDirectory = World.GetSystem<IOFolderSystem>().InputDirectory;
			if (!File.Exists(inputDirectory + "/query.txt"))
				return (null, null);
			
			Logger.WriteInfo("Query", "Started getting queries...");

			var lines = File.ReadAllLines(inputDirectory + "/query.txt");
			string structHeader = string.Empty, bottom = string.Empty;

			var lineIdx = 0;
			foreach (var str in lines)
			{
				var varQueryVersionName = $"QueryVersion_{lineIdx}";
				
				var structNames = str.Replace(" ", string.Empty)
				                     .Split(',', StringSplitOptions.RemoveEmptyEntries);
				var headerInside = string.Empty;
				var functionArgument = string.Empty;
				var queryStruct = string.Empty;
				var condition = string.Empty;
				var getData = string.Empty;
				var checkVersion = string.Empty;
				var function = string.Empty;
				
				queryStruct += "#Struct QueryResult";

				var structFound = 0;
				for (var i = 0; i < structNames.Length; i++)
				{
					var name = structNames[i];
					if (name.StartsWith("//"))
						continue;
						
					queryStruct      += $"_{name}";
					headerInside     += $"\n{name} _{name};";
					functionArgument += $"{name} _{i}, ";
					condition += $"&& Has(entity, {name} {{}})";
					getData += $"if (Has(entity, {name} {{}})) queryResult._{name} = Get(entity, {name} {{}}); else continue;\n\t\t";
					checkVersion += $"if ({varQueryVersionName} < G_{name}Version) rebuild = True;\n";
					
					structFound++;
				}

				if (structFound == 0)
					continue;

				var queryName = queryStruct.Replace("#Struct ", string.Empty);

				function += $"\n//\n// Query {lineIdx}:{queryName} \n//\n";
				function += $"declare {queryName}[] Cached_{queryName};\n";
				function += $"declare SEntity[] Cached_Entities_{queryName};\n";
				function += $"declare Integer {varQueryVersionName};\n";
				function += $"declare Boolean Rebuild_GetAllEntities_{queryName};\n";
				function += $"declare Boolean Rebuild_GetAllData_{queryName};\n";
				function += $@"SEntity[] GetAllEntities({functionArgument.Remove(functionArgument.Length - 2, 2)})
{{
	declare rebuild = False;
	{checkVersion}

	if (!rebuild && !Rebuild_GetAllEntities_{queryName})
		return Cached_Entities_{queryName};

	{varQueryVersionName} = G_RuntimeVersion;

	Rebuild_GetAllData_{queryName} = True;
	Rebuild_GetAllEntities_{queryName} = False;

	Cached_Entities_{queryName}.clear();

	declare entities = EntityManager::GetEntities();
	foreach (entity in entities)
	{{
		if (True {condition})
		{{
			Cached_Entities_{queryName}.add(entity);
		}}
	}}

	return Cached_Entities_{queryName};
}}

";
				function += $@"{queryName}[] GetAllData({functionArgument.Remove(functionArgument.Length - 2, 2)})
{{
	declare rebuild = False;
	{checkVersion}

	if (!rebuild && !Rebuild_GetAllData_{queryName})
		return Cached_{queryName};

	{varQueryVersionName} = G_RuntimeVersion;

	Rebuild_GetAllEntities_{queryName} = True;
	Rebuild_GetAllData_{queryName} = False;

	Cached_{queryName}.clear();

	declare entities = EntityManager::GetEntities();
	foreach (entity in entities)
	{{
		declare queryResult = {queryName} {{}};
		{getData}

		queryResult.Entity = entity;

		Cached_{queryName}.add(queryResult);
	}}

	return Cached_{queryName};
}}

";

				queryStruct += $@"{{
	SEntity Entity;

	{headerInside}
}}
";

				structHeader += queryStruct;
				bottom += function;

				lineIdx++;
			}

			return (structHeader, bottom);
		}
	}
}