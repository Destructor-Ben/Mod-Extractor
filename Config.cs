using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ExtractAnyMod;
[Label("Config")]
public class Config : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;
    public static Config Instance => ModContent.GetInstance<Config>();

    [Label("[i:Cog] Extract Code")]
    [Tooltip("Whether a dll containing the code of the mod will be extracted")]
    [DefaultValue(true)]
    public bool extractCode;
    [Label("[i:PaintingMartiaLisa] Extract Resources")]
    [Tooltip("Whether resources, like textures and sounds, will be extracted")]
    [DefaultValue(true)]
    public bool extractResources;
}
