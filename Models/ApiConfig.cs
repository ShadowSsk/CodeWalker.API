namespace CodeWalker.API.Models
{
    public class ApiConfig
    {
        public string CodewalkerOutputDir { get; set; } = "";
        public string BlenderOutputDir { get; set; } = "";
        public string FivemOutputDir { get; set; } = "";
        public string RpfArchivePath { get; set; } = "";
        public string GTAPath { get; set; } = "";
        public bool Gen9 { get; set; } = false;
        public string Dlc { get; set; } = "";
        public bool EnableMods { get; set; } = false;
        public int Port { get; set; } = 0;

        /// <summary>
        /// Scan the OpenIV "mods" folder inside the GTA directory and expose its contents.
        /// When enabled, search results include "mods\..." paths and downloads resolve to the
        /// modded copy of a file. Defaults to <see cref="EnableMods"/> when not set.
        /// </summary>
        public bool? UseModsFolder { get; set; } = null;

        /// <summary>
        /// When a file exists both in the mods folder and in the base game, list the
        /// mods copy first. Only meaningful together with <see cref="UseModsFolder"/>.
        /// </summary>
        public bool PreferModsOverBase { get; set; } = true;

        /// <summary>
        /// Scan the DLC packs under update\x64\dlcpacks. Turning this off makes startup
        /// much faster on installs with many packs, at the cost of not finding DLC content.
        /// </summary>
        public bool ScanDlcPacks { get; set; } = true;
    }
}

