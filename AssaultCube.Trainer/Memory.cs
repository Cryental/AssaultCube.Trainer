using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace AssaultCube.Trainer
{
    internal class Memory
    {
        public enum AllocationProtectEnum : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }

        public enum StateEnum : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000
        }

        public enum TypeEnum : uint
        {
            MEM_IMAGE = 0x1000000,
            MEM_MAPPED = 0x40000,
            MEM_PRIVATE = 0x20000
        }

        private int _bufSize = 0x1000;
        private uint _mbiSize;
        private IntPtr _mIProcessHandle;
        internal MEMORY_BASIC_INFORMATION[] MemoryBasicInformation;

        internal int ProcessID { get; private set; }
        internal IntPtr BaseAddress { get; private set; }
        internal Process Process { get; private set; }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int OpenProcess(int processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll")]
        private static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        internal void Initialize(string processName)
        {
            if (Process.GetProcessesByName(processName).Length <= 0)
            {
                return;
            }

            OpenProcess(processName);
        }

        private void OpenProcess(string procName)
        {
            Process = Process.GetProcessesByName(procName).FirstOrDefault();

            if (Process == null)
            {
                return;
            }

            ProcessID        = Process.Id;
            _mIProcessHandle = (IntPtr) OpenProcess(0x1F0FFF, false, ProcessID);
            _mbiSize         = (uint) Marshal.SizeOf<MEMORY_BASIC_INFORMATION>();

            BaseAddress = Process.MainModule.BaseAddress;
        }

        internal byte[] Read(IntPtr address, int length)
        {
            var lpBuffer = new byte[length];
            ReadProcessMemory(_mIProcessHandle, address, lpBuffer, length, out _);

            return lpBuffer;
        }

        internal float ReadFloat(IntPtr address)
        {
            return BitConverter.ToSingle(Read(address, 4), 0);
        }

        internal Matrix4x4 ReadMatrix(IntPtr address)
        {
            var lpBuffer = new byte[0x40];
            ReadProcessMemory(_mIProcessHandle, address, lpBuffer, lpBuffer.Length, out _);
            return new Matrix4x4(BitConverter.ToSingle(lpBuffer, 0), BitConverter.ToSingle(lpBuffer, 4), BitConverter.ToSingle(lpBuffer, 8), BitConverter.ToSingle(lpBuffer, 0xC), BitConverter.ToSingle(lpBuffer, 0x10), BitConverter.ToSingle(lpBuffer, 0x14), BitConverter.ToSingle(lpBuffer, 0x18), BitConverter.ToSingle(lpBuffer, 0x1C), BitConverter.ToSingle(lpBuffer, 0x20), BitConverter.ToSingle(lpBuffer, 0x24), BitConverter.ToSingle(lpBuffer, 0x28), BitConverter.ToSingle(lpBuffer, 0x2C), BitConverter.ToSingle(lpBuffer, 0x30), BitConverter.ToSingle(lpBuffer, 0x34), BitConverter.ToSingle(lpBuffer, 0x38), BitConverter.ToSingle(lpBuffer, 0x3C));
        }

        internal string ReadStringAscii(IntPtr address, int size)
        {
            var buffer = new byte[size];

            ReadProcessMemory(_mIProcessHandle, address, buffer, size, out _);

            return Encoding.ASCII.GetString(buffer);
        }

        internal T ReadMemory<T>(IntPtr address)
        {
            var buffer = new byte[Marshal.SizeOf(typeof(T))];

            IntPtr bytesRead;
            ReadProcessMemory(_mIProcessHandle, address, buffer, buffer.Length, out bytesRead);

            var gHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var data = (T) Marshal.PtrToStructure(gHandle.AddrOfPinnedObject(), typeof(T));
            gHandle.Free();

            return data;
        }


        internal Vector3 ReadVector3(IntPtr address)
        {
            var lpBuffer = new byte[12];
            ReadProcessMemory(_mIProcessHandle, address, lpBuffer, lpBuffer.Length, out _);
            return new Vector3 {X = BitConverter.ToSingle(lpBuffer, 0), Y = BitConverter.ToSingle(lpBuffer, 4), Z = BitConverter.ToSingle(lpBuffer, 8)};
        }

        internal bool WriteVector3(IntPtr address, Vector3 targetVector3)
        {
            try
            {
                var buffer = GetBytes(targetVector3);

                return WriteProcessMemory(_mIProcessHandle, address, buffer, buffer.Length, out _);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        internal bool WriteVector2(IntPtr address, Vector2 targetVector2)
        {
            try
            {
                var buffer = GetBytes(targetVector2);

                return WriteProcessMemory(_mIProcessHandle, address, buffer, buffer.Length, out _);
            }
            catch
            {
                return false;
            }
        }

        internal bool WriteMemory<T>(IntPtr address, T t)
        {
            //create byte array with size of type
            var buffer = new byte[Marshal.SizeOf(typeof(T))];

            //allocate handle for buffer
            var gHandle = GCHandle.Alloc(t, GCHandleType.Pinned);
            //arrange data from unmanaged block of memory to structure of type T
            Marshal.Copy(gHandle.AddrOfPinnedObject(), buffer, 0, buffer.Length);
            gHandle.Free(); //release handle

            //change access permission so we can write into memory
            uint oldProtect;
            VirtualProtectEx(_mIProcessHandle, (IntPtr) address, (UIntPtr) buffer.Length, 0x00000004, out oldProtect);

            //write buffer into memory
            IntPtr ptrBytesWritten;
            return WriteProcessMemory(_mIProcessHandle, address, buffer, buffer.Length, out ptrBytesWritten);
        }


        internal byte[] GetBytes<T>(T str)
        {
            var size = Marshal.SizeOf(str);
            var arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        internal Vector2 ReadVector2(IntPtr address)
        {
            var lpBuffer = new byte[8];
            ReadProcessMemory(_mIProcessHandle, address, lpBuffer, lpBuffer.Length, out _);
            return new Vector2 {X = BitConverter.ToSingle(lpBuffer, 0), Y = BitConverter.ToSingle(lpBuffer, 4)};
        }

        internal float ReadPointer(IntPtr address, int pointer)
        {
            var lpBuffer = new byte[8];
            ReadProcessMemory(_mIProcessHandle, address, lpBuffer, lpBuffer.Length, out _);
            return BitConverter.ToSingle(lpBuffer, pointer);
        }

        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public AllocationProtectEnum AllocationProtect;
            public IntPtr RegionSize;
            public StateEnum State;
            public AllocationProtectEnum Protect;
            public TypeEnum Type;
        }
    }
}