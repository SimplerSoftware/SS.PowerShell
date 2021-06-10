# SS.PowerShell
Useful PowerShell cmdlets.

All [releases](https://www.powershellgallery.com/packages/SS.PowerShell/) can be pulled from PowerShell Galery using [PowerShellGet](https://www.powershellgallery.com/).
```PowerShell
> Install-Module -Name SS.PowerShell 
```

## Currently supports... 
* Validate JSON files against a JSON schema file using Newtonsoft.Json.Schema library. 
* Validate XML files against a XSD schema file.
* Create/Save Outlook Email MSG files

#### Outlook message example
```PowerShell
> $msg = New-OutlookEmail -Subject "Trap" -BodyText "We have a CI for Pan!" -Draft
> $msg | Add-OutlookRecipient -EmailAddress hook@neverland.isle -DisplayName "Captain"
> $msg | Add-OutlookRecipient -EmailAddress smee@neverland.isle -DisplayName "Smee" -Type CC
> $msg | Add-OutlookRecipient -EmailAddress lostboys@neverland.isle -DisplayName "Lost Boys" -Type BCC
> $msg | Save-OutlookEmail -File "CI Trap.msg"
```