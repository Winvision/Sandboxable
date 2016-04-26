if ((git log -1 --pretty=format:%H) -eq (git log -1 --pretty=format:%H src/Hyak.Common)) {
	nuget pack src\Hyak.Common\Hyak.Common.csproj -version %APPVEYOR_BUILD_VERSION%
}
if ((git log -1 --pretty=format:%H) -eq (git log -1 --pretty=format:%H src/Microsoft.Azure.Common)) {
	nuget pack src\Microsoft.Azure.Common\Microsoft.Azure.Common.csproj -version %APPVEYOR_BUILD_VERSION%
}
if ((git log -1 --pretty=format:%H) -eq (git log -1 --pretty=format:%H src/Microsoft.Azure.KeyVault)) {
	nuget pack src\Microsoft.Azure.KeyVault\Microsoft.Azure.KeyVault.csproj -version %APPVEYOR_BUILD_VERSION%
}
if ((git log -1 --pretty=format:%H) -eq (git log -1 --pretty=format:%H src/Microsoft.IdentityModel.Clients.ActiveDirectory)) {
	nuget pack src\Microsoft.IdentityModel.Clients.ActiveDirectory\Microsoft.IdentityModel.Clients.ActiveDirectory.csproj -version %APPVEYOR_BUILD_VERSION%
}
if ((git log -1 --pretty=format:%H) -eq (git log -1 --pretty=format:%H src/Microsoft.Azure.Management.KeyVault)) {
	nuget pack src\Microsoft.Azure.Management.KeyVault\Microsoft.Azure.Management.KeyVault.csproj -version %APPVEYOR_BUILD_VERSION%
}