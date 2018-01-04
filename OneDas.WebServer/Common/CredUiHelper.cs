using System;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;

namespace OneDas.Common
{
    public static class CredUiHelper
    {
        #region "Methods"

        public static bool PromptForWindowsCredentials(bool decryptPassword, string caption, string message, ref string domain, ref string userName, ref string password)
        {
            bool isValid = false;
            CREDUI_INFO credUiInfo = new CREDUI_INFO();

            credUiInfo.cbSize = Marshal.SizeOf(credUiInfo);
            credUiInfo.pszCaptionText = caption;
            credUiInfo.pszMessageText = message;

            // CredUIPromptForWindowsCredentials
            int authError = 0;
            uint authPackage = 0;
            IntPtr authBuffer = IntPtr.Zero;
            uint authBufferSize = 0;
            IntPtr outAuthBuffer = default;
            uint outAuthBufferSize = 1000;
            bool save = false;
            CredUIReturnCodes credUIReturnCode = default;

            // CredUnPackAuthenticationBuffer / CredUIParseUserName
            StringBuilder domain_2 = new StringBuilder(255);
            StringBuilder userName_2 = new StringBuilder(255);
            StringBuilder password_2 = new StringBuilder(255);

            while (true)
            {
                domain_2 = new StringBuilder(255);
                userName_2 = new StringBuilder(255);
                password_2 = new StringBuilder(255);

                uint maxDomain = Convert.ToUInt32(domain_2.Capacity);
                uint maxUserName = Convert.ToUInt32(userName_2.Capacity);
                uint maxPassword = Convert.ToUInt32(password_2.Capacity);

                credUIReturnCode = CredUiHelper.CredUIPromptForWindowsCredentials(ref credUiInfo, authError, ref authPackage, authBuffer, authBufferSize, ref outAuthBuffer, ref outAuthBufferSize, ref save, 0);

                if (credUIReturnCode == CredUIReturnCodes.ERROR_CANCELLED)
                {
                    break;
                }

                CredUiHelper.CredUnPackAuthenticationBuffer(decryptPassword ? 0x1 : 0x0, outAuthBuffer, outAuthBufferSize, userName_2, ref maxUserName, domain_2, ref maxDomain, password_2, ref maxPassword);
                credUIReturnCode = CredUiHelper.CredUIParseUserName(userName_2.ToString(), userName_2, 100, domain_2, 100);

                authError = 0;

                switch (credUIReturnCode)
                {
                    case CredUIReturnCodes.ERROR_INVALID_ACCOUNT_NAME:

                        authError = 1315; // ERROR_INVALID_ACCOUNT_NAME

                        continue;

                    case CredUIReturnCodes.NO_ERROR:

                        if (string.Compare(domain_2.ToString(), Environment.MachineName, true) == 0)
                        {
                            using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
                            {
                                isValid = context.ValidateCredentials(userName_2.ToString(), password_2.ToString());
                            }
                        }
                        else
                        {
                            try
                            {
                                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain_2.ToString()))
                                {
                                    isValid = context.ValidateCredentials(userName_2.ToString(), password_2.ToString());
                                }
                            }
                            catch (PrincipalServerDownException)
                            {
                                authError = 1355; // ERROR_NO_SUCH_DOMAIN

                                continue;
                            }
                            catch (Exception)
                            {
                                break;
                            }
                        }

                        break;

                    default:
                        throw new Win32Exception((int)credUIReturnCode);
                }

                if (isValid)
                {
                    break;
                }
                else
                {
                    authError = 1326; // ERROR_LOGON_FAILURE
                }
            }

            CredUiHelper.CoTaskMemFree(outAuthBuffer);

            if (isValid)
            {
                domain = domain_2.ToString();
                userName = userName_2.ToString();
                password = password_2.ToString();
            }

            return isValid;
        }

        public static void ExecuteAsElevatedUser(string argument, string promptForCredentialsMessage, Action action)
        {
            WindowsPrincipal currentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool isElevated = currentPrincipal.IsInRole(WindowsBuiltInRole.Administrator) || currentPrincipal.Claims.Any(x => x.Value == "S-1-5-18");
            bool canElevate = currentPrincipal.Claims.Any(x => x.Value == "S-1-5-32-544");

            if (isElevated) // execute (user is elevated)
            {
                action.Invoke();
            }
            else // (change and) elevate current user
            {
                if (canElevate)
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        Arguments = argument,
                        FileName = Assembly.GetEntryAssembly().Location,
                        UseShellExecute = true,
                        Verb = "runas",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    Process process = new Process
                    {
                        StartInfo = processStartInfo,
                        EnableRaisingEvents = true
                    };

                    if (process.Start())
                    {
                        process.WaitForExit();
                    }
                }
                else
                {
                    string domain = string.Empty;
                    string username = string.Empty;
                    string password = string.Empty;

                    if (CredUiHelper.PromptForWindowsCredentials(true, "OneDas.Server", promptForCredentialsMessage, ref domain, ref username, ref password))
                    {
                        SecureString passwordSecure = new SecureString();
                        password.ToCharArray().ToList().ForEach(x => passwordSecure.AppendChar(x));
                        passwordSecure.MakeReadOnly();

                        ProcessStartInfo processStartInfo = new ProcessStartInfo
                        {
                            Arguments = argument,
                            FileName = Assembly.GetExecutingAssembly().Location,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            Domain = domain,
                            UserName = username,
                            Password = passwordSecure
                        };

                        Process process = new Process
                        {
                            StartInfo = processStartInfo,
                            EnableRaisingEvents = true
                        };

                        if (process.Start())
                        {
                            process.WaitForExit();
                        }
                    }
                }
            }
        }
        #endregion

        #region "Native"

        [DllImport("credui.dll", EntryPoint = "CredUIPromptForWindowsCredentials", CharSet = CharSet.Unicode)]
        private static extern CredUIReturnCodes CredUIPromptForWindowsCredentials(ref CREDUI_INFO pUiInfo,
                                                                                    Int32 dwAuthError,
                                                                                    ref UInt32 pulAuthPackage,
                                                                                    IntPtr pvInAuthBuffer,
                                                                                    UInt32 ulInAuthBufferSize,
                                                                                    ref IntPtr ppvOutAuthBuffer,
                                                                                    ref UInt32 pulOutAuthBufferSize, [MarshalAs(UnmanagedType.Bool)] ref bool pfSave, CredUIWinFlags dwFlags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CredUnPackAuthenticationBuffer(Int32 dwFlags,
                                                                IntPtr pAuthBuffer,
                                                                UInt32 cbAuthBuffer,
                                                                StringBuilder pszUserName,
                                                                ref UInt32 pcchMaxUserName,
                                                                StringBuilder pszDomainName,
                                                                ref UInt32 pcchMaxDomainName,
                                                                StringBuilder pszPassword,
                                                                ref UInt32 pcchMaxPassword);

        [DllImport("credui.dll", EntryPoint = "CredUIParseUserNameW", CharSet = CharSet.Unicode)]
        private static extern CredUIReturnCodes CredUIParseUserName(string userName, StringBuilder user, int userMaxChars, StringBuilder domain, int domainMaxChars);

        [DllImport("ole32.dll")]
        private static extern void CoTaskMemFree(IntPtr ptr);

        #endregion
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CREDUI_INFO
    {
        public int cbSize;
        public IntPtr hwndParent;
        public string pszMessageText;
        public string pszCaptionText;
        public IntPtr hbmBanner;
    }

    [Flags]
    public enum CredUIWinFlags : int
    {
        CredUIWin_Generic = 0x1,
        CredUIWin_Checkbox = 0x2,
        CredUIWin_AuthPackage_Only = 0x10,
        CredUIWin_InCred_Only = 0x20,
        CredUIWin_Enumerate_Admins = 0x100,
        CredUIWin_Enumerate_CurrentUser = 0x200,
        CredUIWin_Secure_Prompt = 0x1000,
        CredUIWin_Pack_32_Wow = 0x10000000
    }

    public enum CredUIReturnCodes : int
    {
        NO_ERROR = 0,
        ERROR_INVALID_PARAMETER = 87,
        ERROR_INSUFFICIENT_BUFFER = 122,
        ERROR_BAD_ARGUMENTS = 160,
        ERROR_INVALID_FLAGS = 1004,
        ERROR_NOT_FOUND = 1168,
        ERROR_CANCELLED = 1223,
        ERROR_NO_SUCH_LOGON_SESSION = 1312,
        ERROR_INVALID_ACCOUNT_NAME = 1315
    }
}