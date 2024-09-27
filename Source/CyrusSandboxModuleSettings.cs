using Microsoft.Xna.Framework.Input;
using YamlDotNet.Serialization;
using static Celeste.Mod.CyrusSandbox.CyrusSandboxModuleSettings.HitboxMenu;

namespace Celeste.Mod.CyrusSandbox
{
    public class CyrusSandboxModuleSettings : EverestModuleSettings
    {
        public IWBTGMenu IWBTGGunOptions { get; set; } = new IWBTGMenu();
        public HealthMenu HealthOptions { get; set; } = new HealthMenu();
        public HitboxMenu HitboxOptions { get; set; } = new HitboxMenu();
        public HitboxSizeMenu HitboxSizeMenu { get; set; } = new HitboxSizeMenu();
        public HitboxOffsetMenu HitboxOffsetMenu { get; set; } = new HitboxOffsetMenu();

        [SettingName("Circle Madeline")]
        [SettingSubText("does not save")]
        [YamlIgnore]
        public bool CircleMadelineOverride { get; set; } = false;

        [SettingSubMenu]
        public class IWBTGMenu
        {
            [SettingName("IWBTG Gun Enabled")]
            [SettingSubText("does not save")]
            [YamlIgnore]
            public bool IWBTGGunEnableOverride { get; set; } = false;

            [DefaultButtonBinding(Buttons.RightShoulder, Keys.Z)]
            public ButtonBinding ShootBullet { get; set; }

            [SettingName("Gun sound")]
            [SettingSubText("0 disables the sound")]
            [SettingRange(0, 3, false)]
            public int GunSound { get; set; } = 1;

            [SettingName("Gun Autofire Enabled")]
            [SettingSubText("does not save")]
            [YamlIgnore]
            public bool IWBTGGunAutoFireOverride { get; set; } = false;

            [SettingName("Gun Mouse Aim Enabled")]
            [SettingSubText("does not save")]
            [YamlIgnore]
            public bool IWBTGGunAimOverride { get; set; } = false;

            [SettingName("Gun Destroys Stuff")]
            [SettingSubText("does not save")]
            [YamlIgnore]
            public bool IWBTGGunDestroyStuffOverride { get; set; } = false;

            [SettingName("Gun Hits Stuff")]
            [SettingSubText("does not save")]
            [YamlIgnore]
            public bool IWBTGGunHitsStuffOverride { get; set; } = false;
        }

        [SettingSubMenu]
        public class HitboxMenu
        {
            [SettingName("Player Hitbox Override")]
            [SettingSubText("does not save")]
            [YamlIgnore]
            public bool PlayerHitboxEnableOverride { get; set; } = false;

            [SettingSubMenu]
            public class HitboxSizeMenu
            {
                [SettingName("Normal Hitbox Size")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string NormalHitboxSize { get; set; } = "8,11";

                [SettingName("Normal Hurtbox Size")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string NormalHurtboxSize { get; set; } = "8,6";
                [SettingName("Crouch Hitbox Size")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string DuckHitboxSize { get; set; } = "8,9";
                [SettingName("Crouch Hurtbox Size")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string DuckHurtboxSize { get; set; } = "8,4";
                [SettingName("Feather Hitbox Size")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string FeatherHitboxSize { get; set; } = "8,8";
                [SettingName("Feather Hurtbox Size")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string FeatherHurtboxSize { get; set; } = "6,6";
            }

            [SettingSubMenu]
            public class HitboxOffsetMenu
            {
                [SettingName("Normal Hitbox Offset")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string NormalHitboxOffset { get; set; } = "-4,-11";
                [SettingName("Normal Hurtbox Offset")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string NormalHurtboxOffset { get; set; } = "-4,-6";
                [SettingName("Crouch Hitbox Offset")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string DuckHitboxOffset { get; set; } = "-4,-11";
                [SettingName("Crouch Hurtbox Offset")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string DuckHurtboxOffset { get; set; } = "-4,-6";
                [SettingName("Feather Hitbox Offset")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string FeatherHitboxOffset { get; set; } = "-4,-10";
                [SettingName("Feather Hurtbox Offset")]
                [SettingSubText("does not save")]
                [YamlIgnore]
                public string FeatherHurtboxOffset { get; set; } = "-3,-9";
            }

        }

        [SettingSubMenu]
        public class HealthMenu
        {
            [SettingName("HP System Enabled")]
            [SettingSubText("does not save")]
            [YamlIgnore]
            public bool HPSystemEnableOverride { get; set; } = false;

            [SettingName("HP Override Enabled")]
            [SettingSubText("does not save")]
            [YamlIgnore]
            public bool HPOverrideEnabled { get; set; } = false;

            [SettingName("Max HP Override")]
            [SettingSubText("does not save")]
            [SettingRange(1, 1000, true)]
            [YamlIgnore]
            public int HPMaxOverride { get; set; } = 1000;

        }

    }
}
