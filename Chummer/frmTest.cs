/*  This file is part of Chummer5a.
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
using System.Collections.Generic;
 using System.Linq;
 using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
 using Chummer.Backend.Equipment;
 using Chummer.Backend.Skills;

namespace Chummer
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
        }

        private void cmdTest_Click(object sender, EventArgs e)
        {
            txtOutput.Text = string.Empty;
            switch (cboTest.Text)
            {
                case "armor.xml":
                    TestArmor();
                    break;
                case "bioware.xml":
                case "cyberware.xml":
                    TestCyberware(cboTest.Text);
                    break;
                case "critters.xml":
                    TestMetatype("critters.xml");
                    break;
                case "gear.xml":
                    TestGear();
                    break;
                case "metatypes.xml":
                    TestMetatype("metatypes.xml");
                    break;
                case "qualities.xml":
                    TestQuality();
                    break;
                case "vehicles.xml":
                    TestVehicles();
                    break;
                case "weapons.xml":
                    TestWeapons();
                    break;
            }
            txtOutput.Text += "Done validation";
        }

        private void TestVehicles()
        {
            Character objCharacter = new Character();
            XmlDocument objXmlDocument = XmlManager.Load("vehicles.xml");
            pgbProgress.Minimum = 0;
            pgbProgress.Value = 0;
            pgbProgress.Maximum = objXmlDocument.SelectNodes("/chummer/vehicles/vehicle").Count;
            pgbProgress.Maximum += objXmlDocument.SelectNodes("/chummer/mods/mod").Count;

            // Vehicles.
            foreach (XmlNode objXmlGear in objXmlDocument.SelectNodes("/chummer/vehicles/vehicle"))
            {
                pgbProgress.Value++;
                Application.DoEvents();
                try
                {
                    Vehicle objTemp = new Vehicle(objCharacter);
                    objTemp.Create(objXmlGear);
                    try
                    {
                        decimal objValue = objTemp.TotalCost;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalCost\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalAccel;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalAccel\r\n";
                    }
                    try
                    {
                        int objValue = objTemp.TotalArmor;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalArmor\r\n";
                    }
                    try
                    {
                        int objValue = objTemp.TotalBody;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalBody\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalHandling;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalHandling\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalSpeed;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalSpeed\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalAvail(GlobalOptions.CultureInfo, GlobalOptions.Language);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedAvail\r\n";
                    }
                }
                catch
                {
                    txtOutput.Text += objXmlGear["name"].InnerText + " general failure\r\n";
                }
            }

            // Vehicle Mods.
            foreach (XmlNode objXmlGear in objXmlDocument.SelectNodes("/chummer/mods/mod"))
            {
                pgbProgress.Value++;
                Application.DoEvents();
                try
                {
                    VehicleMod objTemp = new VehicleMod(objCharacter);
                    objTemp.Create(objXmlGear, 1, null);
                    try
                    {
                        decimal objValue = objTemp.TotalCost;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalCost\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalAvail(GlobalOptions.CultureInfo, GlobalOptions.DefaultLanguage);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalAvail\r\n";
                    }
                    try
                    {
                        int objValue = objTemp.CalculatedSlots;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedSlots\r\n";
                    }
                }
                catch
                {
                    txtOutput.Text += objXmlGear["name"].InnerText + " general failure\r\n";
                }
            }

            objCharacter.DeleteCharacter();
            objCharacter = null;
        }

        private void TestWeapons()
        {
            Character objCharacter = new Character();
            XmlDocument objXmlDocument = XmlManager.Load("weapons.xml");
            pgbProgress.Minimum = 0;
            pgbProgress.Value = 0;
            pgbProgress.Maximum = objXmlDocument.SelectNodes("/chummer/weapons/weapon").Count;
            pgbProgress.Maximum += objXmlDocument.SelectNodes("/chummer/accessories/accessory").Count;
            pgbProgress.Maximum += objXmlDocument.SelectNodes("/chummer/mods/mod").Count;

            // Weapons.
            foreach (XmlNode objXmlGear in objXmlDocument.SelectNodes("/chummer/weapons/weapon"))
            {
                pgbProgress.Value++;
                Application.DoEvents();
                try
                {
                    Weapon objTemp = new Weapon(objCharacter);
                    objTemp.Create(objXmlGear, null);
                    try
                    {
                        decimal objValue = objTemp.TotalCost;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalCost\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalAP(GlobalOptions.Language);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalAP\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalAvail(GlobalOptions.CultureInfo, GlobalOptions.Language);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalAvail\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalRC;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalRC\r\n";
                    }
                    try
                    {
                        int objValue = objTemp.TotalReach;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalReach\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.CalculatedAmmo(GlobalOptions.CultureInfo, GlobalOptions.Language);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedAmmo\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.CalculatedConcealability(GlobalOptions.CultureInfo);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedConcealability\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.CalculatedDamage(GlobalOptions.CultureInfo, GlobalOptions.Language);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedDamage\r\n";
                    }
                }
                catch
                {
                    txtOutput.Text += objXmlGear["name"].InnerText + " general failure\r\n";
                }
            }

            // Weapon Accessories.
            foreach (XmlNode objXmlGear in objXmlDocument.SelectNodes("/chummer/accessories/accessory"))
            {
                pgbProgress.Value++;
                Application.DoEvents();
                try
                {
                    WeaponAccessory objTemp = new WeaponAccessory(objCharacter);
                    objTemp.Create(objXmlGear, new Tuple<string, string>(string.Empty , string.Empty), 0);
                    try
                    {
                        decimal objValue = objTemp.TotalCost;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedCost\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalAvail(GlobalOptions.CultureInfo, GlobalOptions.Language);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalAvail\r\n";
                    }
                }
                catch
                {
                    txtOutput.Text += objXmlGear["name"].InnerText + " general failure\r\n";
                }
            }

            objCharacter.DeleteCharacter();
            objCharacter = null;
        }

        private void TestArmor()
        {
            Character objCharacter = new Character();
            XmlDocument objXmlDocument = XmlManager.Load("armor.xml");
            pgbProgress.Minimum = 0;
            pgbProgress.Value = 0;
            pgbProgress.Maximum = objXmlDocument.SelectNodes("/chummer/armors/armor").Count;
            pgbProgress.Maximum += objXmlDocument.SelectNodes("/chummer/mods/mod").Count;

            // Armor.
            foreach (XmlNode objXmlGear in objXmlDocument.SelectNodes("/chummer/armors/armor"))
            {
                pgbProgress.Value++;
                Application.DoEvents();
                try
                {
                    Armor objTemp = new Armor(objCharacter);
                    List<Weapon> lstWeapons = new List<Weapon>();
                    objTemp.Create(objXmlGear, 0, lstWeapons);
                    try
                    {
                        decimal objValue = objTemp.TotalCost;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalCost\r\n";
                    }
                    try
                    {
                        int objValue = objTemp.TotalArmor;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalArmor\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalAvail(GlobalOptions.CultureInfo, GlobalOptions.Language);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalAvail\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.CalculatedCapacity;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedCapacity\r\n";
                    }
                }
                catch
                {
                    txtOutput.Text += objXmlGear["name"].InnerText + " general failure\r\n";
                }
            }

            // Armor Mods.
            foreach (XmlNode objXmlGear in objXmlDocument.SelectNodes("/chummer/mods/mod"))
            {
                pgbProgress.Value++;
                Application.DoEvents();
                try
                {
                    ArmorMod objTemp = new ArmorMod(objCharacter);
                    List<Weapon> lstWeapons = new List<Weapon>();
                    objTemp.Create(objXmlGear, 1, lstWeapons);
                    try
                    {
                        string objValue = objTemp.TotalAvail(GlobalOptions.CultureInfo, GlobalOptions.Language);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalAvail\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.CalculatedCapacity;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedCapacity\r\n";
                    }
                }
                catch
                {
                    txtOutput.Text += objXmlGear["name"].InnerText + " general failure\r\n";
                }
            }
        }

        private void TestGear()
        {
            Character objCharacter = new Character();
            XmlDocument objXmlDocument = XmlManager.Load("gear.xml");
            pgbProgress.Minimum = 0;
            pgbProgress.Value = 0;
            pgbProgress.Maximum = objXmlDocument.SelectNodes("/chummer/gears/gear").Count;

            // Gear.
            foreach (XmlNode objXmlGear in objXmlDocument.SelectNodes("/chummer/gears/gear"))
            {
                pgbProgress.Value++;
                Application.DoEvents();
                try
                {
                    Gear objTemp = new Gear(objCharacter);
                    List<Weapon> lstWeapons = new List<Weapon>();
                    objTemp.Create(objXmlGear, 1, lstWeapons);
                    try
                    {
                        decimal objValue = objTemp.TotalCost;
                    }
                    catch
                    {
                        if (objXmlGear["category"].InnerText != "Mook")
                            txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalCost\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalAvail(GlobalOptions.CultureInfo, GlobalOptions.Language);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalAvail\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.CalculatedArmorCapacity;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedArmorCapacity\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.CalculatedCapacity;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedCapacity\r\n";
                    }
                    try
                    {
                        decimal objValue = objTemp.CalculatedCost;
                    }
                    catch
                    {
                        if (objXmlGear["category"].InnerText != "Mook")
                            txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedCost\r\n";
                    }
                }
                catch
                {
                    txtOutput.Text += objXmlGear["name"].InnerText + " general failure\r\n";
                }
            }

            objCharacter.DeleteCharacter();
            objCharacter = null;
        }

        private void TestCyberware(string strFile)
        {
            string strPrefix = "cyberware";
            Improvement.ImprovementSource objSource = Improvement.ImprovementSource.Cyberware;
            if (strFile == "bioware.xml")
            {
                strPrefix = "bioware";
                objSource = Improvement.ImprovementSource.Bioware;
            }

            Character objCharacter = new Character();
            XmlDocument objXmlDocument = XmlManager.Load(strFile);
            pgbProgress.Minimum = 0;
            pgbProgress.Value = 0;
            pgbProgress.Maximum = objXmlDocument.SelectNodes("/chummer/" + strPrefix + "s/" + strPrefix).Count;

            Grade objTestGrade = objCharacter.GetGradeList(objSource).FirstOrDefault(x => x.Name == "Standard");

            // Gear.
            foreach (XmlNode objXmlGear in objXmlDocument.SelectNodes("/chummer/" + strPrefix + "s/" + strPrefix))
            {
                pgbProgress.Value++;
                Application.DoEvents();
                try
                {
                    Cyberware objTemp = new Cyberware(objCharacter);
                    List<Weapon> lstWeapons = new List<Weapon>();
                    List<Vehicle> objVehicles = new List<Vehicle>();
                    objTemp.Create(objXmlGear, objCharacter, objTestGrade, objSource, 1, lstWeapons, objVehicles);
                    try
                    {
                        decimal objValue = objTemp.TotalCost;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalCost\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.TotalAvail(GlobalOptions.CultureInfo, GlobalOptions.Language);
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalAvail\r\n";
                    }
                    try
                    {
                        int objValue = objTemp.TotalAgility;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalAgility\r\n";
                    }
                    try
                    {
                        int objValue = objTemp.TotalBody;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalBody\r\n";
                    }
                    try
                    {
                        int objValue = objTemp.TotalStrength;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed TotalStrength\r\n";
                    }
                    try
                    {
                        string objValue = objTemp.CalculatedCapacity;
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedCapacity\r\n";
                    }
                    try
                    {
                        decimal objValue = objTemp.CalculatedESS();
                    }
                    catch
                    {
                        txtOutput.Text += objXmlGear["name"].InnerText + " failed CalculatedESS()\r\n";
                    }
                }
                catch
                {
                    txtOutput.Text += objXmlGear["name"].InnerText + " general failure\r\n";
                }
            }

            objCharacter.DeleteCharacter();
            objCharacter = null;
        }

        private void TestQuality()
        {
            Character objCharacter = new Character();
            XmlDocument objXmlDocument = XmlManager.Load("qualities.xml");
            pgbProgress.Minimum = 0;
            pgbProgress.Value = 0;
            pgbProgress.Maximum = objXmlDocument.SelectNodes("/chummer/qualities/quality").Count;

            // Qualities.
            foreach (XmlNode objXmlGear in objXmlDocument.SelectNodes("/chummer/qualities/quality"))
            {
                pgbProgress.Value++;
                Application.DoEvents();
                try
                {
                    List<Weapon> lstWeapons = new List<Weapon>();
                    Quality objTemp = new Quality(objCharacter);
                    objTemp.Create(objXmlGear, QualitySource.Selected, lstWeapons);
                }
                catch
                {
                    txtOutput.Text += objXmlGear["name"].InnerText + " general failure\r\n";
                }
            }
        }

        void TestMetatype(string strFile)
        {
            XmlDocument objXmlDocument = XmlManager.Load(strFile);

            pgbProgress.Minimum = 0;
            pgbProgress.Value = 0;
            pgbProgress.Maximum = objXmlDocument.SelectNodes("/chummer/metatypes/metatype").Count;

            foreach (XmlNode objXmlMetatype in objXmlDocument.SelectNodes("/chummer/metatypes/metatype"))
            {
                pgbProgress.Value++;
                Application.DoEvents();

                objXmlDocument = XmlManager.Load(strFile);
                Character objCharacter = new Character();

                try
                {
                    int intForce = 0;
                    if (objXmlMetatype["forcecreature"] != null)
                        intForce = 1;

                    // Set Metatype information.
                    if (strFile != "critters.xml" || objXmlMetatype["name"].InnerText == "Ally Spirit")
                    {
                        objCharacter.BOD.AssignLimits(ExpressionToString(objXmlMetatype["bodmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["bodmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["bodaug"].InnerText, intForce, 0));
                        objCharacter.AGI.AssignLimits(ExpressionToString(objXmlMetatype["agimin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["agimax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["agiaug"].InnerText, intForce, 0));
                        objCharacter.REA.AssignLimits(ExpressionToString(objXmlMetatype["reamin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["reamax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["reaaug"].InnerText, intForce, 0));
                        objCharacter.STR.AssignLimits(ExpressionToString(objXmlMetatype["strmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["strmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["straug"].InnerText, intForce, 0));
                        objCharacter.CHA.AssignLimits(ExpressionToString(objXmlMetatype["chamin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["chamax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["chaaug"].InnerText, intForce, 0));
                        objCharacter.INT.AssignLimits(ExpressionToString(objXmlMetatype["intmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["intmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["intaug"].InnerText, intForce, 0));
                        objCharacter.LOG.AssignLimits(ExpressionToString(objXmlMetatype["logmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["logmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["logaug"].InnerText, intForce, 0));
                        objCharacter.WIL.AssignLimits(ExpressionToString(objXmlMetatype["wilmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["wilmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["wilaug"].InnerText, intForce, 0));
                        objCharacter.MAG.AssignLimits(ExpressionToString(objXmlMetatype["magmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["magmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["magaug"].InnerText, intForce, 0));
                        objCharacter.MAGAdept.AssignLimits(ExpressionToString(objXmlMetatype["magmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["magmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["magaug"].InnerText, intForce, 0));
                        objCharacter.RES.AssignLimits(ExpressionToString(objXmlMetatype["resmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["resmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["resaug"].InnerText, intForce, 0));
                        objCharacter.EDG.AssignLimits(ExpressionToString(objXmlMetatype["edgmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["edgmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["edgaug"].InnerText, intForce, 0));
                        objCharacter.ESS.AssignLimits(ExpressionToString(objXmlMetatype["essmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["essmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["essaug"].InnerText, intForce, 0));
                        objCharacter.DEP.AssignLimits(ExpressionToString(objXmlMetatype["depmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["depmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["depaug"].InnerText, intForce, 0));
                    }
                    else
                    {
                        int intMinModifier = -3;
                        objCharacter.BOD.AssignLimits(ExpressionToString(objXmlMetatype["bodmin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["bodmin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["bodmin"].InnerText, intForce, 3));
                        objCharacter.AGI.AssignLimits(ExpressionToString(objXmlMetatype["agimin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["agimin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["agimin"].InnerText, intForce, 3));
                        objCharacter.REA.AssignLimits(ExpressionToString(objXmlMetatype["reamin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["reamin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["reamin"].InnerText, intForce, 3));
                        objCharacter.STR.AssignLimits(ExpressionToString(objXmlMetatype["strmin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["strmin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["strmin"].InnerText, intForce, 3));
                        objCharacter.CHA.AssignLimits(ExpressionToString(objXmlMetatype["chamin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["chamin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["chamin"].InnerText, intForce, 3));
                        objCharacter.INT.AssignLimits(ExpressionToString(objXmlMetatype["intmin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["intmin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["intmin"].InnerText, intForce, 3));
                        objCharacter.LOG.AssignLimits(ExpressionToString(objXmlMetatype["logmin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["logmin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["logmin"].InnerText, intForce, 3));
                        objCharacter.WIL.AssignLimits(ExpressionToString(objXmlMetatype["wilmin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["wilmin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["wilmin"].InnerText, intForce, 3));
                        objCharacter.MAG.AssignLimits(ExpressionToString(objXmlMetatype["magmin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["magmin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["magmin"].InnerText, intForce, 3));
                        objCharacter.MAGAdept.AssignLimits(ExpressionToString(objXmlMetatype["magmin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["magmin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["magmin"].InnerText, intForce, 3));
                        objCharacter.RES.AssignLimits(ExpressionToString(objXmlMetatype["resmin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["resmin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["resmin"].InnerText, intForce, 3));
                        objCharacter.EDG.AssignLimits(ExpressionToString(objXmlMetatype["edgmin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["edgmin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["edgmin"].InnerText, intForce, 3));
                        objCharacter.ESS.AssignLimits(ExpressionToString(objXmlMetatype["essmin"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["essmax"].InnerText, intForce, 0), ExpressionToString(objXmlMetatype["essaug"].InnerText, intForce, 0));
                        objCharacter.DEP.AssignLimits(ExpressionToString(objXmlMetatype["depmin"].InnerText, intForce, intMinModifier), ExpressionToString(objXmlMetatype["depmin"].InnerText, intForce, 3), ExpressionToString(objXmlMetatype["depmin"].InnerText, intForce, 3));
                    }

                    /* If we're working with a Critter, set the Attributes to their default values.
                    if (strFile == "critters.xml")
                    {
                        _objCharacter.BOD.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["bodmin"].InnerText, intForce, 0));
                        _objCharacter.AGI.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["agimin"].InnerText, intForce, 0));
                        _objCharacter.REA.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["reamin"].InnerText, intForce, 0));
                        _objCharacter.STR.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["strmin"].InnerText, intForce, 0));
                        _objCharacter.CHA.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["chamin"].InnerText, intForce, 0));
                        _objCharacter.INT.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["intmin"].InnerText, intForce, 0));
                        _objCharacter.LOG.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["logmin"].InnerText, intForce, 0));
                        _objCharacter.WIL.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["wilmin"].InnerText, intForce, 0));
                        _objCharacter.MAG.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["magmin"].InnerText, intForce, 0));
                        _objCharacter.RES.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["resmin"].InnerText, intForce, 0));
                        _objCharacter.EDG.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["edgmin"].InnerText, intForce, 0));
                        _objCharacter.ESS.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["essmax"].InnerText, intForce, 0));
                        _objCharacter.DEP.Value = Convert.ToInt32(ExpressionToString(objXmlMetatype["depmin"].InnerText, intForce, 0));
                    }

                    // Sprites can never have Physical Attributes or WIL.
                    if (objXmlMetatype["name"].InnerText.EndsWith("Sprite"))
                    {
                        _objCharacter.BOD.AssignLimits("0", "0", "0");
                        _objCharacter.AGI.AssignLimits("0", "0", "0");
                        _objCharacter.REA.AssignLimits("0", "0", "0");
                        _objCharacter.STR.AssignLimits("0", "0", "0");
                    }

                    _objCharacter.Metatype = objXmlMetatype["name"].InnerText;
                    _objCharacter.MetatypeCategory = objXmlMetatype["category"].InnerText;
                    _objCharacter.Metavariant = string.Empty;
                    _objCharacter.MetatypeBP = 400;

                    if (objXmlMetatype["movement"] != null)
                        _objCharacter.Movement = objXmlMetatype["movement"].InnerText;
                    // Load the Qualities file.
                    XmlDocument objXmlQualityDocument = XmlManager.Load("qualities.xml");

                    // Determine if the Metatype has any bonuses.
                    if (objXmlMetatype.InnerXml.Contains("bonus"))
                        objImprovementManager.CreateImprovements(Improvement.ImprovementSource.Metatype, objXmlMetatype["name"].InnerText, objXmlMetatype.SelectSingleNode("bonus"), false, 1, objXmlMetatype["name"].InnerText);

                    // Create the Qualities that come with the Metatype.
                    foreach (XmlNode objXmlQualityItem in objXmlMetatype.SelectNodes("qualities/positive/quality"))
                    {
                        XmlNode objXmlQuality = objXmlQualityDocument.SelectSingleNode("/chummer/qualities/quality[name = \"" + objXmlQualityItem.InnerText + "\"]");
                        List<Weapon> lstWeapons = new List<Weapon>();
                        Quality objQuality = new Quality(_objCharacter);
                        string strForceValue = string.Empty;
                        if (objXmlQualityItem.Attributes["select"] != null)
                            strForceValue = objXmlQualityItem.Attributes["select"].InnerText;
                        QualitySource objSource = new QualitySource();
                        objSource = QualitySource.Metatype;
                        if (objXmlQualityItem.Attributes["removable"] != null)
                            objSource = QualitySource.MetatypeRemovable;
                        objQuality.Create(objXmlQuality, _objCharacter, objSource, lstWeapons, strForceValue);
                        _objCharacter.Qualities.Add(objQuality);
                    }
                    foreach (XmlNode objXmlQualityItem in objXmlMetatype.SelectNodes("qualities/negative/quality"))
                    {
                        XmlNode objXmlQuality = objXmlQualityDocument.SelectSingleNode("/chummer/qualities/quality[name = \"" + objXmlQualityItem.InnerText + "\"]");
                        List<Weapon> lstWeapons = new List<Weapon>();
                        Quality objQuality = new Quality(_objCharacter);
                        string strForceValue = string.Empty;
                        if (objXmlQualityItem.Attributes["select"] != null)
                            strForceValue = objXmlQualityItem.Attributes["select"].InnerText;
                        QualitySource objSource = new QualitySource();
                        objSource = QualitySource.Metatype;
                        if (objXmlQualityItem.Attributes["removable"] != null)
                            objSource = QualitySource.MetatypeRemovable;
                        objQuality.Create(objXmlQuality, _objCharacter, objSource, lstWeapons, strForceValue);
                        _objCharacter.Qualities.Add(objQuality);
                    }

                    /* Run through the character's Attributes one more time and make sure their value matches their minimum value.
                    if (strFile == "metatypes.xml")
                    {
                        _objCharacter.BOD.Value = _objCharacter.BOD.TotalMinimum;
                        _objCharacter.AGI.Value = _objCharacter.AGI.TotalMinimum;
                        _objCharacter.REA.Value = _objCharacter.REA.TotalMinimum;
                        _objCharacter.STR.Value = _objCharacter.STR.TotalMinimum;
                        _objCharacter.CHA.Value = _objCharacter.CHA.TotalMinimum;
                        _objCharacter.INT.Value = _objCharacter.INT.TotalMinimum;
                        _objCharacter.LOG.Value = _objCharacter.LOG.TotalMinimum;
                        _objCharacter.WIL.Value = _objCharacter.WIL.TotalMinimum;
                    }*/

                    // Add any Critter Powers the Metatype/Critter should have.
                    XmlNode objXmlCritter = objXmlDocument.SelectSingleNode("/chummer/metatypes/metatype[name = \"" + objCharacter.Metatype + "\"]");

                    objXmlDocument = XmlManager.Load("critterpowers.xml");
                    foreach (XmlNode objXmlPower in objXmlCritter.SelectNodes("powers/power"))
                    {
                        XmlNode objXmlCritterPower = objXmlDocument.SelectSingleNode("/chummer/powers/power[name = \"" + objXmlPower.InnerText + "\"]");
                        CritterPower objPower = new CritterPower(objCharacter);
                        string strForcedValue = objXmlPower.Attributes?["select"]?.InnerText ?? string.Empty;
                        int intRating = 0;

                        if (objXmlPower.Attributes["rating"] != null)
                            intRating = Convert.ToInt32(objXmlPower.Attributes["rating"].InnerText);

                        objPower.Create(objXmlCritterPower, intRating, strForcedValue);
                        objCharacter.CritterPowers.Add(objPower);
                    }

                    // Set the Skill Ratings for the Critter.
                    foreach (XmlNode objXmlSkill in objXmlCritter.SelectNodes("skills/skill"))
                    {
                        if (objXmlSkill.InnerText.Contains("Exotic"))
                        {
                            //Skill objExotic = new Skill(_objCharacter);
                            //objExotic.ExoticSkill = true;
                            //objExotic.Attribute = "AGI";
                            //if (objXmlSkill.Attributes["spec"] != null)
                            //{
                                //SkillSpecialization objSpec = new SkillSpecialization(objXmlSkill.Attributes["spec"].InnerText);
                                //objExotic.Specializations.Add(objSpec);
                            //}
                            //if (Convert.ToInt32(ExpressionToString(objXmlSkill.Attributes["rating"].InnerText, Convert.ToInt32(intForce), 0)) > 6)
                            //    objExotic.RatingMaximum = Convert.ToInt32(ExpressionToString(objXmlSkill.Attributes["rating"].InnerText, Convert.ToInt32(intForce), 0));
                            //objExotic.Rating = Convert.ToInt32(ExpressionToString(objXmlSkill.Attributes["rating"].InnerText, Convert.ToInt32(intForce), 0));
                            //objExotic.Name = objXmlSkill.InnerText;
                            //_objCharacter.Skills.Add(objExotic);
                        }
                        else
                        {
                            foreach (Skill objSkill in objCharacter.SkillsSection.Skills)
                            {
                                if (objSkill.Name == objXmlSkill.InnerText)
                                {
                                    if (objXmlSkill.Attributes["spec"] != null)
                                    {
                                        //SkillSpecialization objSpec = new SkillSpecialization(objXmlSkill.Attributes["spec"].InnerText);
                                        //objSkill.Specializations.Add(objSpec);
                                    }
                                    //if (Convert.ToInt32(ExpressionToString(objXmlSkill.Attributes["rating"].InnerText, Convert.ToInt32(intForce), 0)) > 6)
                                    //    objSkill.RatingMaximum = Convert.ToInt32(ExpressionToString(objXmlSkill.Attributes["rating"].InnerText, Convert.ToInt32(intForce), 0));
                                    //objSkill.Rating = Convert.ToInt32(ExpressionToString(objXmlSkill.Attributes["rating"].InnerText, Convert.ToInt32(intForce), 0));
                                    break;
                                }
                            }
                        }
                    }

                    //TODO: Sorry, whenever we get critter book...
                    // Set the Skill Group Ratings for the Critter.
                    //foreach (XmlNode objXmlSkill in objXmlCritter.SelectNodes("skills/group"))
                    //{
                    //    foreach (SkillGroup objSkill in _objCharacter.SkillGroups)
                    //    {
                    //        if (objSkill.Name == objXmlSkill.InnerText)
                    //        {
                    //            objSkill.RatingMaximum = Convert.ToInt32(ExpressionToString(objXmlSkill.Attributes["rating"].InnerText, Convert.ToInt32(intForce), 0));
                    //            objSkill.Rating = Convert.ToInt32(ExpressionToString(objXmlSkill.Attributes["rating"].InnerText, Convert.ToInt32(intForce), 0));
                    //            break;
                    //        }
                    //    }
                    //}

                    // Set the Knowledge Skill Ratings for the Critter.
                    //foreach (XmlNode objXmlSkill in objXmlCritter.SelectNodes("skills/knowledge"))
                    //{
                    //    Skill objKnowledge = new Skill(_objCharacter);
                    //    objKnowledge.Name = objXmlSkill.InnerText;
                    //    objKnowledge.KnowledgeSkill = true;
                    //    if (objXmlSkill.Attributes["spec"] != null)
     //                   {
     //                       //SkillSpecialization objSpec = new SkillSpecialization(objXmlSkill.Attributes["spec"].InnerText);
     //                       //objKnowledge.Specializations.Add(objSpec);
     //                   }
                    //    objKnowledge.SkillCategory = objXmlSkill.Attributes["category"].InnerText;
                    //    //if (Convert.ToInt32(objXmlSkill.Attributes["rating"].InnerText) > 6)
                    //    //    objKnowledge.RatingMaximum = Convert.ToInt32(objXmlSkill.Attributes["rating"].InnerText);
                    //    //objKnowledge.Rating = Convert.ToInt32(objXmlSkill.Attributes["rating"].InnerText);
                    //    _objCharacter.Skills.Add(objKnowledge);
                    //}

                    // If this is a Critter with a Force (which dictates their Skill Rating/Maximum Skill Rating), set their Skill Rating Maximums.
                    if (intForce > 0)
                    {
                        int intMaxRating = intForce;
                        // Determine the highest Skill Rating the Critter has.
                        foreach (Skill objSkill in objCharacter.SkillsSection.Skills)
                        {
                            if (objSkill.RatingMaximum > intMaxRating)
                                intMaxRating = objSkill.RatingMaximum;
                        }

                        // Now that we know the upper limit, set all of the Skill Rating Maximums to match.
                        //foreach (Skill objSkill in _objCharacter.Skills)
                        //    objSkill.RatingMaximum = intMaxRating;
                        //foreach (SkillGroup objGroup in _objCharacter.SkillGroups)
                        //    objGroup.RatingMaximum = intMaxRating;

                        // Set the MaxSkillRating for the character so it can be used later when they add new Knowledge Skills or Exotic Skills.
                        
                    }

                    // Add any Complex Forms the Critter comes with (typically Sprites)
                    XmlDocument objXmlProgramDocument = XmlManager.Load("complexforms.xml");
                    foreach (XmlNode objXmlComplexForm in objXmlCritter.SelectNodes("complexforms/complexform"))
                    {
                        int intRating = 0;
                        if (objXmlComplexForm.Attributes["rating"] != null)
                            intRating = ExpressionToInt(objXmlComplexForm.Attributes["rating"].InnerText, intForce, 0);
                        string strForceValue = objXmlComplexForm.Attributes?["select"]?.InnerText ?? string.Empty;
                        XmlNode objXmlComplexFormData = objXmlProgramDocument.SelectSingleNode("/chummer/complexforms/complexform[name = \"" + objXmlComplexForm.InnerText + "\"]");
                        ComplexForm objComplexForm = new ComplexForm(objCharacter);
                        objComplexForm.Create(objXmlComplexFormData, strForceValue);
                        objCharacter.ComplexForms.Add(objComplexForm);
                    }

                    // Add any Gear the Critter comes with (typically Programs for A.I.s)
                    XmlDocument objXmlGearDocument = XmlManager.Load("gear.xml");
                    foreach (XmlNode objXmlGear in objXmlCritter.SelectNodes("gears/gear"))
                    {
                        int intRating = 0;
                        if (objXmlGear.Attributes["rating"] != null)
                            intRating = ExpressionToInt(objXmlGear.Attributes["rating"].InnerText, intForce, 0);
                        string strForceValue = objXmlGear.Attributes?["select"]?.InnerText ?? string.Empty;
                        XmlNode objXmlGearItem = objXmlGearDocument.SelectSingleNode("/chummer/gears/gear[name = \"" + objXmlGear.InnerText + "\"]");
                        Gear objGear = new Gear(objCharacter);
                        List<Weapon> lstWeapons = new List<Weapon>();
                        objGear.Create(objXmlGearItem, intRating, lstWeapons, strForceValue);
                        objGear.Cost = "0";
                        objCharacter.Gear.Add(objGear);
                    }
                }
                catch
                {
                    txtOutput.Text += objCharacter.Metatype + " general failure\r\n";
                }

                objCharacter.DeleteCharacter();
                objCharacter = null;
            }
        }

        /// <summary>
        /// Convert Force, 1D6, or 2D6 into a usable value.
        /// </summary>
        /// <param name="strIn">Expression to convert.</param>
        /// <param name="intForce">Force value to use.</param>
        /// <param name="intOffset">Dice offset.</param>
        /// <returns></returns>
        public static int ExpressionToInt(string strIn, int intForce, int intOffset)
        {
            if (string.IsNullOrWhiteSpace(strIn))
                return intOffset;
            int intValue = 1;
            string strForce = intForce.ToString();
            // This statement is wrapped in a try/catch since trying 1 div 2 results in an error with XSLT.
            try
            {
                intValue = Convert.ToInt32(CommonFunctions.EvaluateInvariantXPath(strIn.Replace("/", " div ").Replace("F", strForce).Replace("1D6", strForce).Replace("2D6", strForce)));
            }
            catch (XPathException)
            {
            }
            intValue += intOffset;
            if (intForce > 0)
            {
                if (intValue < 1)
                    return 1;
            }
            else if (intValue < 0)
                return 0;
            return intValue;
        }

        /// <summary>
        /// Convert Force, 1D6, or 2D6 into a usable value.
        /// </summary>
        /// <param name="strIn">Expression to convert.</param>
        /// <param name="intForce">Force value to use.</param>
        /// <param name="intOffset">Dice offset.</param>
        /// <returns></returns>
        public static string ExpressionToString(string strIn, int intForce, int intOffset)
        {
            return ExpressionToInt(strIn, intForce, intOffset).ToString();
        }
    }
}
