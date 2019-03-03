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

namespace HostsChanger
{
    internal class Entry : INotifyPropertyChanged
    {
        private string ipAddress;
        public string IpAddress
        {
            get { return ipAddress; }
            set
            {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    NotifyPropertyChanged("IpAddress");
                }
            }
        }

        private string machineName;
        public string MachineName
        {
            get { return machineName; }
            set
            {
                if (machineName != value)
                {
                    machineName = value;
                    NotifyPropertyChanged("MachineName");
                }
            }
        }

        private string fqdn;
        public string FQDN
        {
            get { return fqdn; }
            set
            {
                if (fqdn != value)
                {
                    fqdn = value;
                    NotifyPropertyChanged("FQDN");
                }
            }
        }

        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}", ipAddress, machineName, fqdn);
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
