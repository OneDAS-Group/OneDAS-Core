function Add-TargetFramework($name, $packagePath)
{
  [string]$nugetPackageRoot = $env:NUGET_PACKAGES
  if ($nugetPackageRoot -eq "")
  {
    $nugetPackageRoot = Join-Path $env:USERPROFILE ".nuget\packages"
  }

  $realPackagePath = Join-Path $nugetPackageRoot $packagePath 
  $resourceTypeName = $name + "Resources"
  $script:codeContent += @"
    internal static class $resourceTypeName
    {

"@;

  $refContent = @"
    public static class $name
    {

"@

  $name = $name.ToLower()
  $list = Get-ChildItem -filter *.dll $realPackagePath | ForEach-Object { $_.FullName }
  $allPropNames = @()
  foreach ($dllPath in $list)
  {
    $dllName        = Split-Path -Leaf $dllPath

    if ($dllName.StartsWith("Microsoft.VisualBasic") -or `
        $dllName.StartsWith("System.Diagnostics") -or `
        $dllName.StartsWith("System.Drawing") -or `
        $dllName.StartsWith("System.IO") -or `
        $dllName.StartsWith("System.Net") -or `
        $dllName.StartsWith("System.Reflection") -or `
        $dllName.StartsWith("System.Web") -or `
        $dllName.StartsWith("WindowsBase"))
    {
      continue
    }

    $xmlExists      = [System.IO.File]::Exists($dllPath.Replace(".dll", ".xml"))

    $dll            = $dllName.Substring(0, $dllName.Length - 4)
    $logicalName    = "$($name).$($dll)";
    $dllPath        = $dllPath.Substring($nugetPackageRoot.Length)
    $dllPath        = '$(NuGetPackageRoot)' + $dllPath
    $xmlName        = $dllName.Replace(".dll", ".xml")
    $xmlPath        = $dllPath.Replace(".dll", ".xml")

    $script:targetsContent += @"
        <EmbeddedResource Include="$dllPath">
          <LogicalName>$($logicalName).dll</LogicalName>
          <Link>Resources\$name\$dllName</Link>
        </EmbeddedResource>
"@

    if ($xmlExists)
    {
        $script:targetsContent += @"
        <EmbeddedResource Include="$xmlPath">
          <LogicalName>$($logicalName).xml</LogicalName>
          <Link>Resources\$name\$xmlName</Link>
        </EmbeddedResource>
"@
    }


    $propName = $dll.Replace(".", "");
    $allPropNames += $propName
    $fieldName = "_" + $propName
    $script:codeContent += @"
        private static byte[]? $($fieldName)_dll; internal static byte[] $($propName)_dll => ResourceLoader.GetOrCreateResource(ref $($fieldName)_dll, "$($logicalName).dll");

"@

    if ($xmlExists)
    {
        $script:codeContent += @"
        private static byte[]? $($fieldName)_xml; internal static byte[] $($propName)_xml => ResourceLoader.GetOrCreateResource(ref $($fieldName)_xml, "$($logicalName).xml");

"@
    }

    if ($xmlExists)
    {
        $refContent += @"
        public static PortableExecutableReference $propName { get; } = AssemblyMetadata.CreateFromImage($($resourceTypeName).$($propName)_dll).GetReference(display: "$dll ($name)", documentation: XmlDocumentationProvider.CreateFromBytes($($resourceTypeName).$($propName)_xml));

"@
    }
    else
    {
        $refContent += @"
        public static PortableExecutableReference $propName { get; } = AssemblyMetadata.CreateFromImage($($resourceTypeName).$($propName)_dll).GetReference(display: "$dll ($name)");

"@
    }
  }

  $refContent += @"
        public static IEnumerable<PortableExecutableReference> All { get; } = new PortableExecutableReference[]
        {

"@;
    foreach ($propName in $allPropNames)
    {
      $refContent += @"
            $propName,

"@
    }

    $refContent += @"
        };
    }

"@

    $script:codeContent += @"
    }

"@
    $script:codeContent += $refContent;
}

$targetsContent = @"
<Project>
    <ItemGroup>

"@;

$codeContent = @"
// This is a generated file, please edit Generate.ps1 to change the contents

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using OneDas.DataManagement.Explorer.Core;

namespace OneDas.DataManagement.Explorer.Roslyn
{

"@

Add-TargetFramework "Net50" 'Microsoft.NETCore.App.Ref\5.0.0\ref\net5.0' 

$targetsContent += @"
  </ItemGroup>
</Project>
"@;

$codeContent += @"
}
"@

$targetsContent | Out-File "Generated.targets" -Encoding Utf8
$codeContent | Out-File "Generated.cs" -Encoding Utf8
