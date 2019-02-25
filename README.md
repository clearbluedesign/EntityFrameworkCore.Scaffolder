# ClearBlueDesign.EntityFrameworkCore.Scaffolder
This is an Entity Framework Core 2.1+ extension for the Microsoft `Scaffold-DbContext` database-first scaffolder that allows you to:

- Use lazy loading for all related entities (`virtual` keyword)
- Use [EFCore.Pluralizer](https://github.com/bricelam/EFCore.Pluralizer) entity name pluralizer by Brice Lambson
- Change the base class of the generated `MyDbContext` class (as may be needed when integrating with the [IdentityServer](http://docs.identityserver.io/en/dev/index.html))
``` cs
public partial class MyDbContext 
	: IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
   ...
}
```
- Import additional assemblies as part of the generated `MyDbContext` class (e.g. `using Microsoft.AspNetCore.Identity.EntityFrameworkCore;`)
- Override base classes and interfaces of any of the generated entities (for integration with libraries like [IdentityServer](http://docs.identityserver.io/en/dev/index.html) and implementation of `IAuditableEntity`-style interfaces)
``` cs
public partial class User : IdentityUser<int>, IAuditableEntity
{
   ...
}
```
``` cs
public partial class Vehicle : IAuditableEntity
{
   ...
}
```

## Usage
1. Add the following package to your startup project:
``` psm1
Install-Package ClearBlueDesign.EntityFrameworkCore.Scaffolder -Pre
```
2. Add `scaffolder.json` file at the root of the project and configure [custom scaffolding options](https://github.com/clearbluedesign/EntityFrameworkCore.Scaffolder/blob/master/README.md#custom-scaffolding-options-scaffolderjson) as needed.
3. Add `DesignTimeServices.cs` file containing `DesignTimeServices` class that implements `IDesignTimeServices` interface (so that the `ClearBlueDesign.EntityFrameworkCore.Scaffolder` is injected into `Scaffold-DbContext` pipeline):
``` cs
public class DesignTimeServices : IDesignTimeServices {
	public void ConfigureDesignTimeServices(IServiceCollection services) {
		services.AddScaffolder();
	}
}
```
4. Run scaffolding command with the desired [base scaffolder parameters](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell#scaffold-dbcontext):
``` psm1
Scaffold-DbContext 
	"<CONNECTION-STRING>" 
	"Microsoft.EntityFrameworkCore.SqlServer" 
	-Context "MyDbContext" 
	-ContextDir "Data" 
	-OutputDir "Data\Entities" 
	-Tables "Product","ProductNote","ProductHistory" 
```

## Custom Scaffolding Options (scaffolder.json)
This scaffolder extension supports all of the official `Scaffold-DbContext` options ([see here](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell#scaffold-dbcontext)).

All of the new scaffolder options should be specified inside the `scaffolder.json` file at the root of the project:
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

## Roadmap
- Add support for entity-level data validations using partial entity classes with validation attributes (`[MetadataObject(typeof(Vehicle.Metadata)], [AssertThat], [RequiredIf]`, etc), `IValidatableObject`, and `ValidateEntity` override on the generated `MyDbContext`.
- Add support for scaffolding of stored procedures into the generated `MyDbContext` using qeury types.


## Contribution
Want to file a bug, contribute some code, or improve documentation? Excellent! Read up on our [guidelines for contributing](https://github.com/clearbluedesign/EntityFrameworkCore.Scaffolder/blob/master/CONTRIBUTING.md) and then [check out one of our issues](https://github.com/clearbluedesign/EntityFrameworkCore.Scaffolder/issues) in the hotlist: community-help. 

## License
This project is licensed under the [MIT license](https://github.com/clearbluedesign/EntityFrameworkCore.Scaffolder/blob/master/LICENSE).
