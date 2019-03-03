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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HostsChanger
{
    /// <summary>
    /// Interaction logic for EntryEditorUserControl.xaml
    /// </summary>
    public partial class ProfileEditorUserControl : UserControl
    {
        public ProfileEditorUserControl()
        {
            InitializeComponent();
        }

        private void btnAddEntry_Click(object sender, RoutedEventArgs e)
        {
            var profile = (Profile)this.DataContext;

            if (profile != null)
            {
                profile.Entries.Add(new Entry
                {
                    IpAddress = "127.0.0.1",
                    MachineName = "localhost",
                    FQDN = "new.localhost"
                });
            }
        }

        private void btnDelEntry_Click(object sender, RoutedEventArgs e)
        {
            var profile = (Profile)this.DataContext;
            var targetEntry = (Entry)((Button)sender).DataContext;

            profile.Entries.Remove(targetEntry);
        }

        private void textBox_SelectAllOnFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }
    }
}
