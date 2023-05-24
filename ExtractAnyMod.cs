using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace ExtractAnyMod;
public class ExtractAnyMod : Mod
{
    private static MethodInfo extractMethod;
    private static Type buildPropertiesType;
    private static ILHook hook;
    private static event ILContext.Manipulator Hook_UIExtractMod_Extract
    {
        add
        {
            hook = new ILHook(extractMethod, value);
            hook.Apply();
        }

        remove
        {
            hook?.Undo();
            hook = null;
        }
    }

    public override void Load()
    {
        // Reflection
        var assembly = typeof(Main).Assembly;
        buildPropertiesType = assembly.GetType("Terraria.ModLoader.Core.BuildProperties");
        extractMethod = assembly.GetType("Terraria.ModLoader.UI.UIExtractMod")?.GetMethod("Extract", BindingFlags.NonPublic | BindingFlags.Instance);

        // Null check
        if (extractMethod is null)
            throw new Exception("'extractMethod' was null");
        if (buildPropertiesType is null)
            throw new Exception("'buildPropertiesType' was null");

        // Hook
        Hook_UIExtractMod_Extract += HookExtractMod;
    }

    public override void Unload()
    {
        Hook_UIExtractMod_Extract -= HookExtractMod;

        extractMethod = null;
        buildPropertiesType = null;
    }

    private void HookExtractMod(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            while (c.TryGotoNext(i => i.MatchLdfld(buildPropertiesType.FullName, "hideCode")))
            {
                c.Index++;
                c.Emit(OpCodes.Pop);
                c.EmitDelegate(delegate () { return !Config.Instance.extractCode; });
            }

            c.Index = 0;

            while (c.TryGotoNext(i => i.MatchLdfld(buildPropertiesType.FullName, "hideResources")))
            {
                c.Index++;
                c.Emit(OpCodes.Pop);
                c.EmitDelegate(delegate () { return !Config.Instance.extractResources; });
            }
        }
        catch (Exception e)
        {
            throw new ILPatchFailureException(this, il, e);
        }

        Logger.Debug("ExtractMod ILHook successfully applied");
    }
}