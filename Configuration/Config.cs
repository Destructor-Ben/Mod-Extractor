using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ExtractAnyMod.Configuration;
internal class Config : ModConfig
{
#pragma warning disable CS0649
    public static Config Instance => ModContent.GetInstance<Config>();
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [DefaultValue(true)]
    public bool ExtractCode;
    [DefaultValue(true)]
    public bool ExtractResources;
#pragma warning restore CS0649
}
