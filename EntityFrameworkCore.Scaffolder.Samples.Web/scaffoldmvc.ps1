Get-ChildItem ".\Data\Models" -Filter *.cs | 
Foreach-Object {
    $scaffoldCmdController = 
    'dotnet-aspnet-codegenerator ' + 
    '-p . ' +
    'controller ' + 
    '-name ' + $_.BaseName + 'Controller ' +
	'-async ' +
	'-scripts ' +
	'-udl ' +
	#'-api ' + 
    '-m ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.' + $_.BaseName + ' ' +
    '-dc DataContext ' +
    '-outDir Controllers ' +
	'-f ' +
    '-namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Controllers'
	# List commands for testing:
    $scaffoldCmdController

    # Excute commands (uncomment this line):
    iex $scaffoldCmdController

	$viewTypes = @("Create", "Edit", "Delete", "Details", "List")
	foreach ($viewType in $viewTypes) {
		 $scaffoldCmdView = 
		'dotnet-aspnet-codegenerator ' + 
		'-p . ' +
		'view ' + 
		'-viewName ' + $viewType +
		'-templateName ' + $viewType +
		'-m ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.' + $_.BaseName + ' ' +
		'-dc DataContext ' +
		"-scripts " +
		"-udl " +
		"-f " +
		'-outDir Views\' + $_.BaseName +
		'-namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Controllers'

		  # List commands for testing:		
		#$scaffoldCmdView

		# Excute commands (uncomment this line):
		#iex $scaffoldCmdView

	}
	
   
}
