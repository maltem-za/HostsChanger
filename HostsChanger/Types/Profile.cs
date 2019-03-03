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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace HostsChanger
{
    internal class Profile : INotifyPropertyChanged
    {
        public const string ProfileHeader = "#hc-profile";
        public const string ProfileFooter = "#hc-endprofile";

        private string name;
        public virtual string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private bool enabled;
        public virtual bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    NotifyPropertyChanged("Enabled");
                }
            }
        }

        private ObservableCollection<Entry> entries;
        public ObservableCollection<Entry> Entries
        {
            get { return entries; }
        }

        public Profile()
        {
            entries = new ObservableCollection<Entry>();
            entries.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(entries_CollectionChanged);
        }

        void entries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Entries");
        }

        public override string ToString()
        {
            //Header
            var sb = new StringBuilder(ProfileHeader);
            sb.AppendLine(String.Format(" name={0}", Name));

            //Entries
            var lineStart = Enabled ? String.Empty : "#";            
            foreach (var entry in Entries)
            {
                sb.AppendLine(String.Format("{0}{1}", lineStart, entry.ToString()));
            }

            //Footer
            sb.AppendLine(ProfileFooter);

            return sb.ToString();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
