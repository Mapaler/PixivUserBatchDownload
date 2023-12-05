using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Resources;

namespace Pixiv_Login_Redirect_Capture
{
    public partial class Form_Register : Form
    {
        private const string protocolName = "pixiv";
        private const string regArg = "--register";
        private const string unregArg = "--unregister";

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        internal const int BCM_FIRST = 0x1600; //Normal button
        internal const int BCM_SETSHIELD = (BCM_FIRST + 0x000C); //Elevated button

        public Form_Register(string[] args)
        {
            InitializeComponent();
            if (args.Contains(regArg)) //注册
                button_register_Click(this, null);
            if (args.Contains(unregArg)) //注销
                button_unregister_Click(this, null);
            if (!IsAdministrator() && IsRegistered(ClassesKeyMode.Global))
            {
                //AddShieldToButton(button_register); //新版本可以添加到当前用户，就不需要再提权了
                AddShieldToButton(button_unregister); //I非管理员，且注册了全局的情况下，显示提权注销
            }
        }

        /// <summary>
        /// 注册协议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_register_Click(object sender, EventArgs e)
        {
            Reg();
            RefreshButtonEnable();
        }

        /// <summary>
        /// 注销协议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_unregister_Click(object sender, EventArgs e)
        {
            UnReg();
            RefreshButtonEnable();
        }

        /// <summary>
        /// 检测是否是管理员权限
        /// </summary>
        /// <returns></returns>
        public static bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// 注册位置模式描述
        /// </summary>
        public enum ClassesKeyMode
        {
            Auto = 0,
            Global = 1,
            CurrentUser = 2
        }

        /// <summary>
        /// 根据是否有管理员权限，决定目前获取的Classes母键位置是全局还是当前用户
        /// </summary>
        /// <param name="mode">搜索注册位置的模式</param>
        /// <returns>注册位置</returns>
        public static RegistryKey GetClassesKey(ClassesKeyMode mode = ClassesKeyMode.Auto)
        {
            ClassesKeyMode realTarget = mode;
            if (mode == ClassesKeyMode.Auto)
            {
                realTarget = IsAdministrator() ? ClassesKeyMode.Global : ClassesKeyMode.CurrentUser;
            }
            switch (realTarget)
            {
                case ClassesKeyMode.CurrentUser:
                    return Registry.CurrentUser.OpenSubKey("SOFTWARE").OpenSubKey("Classes", true);
                case ClassesKeyMode.Global:
                default:
                    return Registry.ClassesRoot;
            }
        }

        /// <summary>
        /// 检测是否已经注册
        /// </summary>
        /// <param name="mode">搜索注册位置的模式</param>
        /// <returns>是否已注册</returns>
        public static bool IsRegistered(ClassesKeyMode mode = ClassesKeyMode.Auto)
        {
            RegistryKey classesKey = GetClassesKey(mode);
            RegistryKey surekamKey = classesKey.OpenSubKey(protocolName);
            bool registered = surekamKey != null;
            if (registered)
                surekamKey.Close();
            classesKey.Close();
            return registered;
        }

        /// <summary>
        /// 注册启动项到注册表
        /// </summary>
        public static void Reg()
        {
            //注册协议名
            RegistryKey classesKey = GetClassesKey();
            RegistryKey surekamKey = classesKey.CreateSubKey(protocolName, true);
            //设置其为URL协议
            surekamKey.SetValue("URL Protocol", string.Empty);
            //创建打开命令
            RegistryKey shellKey = surekamKey.CreateSubKey("shell");
            RegistryKey openKey = shellKey.CreateSubKey("open");
            RegistryKey commandKey = openKey.CreateSubKey("command");
            //获取当前程序路径
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            commandKey.SetValue(string.Empty, $"\"{exePath}\" \"%1\"");
            surekamKey.Close();
        }

        /// <summary>
        /// 取消注册，删除整个pixiv节点
        /// </summary>
        public static void UnReg()
        {
            //判断当前用户
            if (IsRegistered(ClassesKeyMode.CurrentUser))
            {
                RegistryKey classesKey = GetClassesKey(ClassesKeyMode.CurrentUser);
                classesKey.DeleteSubKeyTree(protocolName);
                classesKey.Close();
            }
            //判断全局
            if (IsRegistered(ClassesKeyMode.Global))
            {
                //没有管理员权限则提权
                if (!IsAdministrator())
                {
                    RestartElevated(unregArg);
                    return;
                }
                else
                {
                    RegistryKey classesKey = GetClassesKey(ClassesKeyMode.Global);
                    classesKey.DeleteSubKeyTree(protocolName);
                    classesKey.Close();
                }
            }
        }

        /// <summary>
        /// 窗口启动时添加盾牌图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register_Load(object sender, EventArgs e)
        {
            RefreshButtonEnable();
            ResourceManager resManager = new ResourceManager(typeof(Form_Register));
            this.Text += $" ({(IsAdministrator() ? resManager.GetString("Identity_Administrator") : resManager.GetString("Identity_CurrentUser"))})";
        }

        /// <summary>
        /// 刷新按钮的注册状态
        /// </summary>
        private void RefreshButtonEnable()
        {
            bool global = IsRegistered(ClassesKeyMode.Global);
            bool user = IsRegistered(ClassesKeyMode.CurrentUser);
            button_register.Enabled = !global && !user;
            button_unregister.Enabled = global || user;
        }

        /// <summary>
        /// 增加盾牌图标
        /// </summary>
        /// <param name="btn">需要增加盾牌图标的按钮</param>
        static internal void AddShieldToButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.System;
            SendMessage(btn.Handle, BCM_SETSHIELD, IntPtr.Zero, new IntPtr(0xFFFFFFFF));
        }

        /// <summary>
        /// 提权重新运行自身
        /// </summary>
        /// <param name="exearg">传递的运行参数</param>
        internal static void RestartElevated(string exearg = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Application.ExecutablePath,
                Verb = "runas",
                Arguments = exearg
            };
            try
            {
                Process p = Process.Start(startInfo);
            }
            catch (Win32Exception ex)
            {
                Debug.Write(ex);
                return;
            }

            Application.Exit();
        }
    }
}
