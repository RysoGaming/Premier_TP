using System;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

public class ProjectInfo
{
    public string Name { get; set; } = string.Empty;
    public string UnrealVersion { get; set; } = "Unknown";
    public bool FromSource { get; set; } = false;
    public string[] Plugins { get; set; } = Array.Empty<string>();
}

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No command provided.");
            return;
        }

        switch (args[0].ToLower())
        {
            case "show-infos":
                if (args.Length < 2)
                {
                    Console.WriteLine("No file path provided.");
                }
                else
                {
                    ShowProjectInfo(args[1]);
                }

                break;
            case "build":
                if (args.Length < 2)
                {
                    Console.WriteLine("No project path provided.");
                }
                else
                {
                    BuildProject(args[1]);
                }

                break;
            case "package":
                if (args.Length < 3)
                {
                    Console.WriteLine("Insufficient arguments. Expected: package <projectPath> <outputPath>");
                }
                else
                {
                    PackageProject(args[1], args[2]);
                }

                break;
            default:
                Console.WriteLine("Invalid command");
                break;
        }
    }

    static void ShowProjectInfo(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var projectInfo = JsonConvert.DeserializeObject<ProjectInfo>(json);

            if (projectInfo != null)
            {
                Console.WriteLine($"Project Name: {projectInfo.Name}");
                Console.WriteLine($"Unreal Version: {projectInfo.UnrealVersion}");
                Console.WriteLine($"From Source: {projectInfo.FromSource}");
                Console.WriteLine("Plugins:");
                foreach (var plugin in projectInfo.Plugins)
                {
                    Console.WriteLine($"- {plugin}");
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize the JSON data.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading or processing the file: {ex.Message}");
        }
    }

    static void BuildProject(string projectPath)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"PathToUnrealBuildTool/BuildTool.sh {projectPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            using (Process process = Process.Start(startInfo)!)
            {
                string? output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Utilisez le null-forgiving operator si vous êtes certain que 'output' ne sera jamais null ici
                Console.WriteLine("Build output:");
                Console.WriteLine(output ?? "No output received.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during build process: {ex.Message}");
        }
    }

    static void PackageProject(string projectPath, string outputPath)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"PathToUnrealAutomationTool/UAT.sh -projectPath={projectPath} -outputPath={outputPath} -package\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            using (Process process = Process.Start(startInfo)!)
            {
                string? output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Utilisez le null-forgiving operator si vous êtes certain que 'output' ne sera jamais null ici
                Console.WriteLine("Package output:");
                Console.WriteLine(output ?? "No output received.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during packaging process: {ex.Message}");
        }
    }
}