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
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace HostsChanger
{
    internal class HostsFile : INotifyPropertyChanged
    {
        private const string RegionHeader = "#hc HostsChanger Region - Do not make changes beyond this point!";

        private FileInfo fileInfo;
        private int regionStartIndex = -1;
        private static readonly string[] defaultPaths = new string[] { 
            @"C:\Windows\System32\drivers\etc\hosts"
        };

        private ObservableCollection<Profile> profiles;
        public ObservableCollection<Profile> Profiles
        {
            get { return profiles; }
        }

        internal Profile ActiveProfile
        {
            get
            {
                return (from profile in profiles
                        where profile.Name.ToLower() != "default"
                        && profile.Enabled
                        select profile).FirstOrDefault();
            }
        }

        public HostsFile() : this(defaultPaths)  { }

        public HostsFile(string[] paths)
        {
            profiles = new ObservableCollection<Profile>();
            profiles.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(profiles_CollectionChanged);

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    fileInfo = new FileInfo(path);
                    LoadFile();
                    break;
                }                    
            }
        }

        void profiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Profiles");
        }

        private void LoadFile()
        {
            var text = File.ReadAllText(fileInfo.FullName);

            // Ensure hosts file has a region for HC to use
            regionStartIndex = text.IndexOf(RegionHeader);

            if (regionStartIndex >= 0)
            {
                LoadProfiles(text.Substring(regionStartIndex));
            }
            else
            {
                InitHostsFile();

                text = File.ReadAllText(fileInfo.FullName);
                regionStartIndex = text.IndexOf(RegionHeader);

                if (regionStartIndex < 0)
                {
                    throw new Exception("Failed to load the hosts file");
                }

                LoadProfiles(text.Substring(regionStartIndex));
            }
        }

        private void WriteFile()
        {
            try
            {
                var text = File.ReadAllText(fileInfo.FullName);
                var nonRegionContent = String.Empty;

                if (regionStartIndex == -1)
                {
                    // No HC region exists, preserve all file contents
                    nonRegionContent = text.Trim();
                }
                else if (regionStartIndex > 0)
                {
                    // HC region does not begin at the start of the file, preserve non-region content
                    nonRegionContent = text.Substring(0, regionStartIndex).Trim();
                }

                File.WriteAllText(fileInfo.FullName, nonRegionContent);

                using (var sw = File.AppendText(fileInfo.FullName))
                {
                    sw.WriteLine(sw.NewLine + sw.NewLine + RegionHeader);

                    foreach (var profile in Profiles)
                    {
                        sw.Write(profile.ToString());
                    }

                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to write HostsChanger region to the hosts file", ex);
            }
        }

        private void InitHostsFile()
        {
            Profiles.Add(new DefaultProfile());
            WriteFile();
        }

        private void LoadProfiles(string hcRegionText)
        {
            profiles.Clear();

            var profilesText = hcRegionText.Substring(hcRegionText.IndexOf(Profile.ProfileHeader));
            var lines = profilesText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Profile profile = new Profile();

            foreach (var line in lines)
            {
                if (line.StartsWith(Profile.ProfileHeader))
                {
                    var profileName = line.Substring(line.IndexOf("name=") + 5).Trim();
                    if (profileName.ToLower() == "default")
                    {
                        profile = new DefaultProfile();
                    }
                    else
                    {
                        profile = new Profile
                        {
                            Name = profileName
                        };
                    }
                }
                else if (line.StartsWith(Profile.ProfileFooter))
                {
                    Profiles.Add(profile);
                }
                else
                {
                    profile.Enabled = line.StartsWith("#") ? false : true;
                    var trimmedLined = line.TrimStart(new char[] { '#' });
                    var parts = trimmedLined.Split(new char[] { '\t', ' '}, StringSplitOptions.RemoveEmptyEntries);

                    profile.Entries.Add(new Entry
                    {
                        IpAddress = parts.Length > 0 ? parts[0] : null,
                        MachineName = parts.Length > 1 ? parts[1] : null,
                        FQDN = parts.Length > 2 ? parts[2] : null
                    });
                }
            }
        }

        /// <summary>
        /// Cycles through the non-default profiles
        /// a) If none are active, the first one is activated
        /// b) If the active profile is the last one in the list, all are deactivated (user needs to click again to activate 1st profile)
        /// c) If the active profile is NOT the last one in the list, the next profile is activated
        /// </summary>
        /// <returns>The now active profile, or null if no non-default profiles are active</returns>
        internal Profile CycleProfile()
        {
            var nonDefaultProfiles = (from p in Profiles
                                     where p.Name.ToLower() != "default"
                                     select p).ToList();
                        
            if (nonDefaultProfiles.Count == 0)
            {
                // There are no non-default profiles, do nothing
                return null;
            }

            if (ActiveProfile == null)
            {
                //a
                nonDefaultProfiles[0].Enabled = true;
                WriteFile();
                return nonDefaultProfiles[0];
            }

            var activeProfileIndex = nonDefaultProfiles.IndexOf(ActiveProfile);
            ActiveProfile.Enabled = false;

            if (activeProfileIndex == nonDefaultProfiles.Count - 1)
            {
                //b
                WriteFile();
                return null;
            }

            //c
            nonDefaultProfiles[activeProfileIndex + 1].Enabled = true;
            WriteFile();
            return nonDefaultProfiles[activeProfileIndex + 1];
        }

        internal void SetActiveProfile(Profile profile)
        {
            if (profile == null || profile.Name.ToLower() == "default")
            {
                // Enabled state cannot be changed for the default profile
                return;
            }

            if (profile.Enabled)
            {
                // Profile was enabled when clicked; disable
                profile.Enabled = false;
            }
            else
            {
                // Disable all other profiles (except default) and enable this one
                foreach (var prof in profiles)
                {
                    if (prof.Name.ToLower() != "default")
                    {
                        prof.Enabled = prof == profile ? true : false;
                    }
                }
            }

            WriteFile();
        }

        internal void SaveChanges()
        {
            WriteFile();
        }

        internal void DiscardChanges()
        {
            LoadFile();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion        
    }
}