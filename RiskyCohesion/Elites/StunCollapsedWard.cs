using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyCohesion.Elites;

internal sealed class StunCollapsedWard : MonoBehaviour
{
    private ElitePassiveSuppression _suppression;
    private CharacterBody _owner;
    private BuffWard _ward;
    private float _radius;

    public static void Attach(GameObject wardObject, CharacterBody owner, ElitePassiveSuppression suppression)
    {
        if (!NetworkServer.active || !wardObject)
            return;

        var ward = wardObject.GetComponent<BuffWard>();
        if (!ward)
            return;

        var collapsedWard = wardObject.AddComponent<StunCollapsedWard>();
        collapsedWard._suppression = suppression;
        collapsedWard._owner = owner;
        collapsedWard._ward = ward;
        collapsedWard._radius = ward.radius;

        suppression.Enabled.SettingChanged += collapsedWard.RemoveWhenDisabled;
    }

    private void OnDestroy() => _suppression.Enabled.SettingChanged -= RemoveWhenDisabled;

    private void RemoveWhenDisabled(object sender, EventArgs args)
    {
        if (_suppression.IsOn)
            return;

        _ward.Networkradius = _radius;
        Destroy(this);
    }

    private void FixedUpdate()
    {
        var radius = ElitePassiveSuppression.Suppresses(_owner)
            ? 0f
            : _radius;

        if (_ward.radius != radius)
            _ward.Networkradius = radius;
    }
}
