Describe "Test-JsonSchema" {
	Context "Should error if..." {
		#http://json-schema.org/examples.html
		It "JSON schema file does not exist" {
			Import-Module SS.PowerShell
			Test-JsonSchema $PSScriptRoot\PersonValid.json $PSScriptRoot\NotExists.json -ErrorVariable err
			$err | Should Not BeNullOrEmpty
			$err[0].Exception.Filename | Should BeLike "*NotExists.json"
			$err[0].Exception.Message | Should BeLike "Schema file not found!*"
		}
		It "JSON file does not exist" {
			Import-Module SS.PowerShell
			Test-JsonSchema $PSScriptRoot\InValid.json $PSScriptRoot\PersonSchema.json -ErrorVariable err
			$err | Should Not BeNullOrEmpty
			$err[0].Exception.Filename | Should BeLike "*InValid.json"
			$err[0].Exception.Message | Should BeLike "Json file not found!*"
		}
		It "person age is invalid JSON schema" {
			Import-Module SS.PowerShell
			($result = Test-JsonSchema $PSScriptRoot\PersonInValidAge.json $PSScriptRoot\PersonSchema.json -ErrorVariable err).Valid | Should Be $false
			$result.Errors | Should Not BeNullOrEmpty
			$result.Errors[0].Message | Should BeLike "* is less than minimum value of 0."
		}
	}
	Context "Should pass if..." {
		It "person is valid JSON schema" {
			Import-Module SS.PowerShell
			(Test-JsonSchema $PSScriptRoot\PersonValid.json $PSScriptRoot\PersonSchema.json -ErrorVariable err).Valid | Should Be $true
			$err | Should BeNullOrEmpty
		}
		It "Schema is ref in JSON file" {
			Import-Module SS.PowerShell
			(Test-JsonSchema $PSScriptRoot\ArmTemplateExtRef.json -SchemaInJsonFile -ErrorVariable err).Valid | Should Be $true
			$err | Should BeNullOrEmpty
		}
	}
}