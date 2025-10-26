namespace DuckovESPv3.Core.Configuration
{
    public class MeleeAutoAttackConfig
    {
        [ConfigEntry("MeleeAutoAttack.Enable", DefaultValue = false)]
        public bool Enable { get; set; }

        [ConfigEntry("MeleeAutoAttack.RequireEnemyAiming", DefaultValue = false)]
        public bool RequireEnemyAiming { get; set; } = false;
    }
}
