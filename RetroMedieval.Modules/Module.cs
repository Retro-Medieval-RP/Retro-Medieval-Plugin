using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Modules.Configuration;
using RetroMedieval.Modules.Storage;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Logging;

namespace RetroMedieval.Modules;

public abstract class Module
{
    internal ModuleInformation Information => GetType().GetCustomAttribute<ModuleInformation>();
    internal List<ModuleConfiguration> Configurations { get; }
    internal List<ModuleStorage> Storages { get; }
    internal List<IRocketCommand> Commands { get; }

    protected string ModuleDir { get; set; }

    protected Module(string directory)
    {
        ModuleDir = Path.Combine(directory, Information.ModuleName);
        Configurations = [];
        Storages = [];
        Commands = [];

        if (!Directory.Exists(ModuleDir))
        {
            Directory.CreateDirectory(ModuleDir);
        }
        
        LoadConfigs();
        LoadStorages();
        LoadCommands();
    }

    public abstract void Load();
    public abstract void Unload();

    internal void CallTick() =>
        OnTimerTick();

    protected virtual void OnTimerTick()
    {
    }

    private void LoadConfigs()
    {
        var configs = GetType().GetCustomAttributes<ModuleConfiguration>();

        foreach (var config in configs)
        {
            if (config.LoadedConfiguration(ModuleDir, config.Name + ".json"))
            {
                Configurations.Add(config);
                Logger.Log("Successfully Loaded Config: " + config.Name);
                continue;
            }

            Logger.LogError("Failed To Load Config: " + config.Name);
        }
    }

    private void LoadStorages()
    {
        var storages = GetType().GetCustomAttributes<ModuleStorage>();

        foreach (var storage in storages)
        {
            if (storage.LoadedStorage(ModuleDir, storage.Name + ".json"))
            {
                Storages.Add(storage);
                Logger.Log("Successfully Loaded Storage: " + storage.Name);
                continue;
            }

            Logger.LogError("Failed To Load Storage: " + storage.Name);
        }
    }
    
    private static List<string> GetCommandTable(List<IRocketCommand> commands)
    {
        var rows = new List<string>();
        rows.AddRange(commands.Select(command =>
            $"| {command.Name} | {command.Help.Replace("|", @"\|").Replace("<", @"\<")} | {command.Syntax.Replace("|", @"\|").Replace("<", @"\<")} | {string.Join(", ", command.Permissions.Count == 0 ? [command.Name] : command.Permissions)} | {string.Join(", ", command.Aliases)} |"));

        return rows;
    }
    
    private void LoadCommands()
    {
        var commands = GetType()
            .Assembly
            .GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(IRocketCommand)))
            .Select(x => Activator.CreateInstance(x) as IRocketCommand)
            .Where(x => x != null)
            .ToList();

        foreach (var command in commands)
        {
            R.Commands.Register(command);
            Commands.Add(command!);
        }
        
        var doc =
            $"""
             # {Information.ModuleName}

             ## Commands:
             | Command Name | Command Help | Command Syntax | Command Permissions | Command Aliases |
             |--------------|--------------|----------------|---------------------|-----------------|
             {string.Join("\n", GetCommandTable(Commands))}
             """;

        var saveLoc = Path.Combine(
            ModuleDir,
            $"{Information.ModuleName}.info.md");

        using var stream = new StreamWriter(saveLoc, false);
        stream.Write(doc);
    }

    public bool GetConfiguration<TConfiguration>(out TConfiguration config)
        where TConfiguration : class, IConfig, new()
    {
        if (Configurations.Any(x => x.IsConfigOfType(typeof(TConfiguration))))
        {
            config =
                (Configurations.Find(x => x.IsConfigOfType(typeof(TConfiguration))) as
                    ModuleConfiguration<TConfiguration>)?.Configuration!;
            return true;
        }

        config = default!;
        return false;
    }

    public bool GetStorage<TStorage>(out TStorage storage) where TStorage : class, IStorage, new()
    {
        if (Storages.Any(x => x.IsStorageOfType(typeof(TStorage))))
        {
            storage = ((ModuleStorage<TStorage>)Storages.First(x => x.IsStorageOfType(typeof(TStorage)))).Storage;
            return true;
        }

        storage = default!;
        return false;
    }

    internal bool NameIs(string moduleName) =>
        Information.ModuleName == moduleName;
}