# HostsChanger
HostsChanger is a free system tray application for Windows which makes it easy to change or edit your hosts file on the fly.

## Download
[Setup Executable](https://github.com/maltem-za/HostsChanger/raw/master/hc-setup.exe)

## Requirements
.NET Framework v3.5 SP1 or later

This software should work anywhere you can install the .NET Framework, however it has only been tested on Windows 7 and Windows Server 2008.

## License
[GPLv3](http://www.gnu.org/licenses/gpl.html)

## Screenshots
![Main Application Screenshot](https://github.com/maltem-za/HostsChanger/blob/master/HostsChanger/screenshots/main.png)
![Settings Screenshot](https://github.com/maltem-za/HostsChanger/blob/master/HostsChanger/screenshots/settings.png)

## Additional Information
HostsChanger is basically comprised of some WPF User Controls built on top of an old, slightly modified version of Phillip Sumi’s WPF NotifyIcon [[CodeProject](https://www.codeproject.com/Articles/36468/WPF-NotifyIcon-2) | [GitHub](https://github.com/hardcodet/wpf-notifyicon)]. The source code for HostsChanger is available under the GNU GPL, while WPF NotifiyIcon is released under the CPOL License. The modified WPF NotifyIcon source is included in this repository as a [ZIP file](https://github.com/maltem-za/HostsChanger/raw/master/wpf-notifyicon.zip) (I first published the application on a personal blog in December 2011).

## FAQ
###### Q: How do the profiles work?
A: Except for the default profile, any profiles that you create are basically mutually exclusive sets of entries for your hosts file – only one of which can be active. This is useful, for example, when you have multiple virtual environment clones running in a development environment. The default profile should contain hosts entries that must always be active and can not be turned off.

###### Q: Why do I get a UAC prompt?
A: In newer versions of Windows (Vista/7/Server 2008)┬á you are required to have administrator privileges to edit the hosts file.

###### Q: Can I add my own icons for profiles?
A: Yes. Simply create a pair of 16×16 png and ico files and name them as follows: HostsChanger-<yourProfileName>.<ext>
Place the files in the Icons subfolder of your HostsChanger installation directory.
