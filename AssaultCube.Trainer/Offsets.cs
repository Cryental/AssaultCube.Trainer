namespace AssaultCube.Trainer
{
    internal static class Offsets
    {
        internal static int BaseAddress = 0x50F4E8;
        internal static int ViewMatrix = 0x501AE8;
        internal static int PauseGame = 0x510CE0;
        internal static int GameVersion = 0x510CF4;
        internal static int CrosshairSize = 0x50F20C;
        internal static int DrawGun = 0x50F200;
        internal static int HideRadar = 0x50F21C;
        internal static int AutoReload = 0x5101D0;
        internal static int HandPosition = 0x510A4C; //0 - Left, 1 - Right
        internal static int RadarZoom = 0x50F264;
        internal static int DrawFPS = 0x50F210;

        internal static int PlayerEntityPointer = 0x0C;
        internal static int PlayerArrayPointer = 0x10;
        internal static int PlayerCountPointer = 0x18;

        // BaseAddress -> Pointer
        internal static int NamePointer = 0x0224;
        internal static int TeamPointer = 0x32C;

        internal static int SpeedPointer = 0x0080;
        internal static int HealthPointer = 0xF8;
        internal static int HeadPositionPointer = 0x04;
        internal static int FootPositionPointer = 0x34;
        internal static int VelocityPointer = 0x10;
        internal static int AnglePointer = 0x40;
        internal static int PrimaryAmmoPointer = 0x150;
        internal static int PistolAmmoPointer = 0x13C;
        internal static int GrenadeAmmoPointer = 0x158;
        internal static int ArmorPointer = 0x158;
        internal static int FiredPointer = 0x01A0;

        // PlayerEntity -> CurrentWeapon
        internal static int CurrentWeaponPointer = 0x0378;
        internal static int WeaponPointer = 0x10;

        // PlayerEntity -> CurrentWeapon -> Weapon -> Pointer
        internal static int DelayTime = 0x50;
        internal static int WeaponPrimaryAmmoPointer = 0x28;
        internal static int WeaponPistolPointer = 0x14;
        internal static int WeaponGrenadeAmmoPointer = 0x10;
        internal static int WeaponPrimaryAmmoClipPointer = 0x0;
        internal static int WeaponPrimaryStatusPointer = 0x50; //120 - Shooting, 2000 - Reloading
        internal static int WeaponPistolStatusPointer = 0x3C;  //160 - Shooting, 1400 - Reloading
    }
}