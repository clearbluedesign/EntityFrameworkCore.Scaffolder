# ClearBlueDesign.EntityFrameworkCore.Scaffolder
**Note: The project is at an early alpha stage phase.**

This project allows you to control how EntityFrameworkCore will scaffold your DbContext and models using database-first approach.

## Usage
1. Add this package to your startup project.
``` psm1
Install-Package ClearBlueDesign.EntityFrameworkCore.Scaffolder -Pre
```
2. Create `scaffolder.json` file at the project root, if none was created during installation process, and configure [options](https://github.com/clearbluedesign/EntityFrameworkCore.Scaffolder/blob/master/README.md#options) as needed.
3. Add implementation of `IDesignTimeServices` to the startup project
``` cs
public class DesignTimeServices : IDesignTimeServices {
	public void ConfigureDesignTimeServices(IServiceCollection services) {
		services.AddScaffolder();
	}
}
```
4. Run scaffold command 
``` psm1
Scaffold-DbContext "<CONNECTION-STRING>" "Microsoft.EntityFrameworkCore.SqlServer" -Context "MyDbContext" -ContextDir "Data" -OutputDir "Data\Entities" -Tables "Product","ProductNote","ProductHistory" 
```
Check [Scaffold-DbContext Docs](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell#scaffold-dbcontext) for the full list of available parameters.

## Options
Below we listed all the available options and their default values.
```
{
	"Scaffolding": {
		"UsePluralizer": true,
		"UseDataAnnotations": false
	},

	"DbContext": {
		"Base": "DbContext"
	},

	"EntityType": {
		"UseLazyLoading": true,
		"BaseMappings": {
			"<TableName1>": [ "MyBaseClass1", "IContract" ],
			"<TableName2>": [ "MyGenericClass1<Int32>", "IContract" ]
			"<TableName3>": [ "IContract" ]
		},
		"LoadAssemblies": [
			"My.Awesome.Project"
		]
	}
}

```

## Contribution
Want to file a bug, contribute some code, or improve documentation? Excellent! Read up on our [guidelines for contributing](https://github.com/clearbluedesign/EntityFrameworkCore.Scaffolder/blob/master/CONTRIBUTING.md) and then [check out one of our issues](https://github.com/clearbluedesign/EntityFrameworkCore.Scaffolder/issues) in the hotlist: community-help. 

## License
This project is licensed under the [MIT license](https://github.com/clearbluedesign/EntityFrameworkCore.Scaffolder/blob/master/LICENSE).
