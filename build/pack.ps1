# Get Git range
$merge = $false
$parents = (git log -1 --pretty=format:%p)
if ($parents -match " ") {
	Write-Host "This is a build with a merge"
	$merge = $true
	$parents = $parents.replace(" ", "..")
}

# Version number
$version = $env:APPVEYOR_BUILD_VERSION
if ($env:APPVEYOR_PULL_REQUEST_NUMBER -ne $null) {
	Write-Host "This is a pull request build"
	$version = "$version-preview"
}
Write-Host "Build version number is $version"

function Pack-Nuget([string]$Name) {
	Write-Host "Checking if commit contains files for package $name" -NoNewline
	if ((git log -1 --pretty=format:%H) -eq (git log -1 --pretty=format:%H src/$name) -or ($merge -and (git log $parents --pretty=format:%H src/$name).length -gt 0)) {
		Write-Host -ForegroundColor Green " -- Yep"
		Write-Host
		nuget pack src\$name\$name.nuspec -version $version
		Write-Host
	} else {
		Write-Host -ForegroundColor Red " -- Nope"
	}
}

Pack-Nuget -Name Hyak.Common
Pack-Nuget -Name Microsoft.Azure.Common
Pack-Nuget -Name Microsoft.Azure.KeyVault
Pack-Nuget -Name Microsoft.IdentityModel.Clients.ActiveDirectory
Pack-Nuget -Name Microsoft.Azure.Management.KeyVault
Pack-Nuget -Name Microsoft.WindowsAzure.Storage