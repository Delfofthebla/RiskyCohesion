using LookingGlass;
using UnityEngine;

namespace RiskyCohesion.LookingGlassStats;

internal static class StatMath
{
    private static readonly float BonusPerBoostItem = 0.1f;

    public static float Linear(float first, float perStack, int stack) => first + perStack * (stack - 1);

    public static float Hyperbolic(float first, float perStack, int stack) => Utils.GetHyperbolicStacking(first, perStack, stack);

    public static float Exponential(float first, float perStack, int stack) => Utils.GetExponentialStacking(first, perStack, stack);

    public static float ReducedPerStack(float first, float reductionPerStack, int stack) => first * Mathf.Pow(1f - reductionPerStack, stack - 1);

    public static float WithLuck(float chance, float luck) => Utils.CalculateChanceWithLuck(chance, luck);

    public static float BoostBonus(float boostItems) => BonusPerBoostItem * boostItems;

    public static float BoostMultiplier(float boostItems) => 1f + BoostBonus(boostItems);

    public static int BoostItemsFor(float multiplier) => Mathf.RoundToInt((multiplier - 1f) / BonusPerBoostItem);
}
