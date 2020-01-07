using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Memory;

namespace AssaultCube.Trainer
{
    class Program
    {
        internal static Memory memory = new Memory();

        internal static Player LocalPlayer;
        internal static List<Player> Players = new List<Player>();
        internal static int PlayerCount;

        static void Main(string[] args)
        {
            memory.Initialize("ac_client");

            ReadLocalPlayer();
            LocalPlayer.Health = 1000;

            while (true)
            {
                Thread.Sleep(1);

                ReadPlayers();

                Console.WriteLine(Players[1].HeadPosition);
            }
        }

        private static void ReadLocalPlayer()
        {
            int localPlayerPointer = memory.ReadMemory<int>((IntPtr) Offsets.BaseAddress + Offsets.PlayerEntityPointer);
            LocalPlayer = new Player(localPlayerPointer);
        }

        private static void ReadPlayers()
        {
            int localPlayerPointer = memory.ReadMemory<int>((IntPtr)Offsets.BaseAddress + Offsets.PlayerEntityPointer);
            LocalPlayer = new Player(localPlayerPointer);

            Players.Clear();

            PlayerCount = memory.ReadMemory<int>((IntPtr) (Offsets.BaseAddress + Offsets.PlayerCountPointer));
            int playerArray = memory.ReadMemory<int>((IntPtr) (Offsets.BaseAddress + Offsets.PlayerArrayPointer));
            
            for (int i = 0; i < PlayerCount; i++)
            {
                if (i == 0)
                {
                    Players.Add(new Player(localPlayerPointer));
                }
                else
                {
                    int playerPointer = memory.ReadMemory<int>((IntPtr)playerArray + (i * 0x04));
                    Players.Add(new Player(playerPointer));
                }
            }
        }
    }
}