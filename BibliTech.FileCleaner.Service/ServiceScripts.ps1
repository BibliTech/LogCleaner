# All of the following scripts require Administrator privilege

# Create Service
$Path = Resolve-Path .\
$Path = [System.IO.Path]::Combine($Path, "BibliCleaner.exe")
New-Service -Name BibliCleaner -BinaryPathName $Path -Description "Periodically delete old files" -DisplayName "BibliTech Cleaner" -StartupType Automatic

# Use this for Deleting the Service:
# $service = Get-WmiObject -Class Win32_Service -Filter "Name='BibliCleaner'"
# $service.delete()