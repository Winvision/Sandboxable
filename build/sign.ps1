if ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null) {
	Get-ChildItem .\src\*\*.csproj | % {
		$xml = ([xml](Get-Content $_ -Encoding UTF8));
		$filename = $_.FullName;
		$update = $false;

		Write-Host "Checking $filename for development Signing Key File"

		$xml.Project.PropertyGroup | ? { $_.AssemblyOriginatorKeyFile -ne $null } | % {
			$_.AssemblyOriginatorKeyFile = $_.AssemblyOriginatorKeyFile -replace "development", "release";
			$update = $true
		};

		if ($update) {
			$xml.Save($filename); 
			Write-Host -ForegroundColor Magenta "$filename updated"
		}
	}
}