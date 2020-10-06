/*
 * Created by SharpDevelop.
 * User: Benoit Le Guern
 * Date: 11/06/2008
 * Time: 15:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Proxy
{
	class MainClass
	{
		#region WININET Options
        private const uint INTERNET_PER_CONN_PROXY_SERVER = 2;
        private const uint INTERNET_PER_CONN_PROXY_BYPASS = 3;
        private const uint INTERNET_PER_CONN_FLAGS = 1;

        private const uint INTERNET_OPTION_REFRESH = 37;
        private const uint INTERNET_OPTION_PROXY = 38;
        private const uint INTERNET_OPTION_SETTINGS_CHANGED = 39;
        private const uint INTERNET_OPTION_END_BROWSER_SESSION = 42;
        private const uint INTERNET_OPTION_PER_CONNECTION_OPTION = 75;

        private const uint PROXY_TYPE_DIRECT = 0x1;
        private const uint PROXY_TYPE_PROXY = 0x2;
        
        private const uint INTERNET_OPEN_TYPE_PROXY = 3;
        #endregion

        #region USER32 Options
        static IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        static IntPtr WM_SETTINGCHANGE = new IntPtr(0x001A);
        #endregion

        #region STRUCT
        enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0000,
            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008
        }

        [StructLayout(LayoutKind.Sequential)]
        struct INTERNET_PER_CONN_OPTION_LIST
        {
            uint dwSize;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 256)]
            string pszConnection;
            uint dwOptionCount;
            uint dwOptionError;
            IntPtr pOptions;

        };
        #endregion

        #region Interop
        [DllImport("wininet.dll", EntryPoint = "InternetSetOptionA", 
                  CharSet = CharSet.Ansi, SetLastError = true, PreserveSig = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, uint dwOption,
                                                     IntPtr pBuffer, int dwReserved);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageTimeout(IntPtr hWnd,
                                                uint Msg, 
                                                UIntPtr wParam, 
                                                UIntPtr lParam, 
                                                SendMessageTimeoutFlags fuFlags,
                                                uint uTimeout, 
                                                out UIntPtr lpdwResult);
        #endregion

        internal static void Notify_OptionSettingChanges()
        {
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }
        
        internal static void Notify_SettingChange()
        {
            UIntPtr result;
            SendMessageTimeout(HWND_BROADCAST, (uint)WM_SETTINGCHANGE,
                               UIntPtr.Zero, UIntPtr.Zero, 
                                SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out result);
        }
        
		public static void Main(string[] args)
		{
			Notify_OptionSettingChanges();
			Notify_SettingChange();
		}
	}
}
