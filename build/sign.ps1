Get-ChildItem .\src\*\*.csproj | % {
	$xml = ([xml](Get-Content $_ -Encoding UTF8));
	$filename = $_.FullName;
	$update = $false;

	Write-Host -ForegroundColor Yellow "Checking $filename for development Signing Key File"

	$xml.Project.PropertyGroup | ? { $_.AssemblyOriginatorKeyFile -ne $null } | % {
		$_.AssemblyOriginatorKeyFile = $_.AssemblyOriginatorKeyFile -replace "development", "release";
		$update = $true
	};

	if ($update) {
		$xml.Save($filename); 
		Write-Host -ForegroundColor Green "  $filename updated"
	}
}