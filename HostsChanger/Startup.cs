#region License
/*
Copyright © Malte Meister 2011

This file is part of HostsChanger.

HostsChanger is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

HostsChanger is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with HostsChanger.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HostsChanger
{
    class Startup
    {
        [STAThread]
        static void Main()
        {
            try
            {
                //MessageBox.Show("Debug MsgBox");

                App app = new App();
                app.InitializeComponent();
                app.InitHostsChangerTrayIcon();
                app.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops.. Please send a screenshot to info@bloing.net\n\n" + ex.ToString(), "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
