using StardewModdingAPI;
using StardewValley;

namespace AutoHeal
{
    public class ModEntry : Mod
    {
        readonly ModConfig Config = new();
        int decimal_ = 0;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
        }

        private void GameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api == null)
            {
                this.Monitor.Log("Generic Mod Config Menu not installed. No integration needed", LogLevel.Info);
                return;
            }
            api.RegisterSimpleOption(this.ModManifest, "每秒恢复的血量", "自动回血的速度，0表示关闭该功能",
                 () => this.Config.HealthHealPerSecond, val => this.Config.HealthHealPerSecond = val);

        }
        private void UpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            float healPerSecond = Helper.ReadConfig<ModConfig>().HealthHealPerSecond;
            float healPerTicked = healPerSecond / 60f;
            decimal_ += (int)(healPerTicked * 1000);
            if (decimal_ < 1000)
                return;
            int healInThisTicked = decimal_ / 1000;
            decimal_ %= 1000;
            Game1.player.health += healInThisTicked;
        }
    }
}
