using System.Linq;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using WeightedWave = RoR2.InfiniteTowerWaveCategory.WeightedWave;

namespace RiskyCohesion.Fixes;

internal static class SimulacrumAdditionsFamilyWaves
{
    private static readonly string InfiniteTowerRunName = "InfiniteTowerRun";

    private static Rule _spawnableOnly;

    public static void Install(ConfigFile config)
    {
        var description = new Description("Family waves aren't picked on stages where part of their family can't spawn.");
        _spawnableOnly = new Rule(config.Bind(ContentMod.SimulacrumAdditions.WavesSection, "Family Waves", true, description));

        RoR2Application.onLoad += () => Failsafe.Run("limiting Simulacrum Additions' family waves to stages they can spawn on", FollowSetting);
    }

    private static void FollowSetting() => _spawnableOnly.Subscribe(GuardFamilyWaves, UnguardFamilyWaves);

    private static void GuardFamilyWaves()
    {
        foreach (var category in WaveCategories())
            Guard(category.wavePrefabs);
    }

    private static void UnguardFamilyWaves()
    {
        foreach (var category in WaveCategories())
            Unguard(category.wavePrefabs);
    }

    private static void Guard(WeightedWave[] waves)
    {
        for (var i = 0; i < waves.Length; i++)
        {
            var family = FamilyOf(waves[i].wavePrefab);
            if (family && waves[i].prerequisites is not SpawnableFamily)
                waves[i].prerequisites = SpawnableFamily.Guarding(waves[i].prerequisites, family);
        }
    }

    private static void Unguard(WeightedWave[] waves)
    {
        for (var i = 0; i < waves.Length; i++)
        {
            if (waves[i].prerequisites is not SpawnableFamily guard)
                continue;

            waves[i].prerequisites = guard.Original;
            Object.Destroy(guard);
        }
    }

    private static InfiniteTowerWaveCategory[] WaveCategories() => GameModeCatalog.FindGameModePrefabComponent(InfiniteTowerRunName) is InfiniteTowerRun run
        ? run.waveCategories
        : [];

    private static FamilyDirectorCardCategorySelection FamilyOf(GameObject wavePrefab)
    {
        var director = wavePrefab.ComponentOrNull<CombatDirector>();

        return director
            ? director.monsterCards as FamilyDirectorCardCategorySelection
            : null;
    }

    private sealed class SpawnableFamily : InfiniteTowerWavePrerequisites
    {
        public InfiniteTowerWavePrerequisites Original { get; private set; }
        public DirectorCardCategorySelection Family { get; private set; }

        public static SpawnableFamily Guarding(InfiniteTowerWavePrerequisites original, DirectorCardCategorySelection family)
        {
            var guard = CreateInstance<SpawnableFamily>();
            guard.Original = original;
            guard.Family = family;

            return guard;
        }

        public override bool AreMet(InfiniteTowerRun run) => (!Original || Original.AreMet(run)) && WholeFamilyCanSpawn();

        private bool WholeFamilyCanSpawn()
        {
            var scene = SceneInfo.instance;
            if (!scene)
                return true;

            return Family.categories
                .Where(category => category.selectionWeight > 0f)
                .SelectMany(category => category.cards)
                .Where(card => card.selectionWeight > 0 && card.IsAvailable())
                .All(card => CanSpawn(scene, card.GetSpawnCard()));
        }

        private static bool CanSpawn(SceneInfo scene, SpawnCard spawnCard)
        {
            if (!spawnCard)
                return false;

            var nodes = scene.GetNodeGraph(spawnCard.nodeGraphType);

            return nodes && nodes.GetActiveNodesForHullMaskWithFlagConditions(HullMaskOf(spawnCard.hullSize), spawnCard.requiredFlags, spawnCard.forbiddenFlags).Count > 0;
        }

        private static HullMask HullMaskOf(HullClassification hull) => (HullMask)(1 << (int)hull);
    }
}
