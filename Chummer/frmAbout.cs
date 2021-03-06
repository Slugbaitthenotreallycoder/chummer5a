﻿/*  This file is part of Chummer5a.
 *
 *  Chummer5a is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Chummer5a is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Chummer5a.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  You can obtain the full source code for Chummer5a at
 *  https://github.com/chummer5a/chummer5a
 */
 using System;
using System.Reflection;
using System.Windows.Forms;

namespace Chummer
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        #region Assembly Attribute Accessors
        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (!string.IsNullOrEmpty(titleAttribute.Title))
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        #region Controls Methods
        private void frmAbout_Load(object sender, EventArgs e)
        {
            string strReturn = LanguageManager.GetString("Label_About", GlobalOptions.Language, false);
            if (string.IsNullOrEmpty(strReturn))
                strReturn = "About";
            Text = strReturn + ' ' + AssemblyTitle;
            labelProductName.Text = AssemblyProduct;
            strReturn = LanguageManager.GetString("String_Version", GlobalOptions.Language, false);
            if (string.IsNullOrEmpty(strReturn))
                strReturn = "Version";
            labelVersion.Text = strReturn + ' ' + AssemblyVersion;
            strReturn = LanguageManager.GetString("About_Copyright_Text", GlobalOptions.Language, false);
            if (string.IsNullOrEmpty(strReturn))
                strReturn = AssemblyCopyright;
            labelCopyright.Text = strReturn;
            strReturn = LanguageManager.GetString("About_Company_Text", GlobalOptions.Language, false);
            if (string.IsNullOrEmpty(strReturn))
                strReturn = AssemblyCompany;
            labelCompanyName.Text = strReturn;
            strReturn = LanguageManager.GetString("About_Description_Text", GlobalOptions.Language, false);
            if (string.IsNullOrEmpty(strReturn))
                strReturn = AssemblyDescription;
            textBoxDescription.Text = strReturn;
            textBoxContributors.Text += "\n\r\n\r\n\r" + string.Join("\n\r\n\r", Properties.Contributors.Usernames) + "\n\r\n\r/u/Iridios";
            txtDisclaimer.Text = LanguageManager.GetString("About_Label_Disclaimer_Text", GlobalOptions.Language);
        }

        private void cmdDonate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=LG855DVUT8FDU");
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.OK;

            if (e.Control && e.KeyCode == Keys.A)
            {
                e.SuppressKeyPress = true;
                (sender as TextBox)?.SelectAll();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void tableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        #endregion
    }
}
