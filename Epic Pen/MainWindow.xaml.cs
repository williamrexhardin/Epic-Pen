//Copyright (c) 2010 Brian Hoary

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.IO;

namespace click_through_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string AssemblyTitle
       {
           get
           {
               // Get all Title attributes on this assembly
                object[] attributes = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    System.Reflection.AssemblyTitleAttribute titleAttribute = (System.Reflection.AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public const int WM_HOTKEY = 0x0312;
        public const int VIRTUALKEYCODE_FOR_CAPS_LOCK = 0x14;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        WindowInteropHelper _host;

        public MainWindow()
        {
            InitializeComponent();

            //global hotkeys:
            _host = new WindowInteropHelper(this);

            SetupHotKey(_host.Handle);
            ComponentDispatcher.ThreadPreprocessMessage += new ThreadMessageEventHandler(ComponentDispatcher_ThreadPreprocessMessage);
        }

        enum hotkeys
        {
            _1=0x31,
            _2=0x32,
            _3=0x33,
            _4=0x34,
            _5=0x35,
            _6=0x36,
        }

        private void SetupHotKey(IntPtr handle)
        {
            RegisterHotKey(handle, GetType().GetHashCode(), 2, (int)hotkeys._1);
            RegisterHotKey(handle, GetType().GetHashCode(), 2, (int)hotkeys._2);
            RegisterHotKey(handle, GetType().GetHashCode(), 2, (int)hotkeys._3);
            RegisterHotKey(handle, GetType().GetHashCode(), 2, (int)hotkeys._4);
            RegisterHotKey(handle, GetType().GetHashCode(), 2, (int)hotkeys._5);
        }

        

        void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {

            if (msg.message == WM_HOTKEY)
            {
                int D = (((int)msg.lParam >> 16) & 0xFFFF);
                int F = (((int)msg.lParam >> 16) & 0xFFFF);
                hotkeys key = (hotkeys)F;
                ModifierKeys modifier = (ModifierKeys)((int)msg.lParam & 0xFFFF);

                if (key == hotkeys._1)
                    toolsWindow.clickThroughCheckBox.IsChecked = !toolsWindow.clickThroughCheckBox.IsChecked;
                else if (key == hotkeys._2)
                    toolsWindow.penButton_Click(new object(), new RoutedEventArgs());
                else if (key == hotkeys._3)
                    toolsWindow.highlighterButton_Click(new object(), new RoutedEventArgs());
                else if (key == hotkeys._4)
                    toolsWindow.eraserButton_Click(new object(), new RoutedEventArgs());
                else if (key == hotkeys._5)
                    toolsWindow.eraseAllButton_Click(new object(), new RoutedEventArgs());
            }
        }




        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);

        
        //if((IntPtr)
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        [DllImport("user32.dll")]
        public static extern int GetWindowLongPtr(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLongPtr(IntPtr hwnd, int index, int newStyle);

        IntPtr hwnd;

        int extendedStyle;
        protected override void OnSourceInitialized(EventArgs e)
        {
            is64Bit = (System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr))) == 8;
            base.OnSourceInitialized(e);
            hwnd = new WindowInteropHelper(this).Handle;
            if (is64Bit)
                extendedStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
            else
                extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        }
        
        ToolsWindow toolsWindow = new ToolsWindow();
        bool is64Bit;
        System.Windows.Forms.MenuItem rememberContent;
        string appDataDir;
            
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + AssemblyTitle;
            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);



            System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon(new System.ComponentModel.Container());

            notifyIcon.Icon = new System.Drawing.Icon(GetType(), "pencilIcon.ico");
            notifyIcon.Visible = true;
            //notify icon:
            rememberContent = new System.Windows.Forms.MenuItem("Remeber content when closed");
            rememberContent.Checked = false;
            rememberContent.Click += new EventHandler(rememberContent_Click);
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("Exit");
            exit.Click += new EventHandler(toolsWindow_CloseButtonClick);

            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] { rememberContent, exit });

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);

            loadSettings();



            int tempWidth = 0;
            int tempHeight = 0;
            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                tempWidth += screen.Bounds.Width;
                tempHeight += screen.Bounds.Height;
            }
#if DEBUG
            tempWidth = 300;
            tempHeight = 300;
#endif
            //debug
            this.Width = tempWidth;
            this.Height = tempHeight;
            this.Left = 0;
            this.Top = 0;


            if (System.IO.File.Exists(appDataDir + "\\content.ink"))
            {
                FileStream fS = new System.IO.FileStream(appDataDir + "\\content.ink", System.IO.FileMode.Open);
                inkCanvas.Strokes = new System.Windows.Ink.StrokeCollection(fS);
                fS.Close();
                System.IO.File.Delete(appDataDir + "\\content.ink");
            }

            inkCanvas.Cursor = Cursors.Pen;
            inkCanvas.UseCustomCursor = true;
            inkCanvas.DefaultDrawingAttributes.IgnorePressure = false;

            toolsWindow.setInkCanvas(inkCanvas);
            toolsWindow.Owner = this;
            toolsWindow.CloseButtonClick += new EventHandler(toolsWindow_CloseButtonClick);

            toolsWindow.clickThroughCheckBox.Checked += new RoutedEventHandler(clickThroughCheckBox_Checked);
            toolsWindow.clickThroughCheckBox.Unchecked += new RoutedEventHandler(clickThroughCheckBox_Checked);

            toolsWindow.Show();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveSettings();
            if (rememberContent.Checked)
            {

                FileStream fS = new System.IO.FileStream(appDataDir + "\\content.ink", System.IO.FileMode.Create);
                inkCanvas.Strokes.Save(fS);
                fS.Close();
            }
        }

        void saveSettings()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateDocumentType("Settings",null,null,null));
            xmlDoc.AppendChild(xmlDoc.CreateElement("Settings"));
            xmlDoc.LastChild.AppendChild(xmlDoc.CreateElement("RememberContent"));
            xmlDoc.LastChild.LastChild.Attributes.Append(xmlDoc.CreateAttribute("value"));
            xmlDoc.LastChild.LastChild.Attributes["value"].Value = rememberContent.Checked.ToString();
            xmlDoc.Save(appDataDir + "\\settings.xml");
        }


        void loadSettings()
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (File.Exists(appDataDir + "\\settings.xml"))
            {
                xmlDoc.Load(appDataDir + "\\settings.xml");
                rememberContent.Checked = bool.Parse(xmlDoc.LastChild.LastChild.Attributes["value"].Value);
            }
        }

        void rememberContent_Click(object sender, EventArgs e)
        {
            rememberContent.Checked = !rememberContent.Checked;
        }

        void toolsWindow_CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        void clickThroughCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)toolsWindow.clickThroughCheckBox.IsChecked == true)
            {
                if(is64Bit)
                SetWindowLongPtr(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
                else
                    SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
                Background = null;
            }
            else
            {
                if (is64Bit)
                SetWindowLongPtr(hwnd, GWL_EXSTYLE, extendedStyle | 0);
                else
                    SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | 0);
                Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
            }
        }
    }
}
