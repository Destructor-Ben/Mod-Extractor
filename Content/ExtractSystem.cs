using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace ExtractAnyMod.Content;
internal class ExtractSystem : ModSystem
{
    private static Type BuildProperties => typeof(Main).Assembly.GetType("Terraria.ModLoader.Core.BuildProperties") ?? throw new Exception("'BuildProperties' is null");
    private static MethodInfo ExtractMethod => (typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIExtractMod")?.GetMethod("Extract", BindingFlags.NonPublic | BindingFlags.Instance)) ?? throw new Exception("'ExtractMethod' is null");
    private static FieldInfo HideCode => BuildProperties.GetField("hideCode", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new Exception("'hideCode' is null");
    private static FieldInfo HideResources => BuildProperties.GetField("hideResources", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new Exception("'hideResources' is null");

    private static ILHook hook;
    private static event ILContext.Manipulator Hook_UIExtractMod_Extract
    {
        add
        {
            hook = new ILHook(ExtractMethod, value);
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
        Hook_UIExtractMod_Extract += HookExtractMod;
    }

    public override void Unload()
    {
        Hook_UIExtractMod_Extract -= HookExtractMod;
    }

    private void HookExtractMod(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            while (c.TryGotoNext(i => i.MatchLdfld(HideCode)))
            {
                c.Index++;
                c.Emit(OpCodes.Pop);
                c.EmitDelegate(delegate () { return !Config.Instance.ExtractCode; });
            }

            c.Index = 0;

            while (c.TryGotoNext(i => i.MatchLdfld(HideResources)))
            {
                c.Index++;
                c.Emit(OpCodes.Pop);
                c.EmitDelegate(delegate () { return !Config.Instance.ExtractResources; });
            }
        }
        catch (Exception e)
        {
            throw new ILPatchFailureException(Mod, il, e);
        }
    }
}
