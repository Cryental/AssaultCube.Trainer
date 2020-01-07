using System;
using System.Numerics;

namespace AssaultCube.Trainer
{
    internal class Player
    {
        private readonly int _playerPointer;

        public Weapon Weapon;

        public Player(int playerPointer)
        {
            _playerPointer = playerPointer;
            Weapon         = new Weapon(_playerPointer);
        }

        public string Name
        {
            get { return Program.memory.ReadStringAscii((IntPtr) (_playerPointer + Offsets.NamePointer), 17).Remove(0, 1); }
        }

        public int Team
        {
            get { return Program.memory.ReadMemory<int>((IntPtr) (_playerPointer + Offsets.TeamPointer)); }
        }

        public int Health
        {
            get { return Program.memory.ReadMemory<int>((IntPtr)(_playerPointer + Offsets.HealthPointer)); }
            set { Program.memory.WriteMemory((IntPtr) (_playerPointer + Offsets.HealthPointer), value); }
        }

        public int Speed
        {
            get { return Program.memory.ReadMemory<int>((IntPtr)(_playerPointer + Offsets.SpeedPointer)); }
            set { Program.memory.WriteMemory((IntPtr)(_playerPointer + Offsets.SpeedPointer), value); }
        }
        public int FireCount
        {
            get { return Program.memory.ReadMemory<int>((IntPtr)(_playerPointer + Offsets.FiredPointer)); }
            set { Program.memory.WriteMemory((IntPtr)(_playerPointer + Offsets.FiredPointer), value); }
        }

        public Vector3 HeadPosition
        {
            get { return Program.memory.ReadVector3((IntPtr) (_playerPointer + Offsets.HeadPositionPointer)); }
            set { Program.memory.WriteVector3((IntPtr) (_playerPointer + Offsets.HeadPositionPointer), value); }
        }

        public Vector3 FootPosition
        {
            get { return Program.memory.ReadVector3((IntPtr) (_playerPointer + Offsets.HeadPositionPointer)); }
            set { Program.memory.WriteVector3((IntPtr) (_playerPointer + Offsets.FootPositionPointer), value); }
        }

        public Vector3 Velocity
        {
            get { return Program.memory.ReadVector3((IntPtr) (_playerPointer + Offsets.VelocityPointer)); }
            set { Program.memory.WriteVector3((IntPtr) (_playerPointer + Offsets.VelocityPointer), value); }
        }

        public Vector3 Angle
        {
            get { return Program.memory.ReadVector3((IntPtr) (_playerPointer + Offsets.AnglePointer)); }
            set { Program.memory.WriteVector3((IntPtr) (_playerPointer + Offsets.AnglePointer), value); }
        }
    }

    internal class Weapon
    {
        private readonly int _weaponPointer;
        private readonly int _playerPointer;

        public Weapon(int playerPointer)
        {
            var currentWeapon = Program.memory.ReadMemory<int>((IntPtr) (playerPointer + Offsets.CurrentWeaponPointer));
            _weaponPointer = Program.memory.ReadMemory<int>((IntPtr) (currentWeapon + Offsets.WeaponPointer));
            _playerPointer = playerPointer;
        }

        public int PrimaryAmmo
        {
            get { return Program.memory.ReadMemory<int>((IntPtr) (_weaponPointer + Offsets.WeaponPrimaryAmmoPointer)); }
            set { Program.memory.WriteMemory((IntPtr) (_weaponPointer + Offsets.WeaponPrimaryAmmoPointer), value); }
        }

        public int PistolAmmo
        {
            get { return Program.memory.ReadMemory<int>((IntPtr)(_weaponPointer + Offsets.WeaponPistolPointer)); }
            set { Program.memory.WriteMemory((IntPtr)(_weaponPointer + Offsets.WeaponPistolPointer), value); }
        }

        public int GrenadeAmmo
        {
            get { return Program.memory.ReadMemory<int>((IntPtr)(_weaponPointer + Offsets.WeaponGrenadeAmmoPointer)); }
            set { Program.memory.WriteMemory((IntPtr)(_weaponPointer + Offsets.WeaponGrenadeAmmoPointer), value); }
        }

        public int AmmoClip
        {
            get { return Program.memory.ReadMemory<int>((IntPtr) (_weaponPointer + Offsets.WeaponPrimaryAmmoClipPointer)); }
            set { Program.memory.WriteMemory((IntPtr) (_weaponPointer + Offsets.WeaponPrimaryAmmoClipPointer), value); }
        }

        public int DelayTime
        {
            get { return Program.memory.ReadMemory<int>((IntPtr) (_weaponPointer + Offsets.DelayTime)); }
            set { Program.memory.WriteMemory((IntPtr) (_weaponPointer + Offsets.DelayTime), value); }
        }
    }
}