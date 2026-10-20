using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using LookingGlass.ItemStatsNameSpace;
using RoR2;

namespace RiskyCohesion.LookingGlassStats;

internal sealed class StatsGroup(Func<ConfigEntry<bool>> bindEnabled, List<StatsGroup> band)
{
    private static readonly List<StatsGroup> _lookingGlassCorrections = [];
    private static readonly List<StatsGroup> _groups = [];
    private static readonly Dictionary<(Dictionary<int, ItemStatsDef>, int), StatsStack> _stacks = [];

    private readonly List<StatsLayer> _layers = [];

    private ConfigEntry<bool> _enabled;

    static StatsGroup()
    {
        RoR2Application.onLoad += ShowAllOnceLoaded;
    }

    public static StatsGroup Bind(ConfigFile config, string key, Description description) => new(() => config.Bind(Sections.LookingGlass, key, true, description), _groups);

    public static StatsGroup CorrectingLookingGlass(ConfigFile config, string key, Description description)
        => new(() => config.Bind(Sections.LookingGlass, key, true, description), _lookingGlassCorrections);

    public void Item(Func<ItemDef> item, Func<ItemStatsDef> stats) => AmendItem(item, _ => stats());

    public void AmendItem(Func<ItemDef> item, Func<ItemStatsDef, ItemStatsDef> amend) => AddLayer(ItemDefinitions.allItemDefinitions, () => Identify(item()), amend);

    public void Equipment(Func<EquipmentDef> equipment, Func<ItemStatsDef> stats) => AmendEquipment(equipment, _ => stats());

    public void AmendEquipment(Func<EquipmentDef> equipment, Func<ItemStatsDef, ItemStatsDef> amend) => AddLayer(ItemDefinitions.allEquipmentDefinitions, () => Identify(equipment()), amend);

    public void Refresh()
    {
        foreach (var layer in _layers)
            layer.Stack?.Show();
    }

    private void AddLayer(Dictionary<int, ItemStatsDef> definitions, Func<(int Index, string Name)> identify, Func<ItemStatsDef, ItemStatsDef> build)
    {
        if (_layers.Count == 0)
        {
            _enabled = bindEnabled();
            band.Add(this);
        }

        _layers.Add(new StatsLayer(this, definitions, identify, build));
    }

    private static (int Index, string Name) Identify(ItemDef item) => item
        ? ((int)item.itemIndex, item.name)
        : (-1, null);

    private static (int Index, string Name) Identify(EquipmentDef equipment) => equipment
        ? ((int)equipment.equipmentIndex, equipment.name)
        : (-1, null);

    private static void ShowAllOnceLoaded()
    {
        var bottomToTop = _lookingGlassCorrections.Concat(_groups).ToArray();

        foreach (var group in bottomToTop)
            group.PlaceLayers();

        foreach (var stack in _stacks.Values)
            stack.Show();

        foreach (var group in bottomToTop)
            group._enabled.SettingChanged += (_, _) => group.Refresh();
    }

    private void PlaceLayers()
    {
        foreach (var layer in _layers)
            layer.Place();
    }

    private sealed class StatsLayer(StatsGroup group, Dictionary<int, ItemStatsDef> definitions, Func<(int Index, string Name)> identify, Func<ItemStatsDef, ItemStatsDef> build)
    {
        public StatsStack Stack { get; private set; }
        public bool IsOn => group._enabled.Value;

        public ItemStatsDef Build(ItemStatsDef below) => build(below);

        public void Place()
        {
            var (index, name) = (-1, "");
            Failsafe.Run("finding an item for LookingGlass stats", () => (index, name) = identify());
            if (index < 0)
                return;

            if (!_stacks.TryGetValue((definitions, index), out var stack))
            {
                stack = new StatsStack(definitions, index, name);
                _stacks[(definitions, index)] = stack;
            }

            stack.Add(this);
            Stack = stack;
        }
    }

    private sealed class StatsStack(Dictionary<int, ItemStatsDef> definitions, int index, string name)
    {
        private readonly ItemStatsDef _original = definitions.TryGetValue(index, out var original)
            ? original
            : null;

        private readonly List<StatsLayer> _layers = [];

        public void Add(StatsLayer layer) => _layers.Add(layer);

        public void Show()
        {
            var shown = _original;
            foreach (var layer in _layers)
            {
                if (layer.IsOn)
                    shown = BuildOrKeep(layer, shown);
            }

            if (shown != null)
                definitions[index] = shown;
            else
                definitions.Remove(index);
        }

        private ItemStatsDef BuildOrKeep(StatsLayer layer, ItemStatsDef below)
        {
            var built = below;
            Failsafe.Run($"building LookingGlass stats for {name}", () => built = layer.Build(below) ?? below);

            return built;
        }
    }
}
