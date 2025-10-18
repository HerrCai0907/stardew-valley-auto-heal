using StardewModdingAPI;
using StardewValley;

namespace AutoHeal
{
    public class ModEntry : Mod
    {
        ModConfig Config = new();
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
                this.Monitor.Log("Generic Mod Config Menu not installed", LogLevel.Info);
                return;
            }
            this.Monitor.Log("register to GMCM menu", LogLevel.Info);
            api.RegisterModConfig(this.ModManifest, () => this.Config = new ModConfig(), () => Helper.WriteConfig(this.Config));
            api.RegisterSimpleOption(this.ModManifest, "每秒恢复的血量", "自动回血的速度，0表示关闭该功能",
                 () => this.Config.HealthHealPerSecond, val => this.Config.HealthHealPerSecond = val);

        }
        private void UpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            int maxHealth = Game1.player.maxHealth;
            int health = Game1.player.health;
            if (health == maxHealth) return;
            if (!Context.IsWorldReady) return;
            float healPerSecond = Helper.ReadConfig<ModConfig>().HealthHealPerSecond;
            float healPerTicked = healPerSecond / 60f;
            decimal_ += (int)(healPerTicked * 1000);
            if (decimal_ < 1000)
                return;
            int healInThisTicked = decimal_ / 1000;
            decimal_ %= 1000;
            Game1.player.health = Math.Min(health + healInThisTicked, maxHealth);
        }
    }
}
