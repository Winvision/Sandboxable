# Get git range
$merge = $false
$parents = (git log -1 --pretty=format:%p)
if ($parents -like " ") {
	# Is merge commit
	$merge = $true
	$parents = $parents.replace(" ", "..")
}

# Version number
$version = $env:APPVEYOR_BUILD_VERSION
if ($env:CI -eq "true") {
	$version = "$version-preview"
}

function Pack-Nuget([string]$Name) {
	if ((git log -1 --pretty=format:%H) -eq (git log -1 --pretty=format:%H src/$name) -or ($merge -and (git log $parents --pretty=format:%H src/$name).length -gt 0)) {
		nuget pack src\$name\$name.csproj -version $version
	}
}

Pack-Nuget -Name Hyak.Common
Pack-Nuget -Name Microsoft.Azure.Common
Pack-Nuget -Name Microsoft.Azure.KeyVault
Pack-Nuget -Name Microsoft.IdentityModel.Clients.ActiveDirectory
Pack-Nuget -Name Microsoft.Azure.Management.KeyVault
Pack-Nuget -Name Microsoft.WindowsAzure.Storage