using System.Text.Json;
using System.Text.Json.Nodes;
using WindowsGSH.Core.Modules;

namespace WindowsGSH.Modules.ArmaReforger;

public sealed class ArmaReforgerModule : ManifestBackedGameServerModule, IModuleExistingServerImportCapability
{
    private const string ConfigRelativePath = @"Configs\server.json";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerOptions.Default)
    {
        WriteIndented = true
    };

    public bool CanImport(string path) => ExistingInstallImport.CanImport(this, path);

    public Task<ModuleExistingServerImportProbe> PreviewImportAsync(string path, CancellationToken cancellationToken) =>
        ExistingInstallImport.PreviewAsync(this, path, cancellationToken);

    public override Task<IReadOnlyDictionary<string, object?>> ReadConfigFileSettingsAsync(
        ServerInstance instance,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = GetConfigPath(instance);
        if (!File.Exists(path))
        {
            return Task.FromResult<IReadOnlyDictionary<string, object?>>(new Dictionary<string, object?>());
        }

        var root = ParseConfig(path);
        var settings = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        Copy(root, settings, "bindAddress", "network.bindAddress");
        Copy(root, settings, "publicPort", "network.port");
        Copy(root, settings, "a2s.port", "network.queryPort");
        Copy(root, settings, "game.name", "server.name");
        Copy(root, settings, "game.password", "server.password");
        Copy(root, settings, "game.passwordAdmin", "server.adminPassword");
        Copy(root, settings, "game.scenarioId", "server.scenarioId");
        Copy(root, settings, "game.maxPlayers", "server.maxPlayers");
        Copy(root, settings, "game.visible", "server.visible");
        Copy(root, settings, "game.gameProperties.battlEye", "game.battleEye");
        Copy(root, settings, "game.gameProperties.disableThirdPerson", "game.disableThirdPerson");
        Copy(root, settings, "game.gameProperties.fastValidation", "game.fastValidation");
        if (GetNode(root, "rcon") is JsonObject rcon)
        {
            settings["rcon.enabled"] = true;
            Copy(rcon, settings, "port", "rcon.port");
            Copy(rcon, settings, "password", "rcon.password");
        }

        return Task.FromResult<IReadOnlyDictionary<string, object?>>(settings);
    }

    public override Task WriteConfigFileSettingsAsync(ServerInstance instance, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = GetConfigPath(instance);
        var root = File.Exists(path) ? ParseConfig(path) : new JsonObject();

        Set(root, "bindAddress", GetSetting(instance, "network.bindAddress", ""));
        Set(root, "bindPort", GetInt(instance, "network.port", 2001));
        Set(root, "publicPort", GetInt(instance, "network.port", 2001));
        Set(root, "a2s.address", GetSetting(instance, "network.bindAddress", ""));
        Set(root, "a2s.port", GetInt(instance, "network.queryPort", 17777));
        Set(root, "game.name", GetSetting(instance, "server.name", "Arma Reforger Dedicated Server"));
        Set(root, "game.password", GetSetting(instance, "server.password", ""));
        Set(root, "game.passwordAdmin", GetSetting(instance, "server.adminPassword", ""));
        Set(root, "game.scenarioId", GetSetting(instance, "server.scenarioId", "{ECC61978EDCC2B5A}Missions/23_Campaign.conf"));
        Set(root, "game.maxPlayers", GetInt(instance, "server.maxPlayers", 32));
        Set(root, "game.visible", GetBool(instance, "server.visible", true));
        Set(root, "game.gameProperties.battlEye", GetBool(instance, "game.battleEye", true));
        Set(root, "game.gameProperties.disableThirdPerson", GetBool(instance, "game.disableThirdPerson", false));
        Set(root, "game.gameProperties.fastValidation", GetBool(instance, "game.fastValidation", true));

        if (GetBool(instance, "rcon.enabled", false))
        {
            var password = GetSetting(instance, "rcon.password", "");
            if (password.Length < 3 || password.Any(char.IsWhiteSpace))
            {
                throw new InvalidDataException("Arma Reforger RCON requires a password of at least 3 characters with no spaces.");
            }

            Set(root, "rcon.address", GetSetting(instance, "network.bindAddress", ""));
            Set(root, "rcon.port", GetInt(instance, "rcon.port", 19999));
            Set(root, "rcon.password", password);
            Set(root, "rcon.permission", "admin");
        }
        else
        {
            root.Remove("rcon");
        }

        var directory = Path.GetDirectoryName(path)!;
        Directory.CreateDirectory(directory);
        var temporaryPath = path + ".windowsgsh.tmp";
        try
        {
            File.WriteAllText(temporaryPath, root.ToJsonString(JsonOptions));
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }

        return Task.CompletedTask;
    }

    private static string GetConfigPath(ServerInstance instance) => Path.Combine(instance.InstallPath, ConfigRelativePath);

    private static JsonObject ParseConfig(string path)
    {
        return JsonNode.Parse(File.ReadAllText(path)) as JsonObject
            ?? throw new InvalidDataException($"Arma Reforger configuration is not a JSON object: {path}");
    }

    private static void Copy(JsonObject root, IDictionary<string, object?> settings, string jsonPath, string settingKey)
    {
        if (GetNode(root, jsonPath) is JsonValue value)
        {
            settings[settingKey] = value.GetValue<object?>();
        }
    }

    private static JsonNode? GetNode(JsonObject root, string path)
    {
        JsonNode? current = root;
        foreach (var segment in path.Split('.'))
        {
            current = current?[segment];
        }

        return current;
    }

    private static void Set(JsonObject root, string path, object? value)
    {
        var segments = path.Split('.');
        var current = root;
        for (var index = 0; index < segments.Length - 1; index++)
        {
            if (current[segments[index]] is not JsonObject child)
            {
                child = new JsonObject();
                current[segments[index]] = child;
            }

            current = child;
        }

        current[segments[^1]] = JsonValue.Create(value);
    }

    private static int GetInt(ServerInstance instance, string key, int fallback) =>
        int.TryParse(GetSetting(instance, key, fallback.ToString()), out var value) ? value : fallback;

    private static bool GetBool(ServerInstance instance, string key, bool fallback) =>
        bool.TryParse(GetSetting(instance, key, fallback.ToString()), out var value) ? value : fallback;
}
