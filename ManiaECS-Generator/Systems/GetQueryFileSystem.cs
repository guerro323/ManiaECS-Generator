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
				var varQueryDataVersionName = varQueryVersionName + "_Data";
				
				var structNames = str.Replace(" ", string.Empty)
				                     .Split(',', StringSplitOptions.RemoveEmptyEntries);
				var headerInside = string.Empty;
				var functionArgument = string.Empty;
				var queryStruct = string.Empty;
				var condition = string.Empty;
				var getDataCondition = string.Empty;
				var getData = string.Empty;
				var dataVersion = string.Empty;
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
					
					structFound++;
				}

				if (structFound == 0)
					continue;
				
				var queryName = queryStruct.Replace("#Struct ", string.Empty);
				
				for (var i = 0; i < structNames.Length; i++)
				{
					var name = structNames[i];
					if (name.StartsWith("//"))
						continue;

					headerInside     += $"\n\t{name} _{name};";
					functionArgument += $"_{name};";
					condition        += $"&& Has_{name}(entity)";
					dataVersion      += $"declare Integer {varQueryDataVersionName}_{name};\n";
					getDataCondition += $"\n\t\tif (!Has_{name}(entity)) continue;";
					getData          += $"\n\t\tif (rebuild || Rebuild_GetAllData_{queryName} || {varQueryDataVersionName}_{name} < G_{name}Version_Data)\n\t\t{{\n\t\t\tqueryResult._{name} = Get_{name}(entity);\n\t\t\t{varQueryDataVersionName}_{name} = G_{name}Version_Data;\n\t\t}} else Cached_{queryName}[I]._{name};\n";
					checkVersion     += $"\tif ({varQueryVersionName} < G_{name}Version || {varQueryDataVersionName}_{name} < G_{name}Version_Data) rebuild = True;\n";
				}

				function += $"\n//\n// Query {lineIdx}:{queryName} \n//\n";
				function += $"declare {queryName}[] Cached_{queryName};\n";
				function += $"declare SEntity[] Cached_Entities_{queryName};\n";
				function += $"declare Integer {varQueryVersionName};\n";
				function += dataVersion;
				function += $"declare Integer DestroyVersion_{queryName};\n";
				function += $"declare Boolean Rebuild_GetAllEntities_{queryName};\n";
				function += $"declare Boolean Rebuild_GetAllData_{queryName};\n";
				
				function += $@"
// ---------------------------------- //
/** Query entities list with '{functionArgument.Replace(';', ' ').Replace("_", string.Empty).TrimEnd(' ')}' components
 *
 * @return 		Return a list of entities
*/
SEntity[] QueryEntities{functionArgument.Replace(";", string.Empty)}()
{{
	declare rebuild = DestroyVersion_{queryName} < EntityManager::GetDestroyVersion();
{checkVersion}
	if (!rebuild && !Rebuild_GetAllEntities_{queryName})
		return Cached_Entities_{queryName};

	{varQueryVersionName} = G_RuntimeVersion;
	DestroyVersion_{queryName} = EntityManager::GetDestroyVersion();

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
				function += $@"
// ---------------------------------- //
/** Query entities data list with '{functionArgument.Replace(';', ' ').Replace("_", string.Empty).TrimEnd(' ')}' components
 *
 * @return 		Return the query struct result with components data and entity
*/
{queryName}[] QueryData{functionArgument.Replace(";", string.Empty)}()
{{
	declare rebuild = DestroyVersion_{queryName} < EntityManager::GetDestroyVersion();
{checkVersion}
	// if no entities were destroyed or the other query function didn't asked for a rebuild before,
	// we can return the cached query.
	if (!rebuild && !Rebuild_GetAllData_{queryName})
		return Cached_{queryName};

	{varQueryVersionName} = G_RuntimeVersion;
	DestroyVersion_{queryName} = EntityManager::GetDestroyVersion();

	Rebuild_GetAllEntities_{queryName} = True;
	Rebuild_GetAllData_{queryName} = False;

	Cached_{queryName}.clear();

	declare entities = EntityManager::GetEntities();
	declare I = 0;
	foreach (entity in entities)
	{{
		declare queryResult = {queryName} {{}};
{getDataCondition}
{getData}

		I += 1;

		queryResult.Entity = entity;

		Cached_{queryName}.add(queryResult);
	}}

	return Cached_{queryName};
}}

";

				queryStruct += $@"
{{
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