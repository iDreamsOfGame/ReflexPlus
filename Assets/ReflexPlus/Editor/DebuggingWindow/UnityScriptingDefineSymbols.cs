﻿using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;

public static class UnityScriptingDefineSymbols
{
    public static bool IsDefined(string symbol)
    {
        var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
        var symbols = GetSymbols(platform);
        return symbols.Contains(symbol);
    }

    public static void Add(string symbol, BuildTargetGroup platform)
    {
        var symbols = GetSymbols(platform);
        symbols.Add(symbol);
        SetSymbols(symbols, platform);
    }

    public static void Remove(string symbol, BuildTargetGroup platform)
    {
        var symbols = GetSymbols(platform);
        symbols.Remove(symbol);
        SetSymbols(symbols, platform);
    }

    private static HashSet<string> GetSymbols(BuildTargetGroup platform)
    {
        return new HashSet<string>(PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(platform)).Split(';'));
    }

    private static void SetSymbols(HashSet<string> symbols, BuildTargetGroup platform)
    {
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(platform), string.Join(";", symbols));
    }

    public static void Toggle(string symbol, BuildTargetGroup platform)
    {
        var symbols = GetSymbols(platform);
        if (!symbols.Add(symbol))
            symbols.Remove(symbol);

        SetSymbols(symbols, platform);
    }
}