using UnityEngine;

namespace RiskyCohesion;

internal static class GameObjectExtensions
{
    public static T ComponentOrNull<T>(this GameObject gameObject) where T : Component => gameObject
        ? gameObject.GetComponent<T>()
        : null;
}
