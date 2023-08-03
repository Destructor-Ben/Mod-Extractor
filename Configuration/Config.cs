#pragma warning disable CS0649
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ExtractAnyMod.Configuration;
internal class Config : ModConfig
{
    public static Config Instance => ModContent.GetInstance<Config>();
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [DefaultValue(true)]
    public bool ExtractCode;
    [DefaultValue(true)]
    public bool ExtractResources;
}
