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
using Hardcodet.Wpf.TaskbarNotification;

namespace HostsChanger
{
    /// <summary>
    /// Interaction logic for HostsChangerUserControl.xaml
    /// </summary>
    public partial class HostsChangerUserControl : UserControl
    {
        private HostsFile hostsFile;

        private TaskbarIcon TaskbarIcon
        {
            get { return (TaskbarIcon)App.Current.Properties["TaskbarIcon"]; }
        }

        // Command binding done in XAML
        public static RoutedCommand LeftClickCommand = new RoutedCommand("Left Click", typeof(HostsChangerUserControl));

        public HostsChangerUserControl()
        {
            InitializeComponent();

            hostsFile = new HostsFile();
            this.DataContext = hostsFile;
        }

        #region Click Handlers

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var profile = (Profile)button.DataContext;

            hostsFile.SetActiveProfile(profile);
            UpdateTrayIcon();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TaskbarIcon.HideTrayPopup();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            if (pnlProfiles.Visibility == Visibility.Collapsed)
            {
                HideOptionsPanel(true);
            }
            else
            {
                ShowOptionsPanel();
            }
        }

        private void btnCancelOptions_Click(object sender, RoutedEventArgs e)
        {
            var optionsButtonContent = (StackPanel)btnOptions.Content;
            var optionsButtonTextBlock = (TextBlock)optionsButtonContent.Children[1];

            HideOptionsPanel(false);
        }

        private void btnAddProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new Profile
            {
                Name = "new profile"
            };

            hostsFile.Profiles.Add(profile);
            profilesList.SelectedItem = profile;
        }

        private void btnDelProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = profilesList.SelectedItem as Profile;

            if (profile != null && profile.Name.ToLower() != "default")
            {
                var message = String.Format(
                    "Are you sure you want to delete the profile \"{0}\"?\nThis profile has {1} entries.",
                    profile.Name,
                    profile.Entries.Count);

                ShowConfirmationPanel(message);
            }
        }

        // Confirmation panel
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var profile = (Profile)profilesList.SelectedItem;
            hostsFile.Profiles.Remove(profile);
            HideConfirmationPanel();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            HideConfirmationPanel();
        }

        // Tray icon left click
        private void ExecutedLeftClick(object sender, ExecutedRoutedEventArgs e)
        {
            if ((bool)Properties.Settings.Default["LeftClickCyclesProfiles"])
            {
                hostsFile.CycleProfile();
                UpdateTrayIcon();
            }
        }

        private void CanExecuteLeftClick(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        private void ShowConfirmationPanel(string message)
        {
            pnlOptions.Visibility = Visibility.Collapsed;
            btnOptions.IsEnabled = false;
            txtConfirmationMessage.Text = message;
            pnlConfirmation.Visibility = Visibility.Visible;
        }

        private void HideConfirmationPanel()
        {
            pnlConfirmation.Visibility = Visibility.Collapsed;
            btnOptions.IsEnabled = true;
            pnlOptions.Visibility = Visibility.Visible;
        }

        private void ShowOptionsPanel()
        {
            var optionsButtonContent = (StackPanel)btnOptions.Content;
            var optionsButtonTextBlock = (TextBlock)optionsButtonContent.Children[1];

            pnlProfiles.Visibility = Visibility.Collapsed;
            btnClose.Visibility = Visibility.Collapsed;

            pnlOptions.Visibility = Visibility.Visible;
            btnCancelOptions.Visibility = Visibility.Visible;

            btnOptions.Style = (Style)FindResource("OrangeButton");
            optionsButtonTextBlock.Text = "Close & Save!";
        }

        private void HideOptionsPanel(bool saveOptions)
        {
            var optionsButtonContent = (StackPanel)btnOptions.Content;
            var optionsButtonTextBlock = (TextBlock)optionsButtonContent.Children[1];

            pnlOptions.Visibility = Visibility.Collapsed;
            btnCancelOptions.Visibility = Visibility.Collapsed;

            pnlProfiles.Visibility = Visibility.Visible;
            btnClose.Visibility = Visibility.Visible;

            btnOptions.Style = (Style)FindResource("NormalButton");
            optionsButtonTextBlock.Text = "Options";

            if (saveOptions)
            {
                hostsFile.SaveChanges();
                Properties.Settings.Default.Save();
                SetTrayIconPopupActivationMode();
            }
            else
            {
                hostsFile.DiscardChanges();
                Properties.Settings.Default.Reload();
            }
        }

        public void UpdateTrayIcon()
        {
            var profile = hostsFile.ActiveProfile;
            var imgSourceValueConverter = new ImageSourceValueConverter(true);
            ImageSource imgSource;

            if (profile == null || !profile.Enabled)
            {
                // Use default icon
                imgSource = (ImageSource)imgSourceValueConverter.Convert("default", null, null, null);
            }
            else
            {
                // Find profile icon/image
                imgSource = (ImageSource)imgSourceValueConverter.Convert(profile.Name, null, null, null);
            }

            if (TaskbarIcon != null)
            {
                // Apply image
                TaskbarIcon.IconSource = imgSource;
                SetTrayIconToolTipText();
            }
        }

        public void SetTrayIconPopupActivationMode()
        {
            if (TaskbarIcon != null)
            {
                if ((bool)Properties.Settings.Default["LeftClickCyclesProfiles"])
                {
                    TaskbarIcon.PopupActivation = PopupActivationMode.RightClick;
                }
                else
                {
                    TaskbarIcon.PopupActivation = PopupActivationMode.LeftOrRightClick;
                }
                SetTrayIconToolTipText();
            }
        }

        private void SetTrayIconToolTipText()
        {
            var profileString = String.Format("[Profile: {0}]", hostsFile.ActiveProfile == null ? "None" : hostsFile.ActiveProfile.Name);

            if ((bool)Properties.Settings.Default["LeftClickCyclesProfiles"])
            {
                TaskbarIcon.ToolTipText = profileString + " Right-click for options, left-click to cycle profiles";
            }
            else
            {
                TaskbarIcon.ToolTipText = profileString + " Click for options";
            }
        }
    }
}
