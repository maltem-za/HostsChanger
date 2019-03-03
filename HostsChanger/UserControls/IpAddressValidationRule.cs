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
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace HostsChanger
{
    internal class IpAddressValidationRule : ValidationRule
    {
        private const string validationRegex = "^([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])$";

        public string ErrorMessage { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);

            string inputString = (value ?? string.Empty).ToString();
            
            if (!Regex.IsMatch(inputString, validationRegex))
            {
                result = new ValidationResult(false, ErrorMessage);
            }

            return result;
        }
    }
}
