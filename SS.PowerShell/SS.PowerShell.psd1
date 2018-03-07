#
# Module manifest for module 'SS.PowerShell'
#
# Generated by: John W Carew
#
# Generated on: 03/06/2018
#
@{
	RootModule = 'SS.PowerShell.dll'
	ModuleVersion = '1.0.2.0'
	GUID = 'c546edb7-361b-43d6-b663-01895a9ffe00'
	Author = 'John W Carew'
	CompanyName = 'Simpler Software'
	Copyright = '(c) 2018 Simpler Software. All rights reserved.'
	Description = 'Useful PowerShell cmdlets.
	Currently supports...
	* Validate JSON files against a JSON schema file using Newtonsoft.Json.Schema library.
	* Validate XML files against a XSD schema file.'
	PowerShellVersion = '5.0'
	DotNetFrameworkVersion = '4.0'
	CLRVersion = '4.0'
	FileList = @(
		'Newtonsoft.Json.dll',
		'Newtonsoft.Json.Schema.dll',
		'SS.PowerShell.dll',
		'SS.PowerShell.dll-Help.xml',
		'SS.PowerShell.psd1',
		'LICENSE.txt'
	)
	CmdletsToExport = @('Test-JsonSchema','Test-XmlSchema')
	FunctionsToExport = @()
	AliasesToExport = @()

	PrivateData = @{
		PSData = @{
			Tags = @('Newtonsoft', 'XML', 'JSON', 'Schema', 'Validate')
			#Prerelease = 'preview'
			ReleaseNotes = '* Added XML document schema support.'
			LicenseUri = 'https://github.com/Simpler-Software/SS.PowerShell/blob/master/LICENSE.md'
			ProjectUri = 'https://github.com/Simpler-Software/SS.PowerShell'
			# IconUri = ''
		} 
	}
}

