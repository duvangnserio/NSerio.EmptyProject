param
(
	[Parameter()] [version] $version
)

function BuildSolution(){
	$msbuild=&"${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe
	#build and publish web solutions
	&$msbuild ".\Source\NSerio.EmptyProject.sln" /p:Configuration=Release
}

function BuildRap($version, $currentDirectory){	
	$newName = "NSerio.EmptyProject v$version.rap"

	$buildfile = Join-Path -Path $currentDirectory -ChildPath "build.xml"
	& ".\buildtools\kCura.RAPBuilder.exe" /source:$currentDirectory /input:$buildfile /version:$version /servertype:local /debug:false /sign:false
	
	cd .\Delivery
	Rename-Item -Path "NSerio.EmptyProject.rap" -NewName $newName
	cd $currentDirectory
	Write-Host "The new version has been generated correctly! $version"
}

#main
if ($version){
	
	$currentDirectory = Get-Location	
	BuildSolution
	BuildRap $version $currentDirectory
	explorer .\Delivery
}
else{
	write-host "use build.ps1 script with the respective params. .\build.ps1 -version 'v.v.v.v'"
	write-host "Received params: ($version)"
}