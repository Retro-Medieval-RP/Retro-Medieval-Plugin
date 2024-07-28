using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using RetroMedieval.Modules.Configuration;
using RetroMedieval.Modules.Storage;
using RetroMedieval.Utils;
using Rocket.Core.Logging;
using SDG.Unturned;

namespace RetroMedieval.Modules;

public sealed class ModuleLoader : Padlock<ModuleLoader>
{
    public string ModuleDirectory { get; private set; } = "";

    private List<Module> Modules { get; } = [];

    private Timer? Timer { get; set; }

    ~ModuleLoader()
    {
        if (Timer != null)
        {
            Timer.Elapsed -= OnTimerElapsed;
        }

        Timer = null;
    }

    public bool GetModule<TModule>(out TModule module) where TModule : class
    {
        if (Modules.All(x => x.GetType() != typeof(TModule)))
        {
            module = default!;
            return false;
        }

        module = (Modules.Find(x => x.GetType() == typeof(TModule)) as TModule)!;
        return true;
    }

    public void SetDirectory(string dir)
    {
        ModuleDirectory = Path.Combine(dir, "Modules");

        if (!Directory.Exists(ModuleDirectory))
        {
            Directory.CreateDirectory(ModuleDirectory);
        }
    }

    private List<string> GetCommandTable(in Module module)
    {
        var rows = new List<string>();
        rows.AddRange(module.Commands.Select(command =>
            $"| {command.Name} | {command.Help.Replace("|", @"\|").Replace("<", @"\<")} | {command.Syntax.Replace("|", @"\|").Replace("<", @"\<")} | {string.Join(", ", command.Permissions.Count == 0 ? [command.Name] : command.Permissions)} | {string.Join(", ", command.Aliases)} |"));

        return rows;
    }

    public void PrintGeneralDoc()
    {
        var doc =
            $"""
             # Modules:
             {string.Join("\n", Modules
                 .Select(x => x.Information.ModuleName)
                 .Select(x => $"- {x}"))}

             # Commands:
             | Command Name | Command Help | Command Syntax | Command Permissions | Command Aliases |
             |--------------|--------------|----------------|---------------------|-----------------|
             {string.Join("\n", Modules
                 .Select(x => GetCommandTable(x))
                 .Select(x => string.Join("\n", x))
                 .Where(x => !string.IsNullOrEmpty(x)))}
             """;

        var saveLoc = Path.Combine(
            ModuleDirectory,
            "modules.info.md");

        using var stream = new StreamWriter(saveLoc, false);
        stream.Write(doc);
    }

    public void LoadModules(Assembly plugin)
    {
        var modules = plugin.GetTypes()
            .Where(x => x.BaseType == typeof(Module))
            .Select(x => Activator.CreateInstance(x, ModuleDirectory) as Module)
            .Where(x => x != null);

        foreach (var module in modules)
        {
            module?.Load();

            if (module != null)
            {
                Modules.Add(module);
            }
        }

        PrintGeneralDoc();
    }

    public void ReloadAllModules()
    {
        foreach (var module in Modules)
        {
            ReloadModule(module);
        }
    }

    public void ReloadModule(string moduleName)
    {
        if (!Exists(moduleName))
        {
            Logger.LogError($"Module with {moduleName} cannot be found to be reloaded");
            return;
        }

        var module = Modules.First(x => x.Information.ModuleName == moduleName);
        ReloadModule(module);
    }

    private static void ReloadModule(Module module)
    {
        module.Dispose();
        module.Reload();
        module.Load();
    }

    private bool Exists(string moduleName) =>
        Modules.Any(x => x.NameIs(moduleName));

    public void SetUpdateTimer(Timer timer)
    {
        Timer = timer;
        Timer.Elapsed += OnTimerElapsed;
        Timer.Start();
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        foreach (var m in Modules) m.CallTick();
    }

    public void ReloadAllConfigurations()
    {
        foreach (var module in Modules)
        {
            foreach (var config in module.Configurations)
            {
                config.UnloadConfiguration(Path.Combine(ModuleDirectory, module.Information.ModuleName), config.Name + ".json");
                config.LoadedConfiguration(Path.Combine(ModuleDirectory, module.Information.ModuleName), config.Name + ".json");
            }
        }
    }

    public void ReloadConfiguration(string configName)
    {
        var module = Modules.First(x => x.Configurations.Any(c => c.Name == configName));
        var config = module.Configurations.First(c => c.Name == configName);
        config.UnloadConfiguration(Path.Combine(ModuleDirectory, module.Information.ModuleName), config.Name + ".json");
        config.LoadedConfiguration(Path.Combine(ModuleDirectory, module.Information.ModuleName), config.Name + ".json");
    }

    public void ReloadAllStorages()
    {
        foreach (var module in Modules)
        {
            foreach (var storage in module.Storages)
            {
                storage.UnloadStorage(Path.Combine(ModuleDirectory, module.Information.ModuleName), storage.Name + ".json");
                storage.LoadedStorage(Path.Combine(ModuleDirectory, module.Information.ModuleName), storage.Name + ".json");
            }
        }
    }

    public void ReloadStorage(string storageName)
    {
        var module = Modules.First(x => x.Storages.Any(s => s.Name == storageName));
        var storage = module.Storages.First(c => c.Name == storageName);
        storage.UnloadStorage(Path.Combine(ModuleDirectory, module.Information.ModuleName), storage.Name + ".json");
        storage.LoadedStorage(Path.Combine(ModuleDirectory, module.Information.ModuleName), storage.Name + ".json");
    }
}