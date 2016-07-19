using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcherConsole.WinAPI
{
    class Win32Utilities
    {
        /// <summary>
        /// Throws a Win32Exception with the supplied Windows System Error Code if it is not ERROR_SUCCESS.
        /// See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms681382(v=vs.85).aspx
        /// </summary>
        /// <param name="resultCode">A Windows System Error Code</param>
        public static void ThrowIfResultCodeNotSuccess(int resultCode)
        {
            if (resultCode != Win32Constants.ERROR_SUCCESS)
            {
                throw new System.ComponentModel.Win32Exception(resultCode);
            }
        }
    }
}
