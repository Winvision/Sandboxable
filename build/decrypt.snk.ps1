if ($env:APPVEYOR_PULL_REQUEST_NUMBER -ne $null) {
	Write-Host -ForegroundColor Magenta "This is a pull request build, using development.snk"
} else {
	Write-Host -ForegroundColor Magenta "This is not a pull request build, using release.snk"
	nuget install secure-file -ExcludeVersion -NonInteractive -Verbosity normal
	secure-file\tools\secure-file -decrypt src\release.snk.enc -secret "$env:snkpassword"
}