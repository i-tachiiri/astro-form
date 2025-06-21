## CIの自動テストでよく発生するエラー

プロジェクトまたはソリューションファイル（.csproj または .sln）が見つからないディレクトリで dotnet format を実行した
```
Run dotnet format --verify-no-changes
Unhandled exception: System.IO.FileNotFoundException: Could not find a MSBuild project file or solution file in '/home/runner/work/astro-form/astro-form/'. Specify which to use with the <workspace> argument.
   at Microsoft.CodeAnalysis.Tools.Workspaces.MSBuildWorkspaceFinder.FindWorkspace(String searchDirectory, String workspacePath)
   at Microsoft.CodeAnalysis.Tools.Workspaces.MSBuildWorkspaceFinder.FindWorkspace(String searchDirectory, String workspacePath)
   at Microsoft.CodeAnalysis.Tools.FormatCommandCommon.ParseWorkspaceOptions(ParseResult parseResult, FormatOptions formatOptions)
   at Microsoft.CodeAnalysis.Tools.Commands.RootFormatCommand.FormatCommandDefaultHandler.InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken)
   at System.CommandLine.Invocation.InvocationPipeline.InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken)
Error: Process completed with exit code 1.
```

カバレッジに関する設定の不足
```
Run RATE=$(grep -oPm1 '(?<=line-rate=")\d+\.\d+' TestResults/*/coverage.cobertura.xml)
grep: TestResults/*/coverage.cobertura.xml: No such file or directory
Error: Process completed with exit code 2.
```
