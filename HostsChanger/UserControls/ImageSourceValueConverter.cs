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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace HostsChanger
{
    /// <summary>
    /// Responsible for the automatic conversion that happens when an image's source property
    /// is bound to a string property (profile name)
    /// </summary>
    [ValueConversion(typeof(String), typeof(ImageSource))]
    public class ImageSourceValueConverter : IValueConverter
    {
        private const string _pack = "pack://application:,,,/";

        private string defaultImage;
        private string[] iconFiles;
        private string fileExtension;

        public ImageSourceValueConverter() : this(false) { }

        /// <summary>
        /// This class is also used to fetch the tray icon .ico file when the profile changes
        /// The tray icon ONLY supports .ico files, hence this overload
        /// </summary>
        /// <param name="ico">Whether to look for .ico files instead of .png files</param>
        public ImageSourceValueConverter(bool ico)
        {
            fileExtension = ico ? ".ico" : ".png";            
            var iconDirectory =  ResolveIconDirectory();
            defaultImage = iconDirectory + "HostsChanger-Unknown" + fileExtension;
            iconFiles = Directory.GetFiles(iconDirectory, "HostsChanger-*" + fileExtension, SearchOption.TopDirectoryOnly);           
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var profileName = value.ToString();

            // Find an image file for this profile if it exists, otherwise use the default
            var matchingImagePath = (from fileName in iconFiles
                                     where IsIconForProfile(fileName, profileName)
                                     select fileName).FirstOrDefault();

            if (matchingImagePath != null)
            {
                return new BitmapImage(new Uri(matchingImagePath));
            }

            return new BitmapImage(new Uri(defaultImage));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageSource = value as BitmapImage;

            if (imageSource != null)
            {
                return imageSource.UriSource.OriginalString;
            }

            return DependencyProperty.UnsetValue;
        }

        #endregion

        private bool IsIconForProfile(string fileName, string profileName)
        {
            var fileNameWithExtension = fileName.Substring(fileName.LastIndexOf('\\') + 1).Substring(13);

            return fileNameWithExtension.Equals(profileName + fileExtension, StringComparison.CurrentCultureIgnoreCase);
        }

        private string ResolveIconDirectory()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            if (currentDirectory.EndsWith("\\bin\\Debug") || currentDirectory.EndsWith("\\bin\\Release"))
            {
                var loc = currentDirectory;
                if (loc.IndexOf(@"\bin\Debug") > -1)
                    loc = loc.Remove(loc.IndexOf(@"\bin\Debug"));
                if (loc.IndexOf(@"\bin\Release") > -1)
                    loc = loc.Remove(loc.IndexOf(@"\bin\Release"));
                loc = (loc + @"\Icons\");

                if (Directory.Exists(loc))
                {
                    return loc;
                }
            }
            else
            {
                if (Directory.Exists(currentDirectory + @"\Icons\"))
                {
                    return currentDirectory + @"\Icons\";
                }
            }

            throw new DirectoryNotFoundException("Couldn't find Icons directory");
        }
    }
}
