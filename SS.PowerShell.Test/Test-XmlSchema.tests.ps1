Describe "Test-XmlSchema" {
	Context "Should error if..." {
		#http://www.xmlvalidation.com/example/
		It "XML schema file does not exist" {
			Import-Module SS.PowerShell
			Test-XmlSchema $PSScriptRoot\NotExists.xsd $PSScriptRoot\AddressValid.xml -ErrorVariable err
			$err | Should Not BeNullOrEmpty
			$err[0].Exception.Filename | Should BeLike "*NotExists.xsd"
			$err[0].Exception.Message | Should BeLike "Schema file not found!*"
		}
		It "XML file does not exist" {
			Import-Module SS.PowerShell
			Test-XmlSchema $PSScriptRoot\AddressSchema.xsd $PSScriptRoot\InValid.xml -ErrorVariable err
			$err | Should Not BeNullOrEmpty
			$err[0].Exception.Filename | Should BeLike "*InValid.xml"
			$err[0].Exception.Message | Should BeLike "xml file not found!*"
		}
		It "address zip is invalid XML schema" {
			Import-Module SS.PowerShell
			($result = Test-XmlSchema $PSScriptRoot\AddressSchema.xsd $PSScriptRoot\AddressInValid.xml -ErrorVariable err).Valid | Should Be $false
			$result.Errors | Should Not BeNullOrEmpty
			$result.Errors[0].Message | Should BeLike "The 'zip' element is invalid*"
		}
	}
	Context "Should pass if..." {
		It "address is valid XML schema" {
			Import-Module SS.PowerShell
			(Test-XmlSchema $PSScriptRoot\AddressSchema.xsd $PSScriptRoot\AddressValid.xml -ErrorVariable err).Valid | Should Be $true
			$err | Should BeNullOrEmpty
		}
	}
}