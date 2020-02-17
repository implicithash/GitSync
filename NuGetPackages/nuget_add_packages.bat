set ProductVersion=1.0.0
set Config=Release
set Source=\\navbuild\NuGet

.nuget\nuget add EW.ConfigMan.Contracts.Package\bin\%Config%\EW.ConfigMan.Contracts.Package.%ProductVersion%.symbols.nupkg -Source %Source%
.nuget\nuget add EW.ConfigMan.Deam.Package\bin\%Config%\EW.ConfigMan.Deam.Package.%ProductVersion%.symbols.nupkg -Source %Source%
.nuget\nuget add EW.ConfigMan.Entities.Package\bin\%Config%\EW.ConfigMan.Entities.Package.%ProductVersion%.symbols.nupkg -Source %Source%
.nuget\nuget add EW.ConfigMan.Services.Package\bin\%Config%\EW.ConfigMan.Services.Package.%ProductVersion%.symbols.nupkg -Source %Source%