

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Extensions
{
    public static partial class Util
    {
        /// <summary>
        /// Detects if the application is running in a development environment.
        /// </summary>
        /// <param name="checks">
        /// Tipos de checagem a serem realizados. Por padrão, todos os tipos.
        /// </param>
        public static bool IsDev(DevelopmentCheckType checks = DevelopmentCheckType.All)
        {

            if (checks == DevelopmentCheckType.None)
                return false;

            if (checks.HasFlag(DevelopmentCheckType.DebugBuild))
            {
                if (Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false)
                    .FirstOrDefault() is AssemblyConfigurationAttribute configAttr)
                {

                    if (configAttr.Configuration.Equals("Debug", StringComparison.OrdinalIgnoreCase))
                        return true;

                }
            }

            // 2️⃣ ASP.NET Core environment variable
            if (checks.HasFlag(DevelopmentCheckType.AspNetCoreEnvironment))
            {
                var aspnetEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (!string.IsNullOrEmpty(aspnetEnv) &&
                    aspnetEnv.FlatEqual("Development"))
                    return true;
            }

            // 3️⃣ .NET environment variable
            if (checks.HasFlag(DevelopmentCheckType.DotNetEnvironment))
            {
                var dotnetEnv = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                if (!string.IsNullOrEmpty(dotnetEnv) &&
                    dotnetEnv.FlatEqual("Development"))
                    return true;
            }

            // 4️⃣ Debugger attached (runtime)
            if (checks.HasFlag(DevelopmentCheckType.DebuggerAttached))
            {
                if (Debugger.IsAttached)
                    return true;
            }

            // 5️⃣ Launch profile detection (Visual Studio / launchSettings.json)
            if (checks.HasFlag(DevelopmentCheckType.LaunchProfile))
            {
                var launchProfile = Environment.GetEnvironmentVariable("DOTNET_LAUNCH_PROFILE");
                if (!string.IsNullOrEmpty(launchProfile))
                    return true;
            }

            // 6️⃣ Marker file detection (useful in containers or local dev)
            if (checks.HasFlag(DevelopmentCheckType.MarkerFile))
            {
                if (File.Exists(Path.Combine(AppContext.BaseDirectory, "dev.marker")))
                    return true;
            }

            // 7️⃣ Machine name pattern (if dev machines follow a naming convention)
            if (checks.HasFlag(DevelopmentCheckType.MachineName))
            {
                if (Environment.MachineName.StartsWith("DEV-", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // 8️⃣ Executable path check (running from bin\Debug)
            if (checks.HasFlag(DevelopmentCheckType.ExecutablePath))
            {
                var exePath = AppContext.BaseDirectory;
                if (exePath.Contains(Path.Combine("bin", "Debug"), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // 9️⃣ Host environment check
            if (checks.HasFlag(DevelopmentCheckType.HostEnvironment))
            {
                try
                {
                    // This works if you have DI and IHostEnvironment registered
                    var envName = AppDomain.CurrentDomain.GetData("DOTNET_HOST_ENVIRONMENT") as string;
                    if (!string.IsNullOrEmpty(envName) && envName.Equals("Development", StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                catch
                {
                    // Ignore if not available
                }
            }

            return false;
        }

    }


    /// <summary>
    /// Enumeração com flags para tipos de checagem de modo desenvolvimento.
    /// </summary>
    [Flags]
    public enum DevelopmentCheckType
    {
        /// <summary>
        /// Nenhuma checagem.
        /// </summary>
        None = 0,
        /// <summary>
        /// Verifica se compilado em DEBUG.
        /// </summary>
        DebugBuild = 1,
        /// <summary>
        /// Verifica variável ASPNETCORE_ENVIRONMENT.
        /// </summary>
        AspNetCoreEnvironment = 2,
        /// <summary>
        /// Verifica variável DOTNET_ENVIRONMENT.
        /// </summary>
        DotNetEnvironment = 4,
        /// <summary>
        /// Verifica se debugger está anexado.
        /// </summary>
        DebuggerAttached = 8,
        /// <summary>
        /// Verifica perfil de lançamento DOTNET_LAUNCH_PROFILE.
        /// </summary>
        LaunchProfile = 16,
        /// <summary>
        /// Verifica arquivo marcador dev.marker.
        /// </summary>
        MarkerFile = 32,
        /// <summary>
        /// Verifica padrão de nome da máquina.
        /// </summary>
        MachineName = 64,
        /// <summary>
        /// Verifica caminho executável bin\Debug.
        /// </summary>
        ExecutablePath = 128,
        /// <summary>
        /// Verifica DOTNET_HOST_ENVIRONMENT.
        /// </summary>
        HostEnvironment = 256,
        /// <summary>
        /// Todas as checagens (padrão).
        /// </summary>
        All = DebugBuild | AspNetCoreEnvironment | DotNetEnvironment | DebuggerAttached | LaunchProfile | MarkerFile | MachineName | ExecutablePath | HostEnvironment,

        /// <summary>
        /// Checagens relacionadas ao ambiente de execução. 
        /// </summary>
        Environment = AspNetCoreEnvironment | DotNetEnvironment | HostEnvironment

    }


}