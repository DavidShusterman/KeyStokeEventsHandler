using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyStrokeEvents
{
    /// <summary>
    /// KeyLogger Object, Used for watching and logging keystrokes of the user
    /// </summary>
    class KeyWatcher
    {
        

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private readonly int WH_KEYBOARD_LL;
        private readonly int WM_KEYDOWN;
        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private readonly int SW_HIDE;
        private readonly List<Keys> _magicKeysCombination;
        private List<Keys> _enteredKeys;
        private bool _sequenceHasStarted;
        public event Action MagicSequencePressed;

        /// <summary>
        /// Constructor, Initializes the KeyWatcher .
        /// </summary>
        /// <param name="combinatiMagicKeys">the combination used to trigger an event</param>
        public KeyWatcher(List<Keys> combinatiMagicKeys)
        {
            WH_KEYBOARD_LL = 13;
            WM_KEYDOWN = 0x0100;
            SW_HIDE = 0;
            _proc = HookCallback;
            _magicKeysCombination = combinatiMagicKeys;
            _enteredKeys = new List<Keys>();
            _sequenceHasStarted = false;
        }

        /// <summary>
        /// Hides Console, sets the hook to the keyboard and runs the application
        /// </summary>
        public void InitializeLogging()
        {
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        /// <summary>
        /// HookCallBack function, triggered when key is pressed.
        /// </summary>
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
            {
                var vkCode = Marshal.ReadInt32(lParam);
                var key = (Keys) vkCode;

                if (!_sequenceHasStarted && key == _magicKeysCombination.First())
                {
                    _sequenceHasStarted = true;
                    _enteredKeys.Add(key);
                }

                if (_sequenceHasStarted && key != _magicKeysCombination.First())
                {
                    HandleSequencePressing(key,nCode,wParam,lParam);
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        /// <summary>
        /// Handle the key being pressed while sequence has started
        /// </summary>
        public void  HandleSequencePressing(Keys key, int nCode, IntPtr wParam, IntPtr lParam)
        {
            _enteredKeys.Add(key);
            var isKeysSequenceMatch = CheckIfKeysEnteredAreLegit(_magicKeysCombination, _enteredKeys);
            if (isKeysSequenceMatch && _enteredKeys.Count == _magicKeysCombination.Count)
            {
                MagicSequencePressed();
                ResetWatchingObjects();
            }
            else if (isKeysSequenceMatch && _enteredKeys.Count != _magicKeysCombination.Count)
            {
                //
            }
            else
            {
                ResetWatchingObjects();
            }
        }

        /// <summary>
        /// Resets the objects used to watch over the keys being pressed
        /// </summary>
        public void ResetWatchingObjects()
        {
            _enteredKeys = new List<Keys>();
            _sequenceHasStarted = false;
        }

        /// <summary>
        /// Checks to see the keys pressed are matched to the magic combination.
        /// </summary>
        public bool CheckIfKeysEnteredAreLegit(List<Keys> magicCombination, List<Keys> enteredKeys)
        {
            var magicCombinationArray = magicCombination.ToArray();
            var enteredKeysArray = enteredKeys.ToArray();

            for (var i = 0; i < enteredKeysArray.Length; i++)
            {
                if (enteredKeys[i] != magicCombinationArray[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Sets the hook to the keyboard keys.
        /// </summary>  
        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        #region DLLImports
        //These Dll's will handle the hooks. Yaaar mateys!

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // The two dll imports below will handle the window hiding.

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        #endregion

    }
}
