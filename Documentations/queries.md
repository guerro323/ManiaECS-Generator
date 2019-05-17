# Queries

Queries are helpful when you want to get entities with specific components.  
To make queries, create a file called `query.txt` inside your input folder.  

If you want to have a list of `PlayerData` and `Position`, insert this into your query file:

```csharp
// Comments are supported! Use them to seperate your queries

// MyCustomSystem queries:
PlayerData, Position
```

And to use them in your systems:
```php
// To get a list of entities
declare SEntity[] queriedEntities = Components::GetAllEntities(PlayerData {}, Position {});

// To get a list of the components data
declare QueryResult_PlayerData_Position queriedComponents = Components::GetAllData(PlayerData {}, Position {});
```

Once a query is called, it will not be rebuilt (= the results are cached) until some structural changes happen (new components, new and removed entities, etc...)