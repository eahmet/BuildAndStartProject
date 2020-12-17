using System;
using System.IO;
using System.Linq;

namespace BuildAndStartProject
{
    class Program
    {
        public static string containerName=string.Empty;
        public static string name=string.Empty;
        public static string roles=string.Empty;
        static void Main(string[] args)
        {
            var path = args[0];
            var projectPath =new DirectoryInfo(path);
            var outputPath =args[1];
            containerName = args[2];
            name=args[3];
            roles = args[4];
            var key = Console.ReadKey();
            if(key.Key==ConsoleKey.S)
            {
                Build(projectPath,path,outputPath);
                Start(projectPath,path,outputPath);
            }
            else if(key.Key==ConsoleKey.D)
            {
                Build(projectPath,path,outputPath);
                BuildToDockerAndStart(projectPath,path,outputPath);
            }
        }

        private static void BuildToDockerAndStart(DirectoryInfo projectPath,string path,string outputPath)
        {
            var projectName = projectPath.Name.Replace(".","").ToLower();
            Console.Clear();
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "docker";
            startInfo.Arguments = "build "+ outputPath + "\\" + projectName + " -t " + projectName + ":latest";
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();

            process.Close();

            startInfo.Arguments = $"run -p 12006:12006 -p 6008:6008 --name {containerName} -e Name={name} -e HostName={containerName} -e Port=0 -e ActorSystem=bpmlocal -e Roles={roles} -e WebPort=12006 -e StudioRouterName=StudioRouterLocal -e Debug=false -e BPMRouterName=BPMRouterLocal -e AuthenticationRouterName=AuthenticationRouterLocal -e ConfigurationRouterName=ConfigurationRouterLocal -e HumanResourcesRouterName=HumanResourcesRouterLocal -e BSCRouterName=BSCRouterLocal -e SysConf=eyAiRGF0YWJhc2UiOiAiU2VydmVyPXNxbHNydjtJbml0aWFsIENhdGFsb2c9RU5TRU1CTEVfU1VJVEVfREVWO1BlcnNpc3QgU2VjdXJpdHkgSW5mbz1GYWxzZTtVc2VyIElEPXNhO1Bhc3N3b3JkPUJjMzQxNDMxNDtNdWx0aXBsZUFjdGl2ZVJlc3VsdFNldHM9RmFsc2U7Q29ubmVjdGlvbiBUaW1lb3V0PTMwOyIsICJQcm92aWRlciI6ICJTcWxTZXJ2ZXIiIH0= -e CLUSTER_IP=bpmwebapi -e CLUSTER_SEEDS=akka.tcp://bpmlocal@lighthouse:4053 -e CLUSTER_PORT=6008 -d bimserbpmwebapi";
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();

            process.Close();
            Console.WriteLine("Container started");
        }
        private static void Build(DirectoryInfo projectPath,string path,string outputPath)
        {
            Console.Clear();
            var projectName = projectPath.Name.Replace(".","");
            var files = projectPath.GetFiles();
            var projFile = files.FirstOrDefault(x=>x.Extension==".csproj");
            if(projFile== null) return;
            
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "dotnet";
            startInfo.Arguments = "publish "+ path+"\\"+projFile.Name + "  -o " + outputPath + "\\" + projectName;
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();

            process.Close();
        }
        private static void Start(DirectoryInfo projectPath,string path,string outputPath)
        {
            Console.Clear();
            var projectName = projectPath.Name.Replace(".","");
            var files = projectPath.GetFiles();
            var projFile = files.FirstOrDefault(x=>x.Extension==".csproj");
            if(projFile== null) return;
            
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            
            startInfo.EnvironmentVariables["LogStashUri"] = "http://192.168.41.75:5044";
            startInfo.EnvironmentVariables["TracingServiceName"] = "BPM WebAPI";
            startInfo.EnvironmentVariables["TracingHostAddress"] = "192.168.41.75";
            startInfo.EnvironmentVariables["TracingHostPort"] = "16686";
            startInfo.EnvironmentVariables["TracingAgentHostAddress"] = "192.168.41.75";
            startInfo.EnvironmentVariables["TracingAgentHostPort"] = "6831";
            startInfo.EnvironmentVariables["TracingSamplerType"] = "remote";
            startInfo.EnvironmentVariables["TracingSamplerParam"] = "1";
            startInfo.EnvironmentVariables["TracingLogSpan"] = "0";
            startInfo.EnvironmentVariables["Name"] = "BPMWebAPILocal";
            startInfo.EnvironmentVariables["HostName"] = "127.0.0.1";
            startInfo.EnvironmentVariables["Port"] = "0";
            startInfo.EnvironmentVariables["ActorSystem"] = "bpmlocal";
            startInfo.EnvironmentVariables["Roles"] = "BPMWebAPILocal";
            startInfo.EnvironmentVariables["WebPort"] = "12006";
            startInfo.EnvironmentVariables["Debug"] = "false";
            startInfo.EnvironmentVariables["BPMRouterName"] = "BPMRouterLocal";
            startInfo.EnvironmentVariables["AuthenticationRouterName"] = "AuthenticationRouterLocal";
            startInfo.EnvironmentVariables["ConfigurationRouterName"] = "ConfigurationRouterLocal";
            startInfo.EnvironmentVariables["HumanResourcesRouterName"] = "HumanResourcesRouterLocal";
            startInfo.EnvironmentVariables["BSCRouterName"] = "BSCRouterLocal";
            startInfo.EnvironmentVariables["SysConf"] = "eyAiRGF0YWJhc2UiOiAiU2VydmVyPXNxbHNydjtJbml0aWFsIENhdGFsb2c9RU5TRU1CTEVfU1VJVEVfREVWO1BlcnNpc3QgU2VjdXJpdHkgSW5mbz1GYWxzZTtVc2VyIElEPXNhO1Bhc3N3b3JkPUJjMzQxNDMxNDtNdWx0aXBsZUFjdGl2ZVJlc3VsdFNldHM9RmFsc2U7Q29ubmVjdGlvbiBUaW1lb3V0PTMwOyIsICJQcm92aWRlciI6ICJTcWxTZXJ2ZXIiIH0=";
            startInfo.EnvironmentVariables["CLUSTER_IP"] = "127.0.0.1";
            startInfo.EnvironmentVariables["CLUSTER_SEEDS"] = "akka.tcp://bpmlocal@127.0.0.1:4053";
            startInfo.UseShellExecute = false;

            startInfo.FileName = "dotnet";
            startInfo.Arguments = "exec " + outputPath + "\\" + projectName+"\\Bimser.BPM.WebApi.dll";
            process.StartInfo = startInfo;
            process.Start();

            Console.ReadLine();
        }
    }
}
