using Microsoft.Xna.Framework.Input;
using YamlDotNet.Serialization;
using static Celeste.Mod.AletrisSandbox.AletrisSandboxModuleSettings.HitboxMenu;

namespace Celeste.Mod.AletrisSandbox;

public class AletrisSandboxModuleSettings : EverestModuleSettings
{
    public IWBTGMenu IWBTOptions { get; set; } = new();
    public HealthMenu HealthOptions { get; set; } = new();
    public HitboxMenu HitboxOptions { get; set; } = new();
    public HitboxSizeMenu HitboxSizeOptions { get; set; } = new();
    public HitboxOffsetMenu HitboxOffsetOptions { get; set; } = new();
    public MiscMenu MiscelleaneousMenu { get; set; } = new();

    [DefaultButtonBinding(button: Buttons.Y, key: Keys.C)]
    public ButtonBinding BulletFirekey { get; set; }

    [SettingSubMenu]
    public class IWBTGMenu
    {
        [SettingName("IWBTG Gun Enabled"), SettingSubText("pewpew")]
        public bool IWBTGGunEnableOverride { get; set; } = false;

        [SettingName("IWBTG Gun Visible"), SettingSubText("you should probably keep this on")]
        public bool IWBTGGunVisibleOverride { get; set; } = true;

        [SettingName("IWBTG Jump Enabled"), SettingSubText("does not save"), YamlIgnore]
        public bool IWBTGJumpEnableOverride { get; set; } = false;

        [DefaultButtonBinding(Buttons.RightShoulder, Keys.Z)]
        public ButtonBinding ShootBullet { get; set; }

        [SettingName("Gun sound"), SettingSubText("0 disables the sound"), SettingRange(0, 4, false)]
        public int GunSound { get; set; } = 1;

        [SettingName("Gun Autofire Enabled"), SettingSubText("does not save"), YamlIgnore]
        public bool IWBTGGunAutoFireOverride { get; set; } = false;

        [SettingName("Gun Mouse Aim Enabled"), SettingSubText("does not save"), YamlIgnore]
        public bool IWBTGGunAimOverride { get; set; } = false;

        [SettingName("Gun Destroys Stuff"), SettingSubText("does not save"), YamlIgnore]
        public bool IWBTGGunDestroyStuffOverride { get; set; } = false;

        [SettingName("Gun Hits Stuff"), SettingSubText("does not save"), YamlIgnore]
        public bool IWBTGGunHitsStuffOverride { get; set; } = false;
    }

    [SettingSubMenu]
    public class HitboxMenu
    {
        [SettingName("Player Hitbox Override"), SettingSubText("does not save"), YamlIgnore]
        public bool PlayerHitboxEnableOverride { get; set; } = false;

        [SettingSubMenu]
        public class HitboxSizeMenu
        {
            [SettingName("Normal Hitbox Size X"), SettingSubText("does not save"), YamlIgnore]
            public int NormalHitboxSizeX { get; set; } = 8;

            [SettingName("Normal Hitbox Size Y"), SettingSubText("does not save"), YamlIgnore]
            public int NormalHitboxSizeY { get; set; } = 11;

            [SettingName("Normal Hurtbox Size X"), SettingSubText("does not save"), YamlIgnore]
            public int NormalHurtboxSizeX { get; set; } = 8;

            [SettingName("Normal Hurtbox Size Y"), SettingSubText("does not save"), YamlIgnore]
            public int NormalHurtboxSizeY { get; set; } = 6;

            [SettingName("Crouch Hitbox Size X"), SettingSubText("does not save"), YamlIgnore]
            public int DuckHitboxSizeX { get; set; } = 8;

            [SettingName("Crouch Hitbox Size Y"), SettingSubText("does not save"), YamlIgnore]
            public int DuckHitboxSizeY { get; set; } = 9;

            [SettingName("Crouch Hurtbox Size X"), SettingSubText("does not save"), YamlIgnore]
            public int DuckHurtboxSizeX { get; set; } = 8;

            [SettingName("Crouch Hurtbox Size Y"), SettingSubText("does not save"), YamlIgnore]
            public int DuckHurtboxSizeY { get; set; } = 4;

            [SettingName("Feather Hitbox Size X"), SettingSubText("does not save"), YamlIgnore]
            public int FeatherHitboxSizeX { get; set; } = 8;

            [SettingName("Feather Hitbox Size Y"), SettingSubText("does not save"), YamlIgnore]
            public int FeatherHitboxSizeY { get; set; } = 8;

            [SettingName("Feather Hurtbox Size X"), SettingSubText("does not save"), YamlIgnore]
            public int FeatherHurtboxSizeX { get; set; } = 6;

            [SettingName("Feather Hurtbox Size Y"), SettingSubText("does not save"), YamlIgnore]
            public int FeatherHurtboxSizeY { get; set; } = 6;
        }

        [SettingSubMenu]
        public class HitboxOffsetMenu
        {
            [SettingName("Normal Hitbox Offset X"), SettingSubText("does not save"), YamlIgnore]
            public int NormalHitboxOffsetX { get; set; } = -4;

            [SettingName("Normal Hitbox Offset Y"), SettingSubText("does not save"), YamlIgnore]
            public int NormalHitboxOffsetY { get; set; } = -11;

            [SettingName("Normal Hurtbox Offset X"), SettingSubText("does not save"), YamlIgnore]
            public int NormalHurtboxOffsetX { get; set; } = -4;

            [SettingName("Normal Hurtbox Offset Y"), SettingSubText("does not save"), YamlIgnore]
            public int NormalHurtboxOffsetY { get; set; } = -6;

            [SettingName("Crouch Hitbox Offset X"), SettingSubText("does not save"), YamlIgnore]
            public int DuckHitboxOffsetX { get; set; } = -4;

            [SettingName("Crouch Hitbox Offset Y"), SettingSubText("does not save"), YamlIgnore]
            public int DuckHitboxOffsetY { get; set; } = -11;

            [SettingName("Crouch Hurtbox Offset X"), SettingSubText("does not save"), YamlIgnore]
            public int DuckHurtboxOffsetX { get; set; } = -4;

            [SettingName("Crouch Hurtbox Offset Y"), SettingSubText("does not save"), YamlIgnore]
            public int DuckHurtboxOffsetY { get; set; } = -7;

            [SettingName("Feather Hitbox Offset X"), SettingSubText("does not save"), YamlIgnore]
            public int FeatherHitboxOffsetX { get; set; } = -4;

            [SettingName("Feather Hitbox Offset Y"), SettingSubText("does not save"), YamlIgnore]
            public int FeatherHitboxOffsetY { get; set; } = -10;

            [SettingName("Feather Hurtbox Offset X"), SettingSubText("does not save"), YamlIgnore]
            public int FeatherHurtboxOffsetX { get; set; } = -3;

            [SettingName("Feather Hurtbox Offset Y"), SettingSubText("does not save"), YamlIgnore]
            public int FeatherHurtboxOffsetY { get; set; } = -9;
        }
    }

    [SettingSubMenu]
    public class HealthMenu
    {
        [SettingName("HP System Enabled"), SettingSubText("does not save"), YamlIgnore]
        public bool HPSystemEnableOverride { get; set; } = false;

        [SettingName("HP Override Enabled"), SettingSubText("does not save"), YamlIgnore]
        public bool HPOverrideEnabled { get; set; } = false;

        [SettingName("Max HP Override"), SettingSubText("does not save"), SettingRange(1, 1000, true), YamlIgnore]
        public int HPMaxOverride { get; set; } = 1000;
    }

    [SettingSubMenu]
    public class MiscMenu
    {
        [SettingName("Relativistic Velocity Enabled"), SettingSubText("does not save"), YamlIgnore]
        public bool RelativisticVelocityOverride { get; set; } = false;

        [SettingName("Circle The Madeline"), SettingSubText("does not save"), YamlIgnore]
        public bool CircleMadelineOverride { get; set; } = false;

        [SettingName("Circle Radius"), SettingRange(0, 100, false)]
        public float CircleMadelineRadius { get; set; } = 6f;
    }
}