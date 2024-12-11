// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eco.Shared.Services;
using UnityEngine;

/// <summary>A variant of mono behaviour that allows to easly found memory leaks. It saves weak reference to each monobehaviour</summary>
public class TrackableBehavior : MonoBehaviour
{
    public static List<WeakReference<TrackableBehavior>> References = new ();
    public TrackableBehavior()
    {
        if (QualityAssurance.Enabled) //Disabled for production so it wont affect performance
            References.Add(new WeakReference<TrackableBehavior>(this));
    }
    
    static string report = null;
    static float lastTimeReportGenerated = 0f;
    const float secondsToConsiderReportOutdated = 20;
    /// <summary> Enumerates Count/Type of not-removed monobehaviours </summary>
    public static string PrepareReport()
    {
        if (report == null || Time.time - lastTimeReportGenerated > secondsToConsiderReportOutdated)
        {
            GC.Collect(); //Force GC to ensure that there are will be left only stucked behaviours
            lastTimeReportGenerated = Time.time;
            var errors = UndeletedBehaviours();
            if (errors.Count > 0)
            {
                StringBuilder s = new();
                foreach (var e in errors)
                    s.AppendLine($"Count: {e.Item2}. Type: {e.Item1.FullName}");
                report = s.ToString();
            }
            else  report = string.Empty;
        }
        return report;
    }

    public static List<(Type, int, List<TrackableBehavior>)> UndeletedBehaviours()
    {
        return References.Select(x=>x.TryGetTarget(out var t) ? t : null)          //Get all behaviours that are not garbabge collected and return nulls for all that were collected
            .Where(x => !object.ReferenceEquals(x, null) && x == null)             //Skip nulls and behaviours that were not destroyed (x == null returns true if unity object is destroyed even when it's not actually null)
            .GroupBy(x => x.GetType()).Select(x => (x.Key, x.Count(), x.ToList())) //Group them by type and count how many destroyed objects were not collected
            .OrderByDescending(x => x.Item2).ToList();
    }
}
