﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using NHotkey.Wpf;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Timers;
using Timer = System.Timers.Timer;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Button = System.Windows.Forms.Button;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using Icon = System.Drawing.Icon;
using Rectangle = System.Drawing.Rectangle;
using System.Runtime.InteropServices;
using ContextMenu = System.Windows.Forms.ContextMenu;

namespace Color_Test_WPF_App_NET_Framework
{
    class TheTrayIcon{
        private MainWindow theWindow = new MainWindow();

        public TheTrayIcon() {

            theWindow.Show();

            NotifyIcon nIcon = new NotifyIcon();
            //the shortcut menu for notifyIcon
            ContextMenu contextMenu1 = new ContextMenu();

            //Global Shortcuts
            HotkeyManager.Current.AddOrReplace("toggleRealTimeGS", Key.L, ModifierKeys.Control | ModifierKeys.Shift, theWindow.toggleRealTimeGS);
            HotkeyManager.Current.AddOrReplace("screenshotGS", Key.M, ModifierKeys.Control | ModifierKeys.Shift, theWindow.screenshotGS);
            HotkeyManager.Current.AddOrReplace("switchTypesGS", Key.N, ModifierKeys.Control | ModifierKeys.Shift, theWindow.switchTypesGS);


            MenuItem mainWindow = new MenuItem("Color Oracle", (s, d) => openMainWindow(s, d));
            MenuItem toggleRealTime = new MenuItem("Live Mode", (s, d) => theWindow.toggleRealTimeGS(s, d)); toggleRealTime.Shortcut = Shortcut.CtrlShiftL;
            MenuItem screenshot = new MenuItem("Screenshot", (s, d) => theWindow.screenshotGS(s, d));        screenshot.Shortcut = Shortcut.CtrlShiftM;
            MenuItem toggleMethod = new MenuItem("Toggle Method", (s, d) => theWindow.switchTypesGS(s, d));  toggleMethod.Shortcut = Shortcut.CtrlShiftN;
            MenuItem exit = new MenuItem("Exit", (s, d) => this.close(s, d));

            contextMenu1.MenuItems.Add(mainWindow);
            contextMenu1.MenuItems.Add(toggleRealTime);
            contextMenu1.MenuItems.Add(screenshot);
            contextMenu1.MenuItems.Add(toggleMethod);
            contextMenu1.MenuItems.Add(exit);


            nIcon.ContextMenu = contextMenu1;
            nIcon.Icon = new Icon("../../images/trayicon.ico");
            nIcon.Visible = true;
            nIcon.Text = "Color Oracle";
        }


        private void close(object sender, EventArgs e){
            System.Windows.Application.Current.Shutdown();
        }

        private void openMainWindow(object sender, EventArgs e) { this.theWindow.Show(); }

    }
}