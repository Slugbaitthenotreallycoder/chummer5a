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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace Chummer.Backend.Equipment
{
    /// <summary>
    /// Vehicle Modification.
    /// </summary>
    public class VehicleMod : IHasInternalId, IHasName, IHasXmlNode
    {
        private Guid _guiID;
        private string _strName = string.Empty;
        private string _strCategory = string.Empty;
        private string _strLimit = string.Empty;
        private string _strSlots = "0";
        private int _intRating;
        private string _strMaxRating = "0";
        private string _strCost = string.Empty;
        private decimal _decMarkup;
        private string _strAvail = string.Empty;
        private XmlNode _nodBonus;
        private XmlNode _nodWirelessBonus;
        private bool _blnWirelessOn = true;
        private string _strSource = string.Empty;
        private string _strPage = string.Empty;
        private bool _blnIncludeInVehicle;
        private bool _blnInstalled = true;
        private int _intConditionMonitor;
        private readonly TaggedObservableCollection<Weapon> _lstVehicleWeapons = new TaggedObservableCollection<Weapon>();
        private string _strNotes = string.Empty;
        private string _strSubsystems = string.Empty;
        private readonly TaggedObservableCollection<Cyberware> _lstCyberware = new TaggedObservableCollection<Cyberware>();
        private string _strExtra = string.Empty;
        private string _strWeaponMountCategories = string.Empty;
        private bool _blnDiscountCost;
        private bool _blnDowngrade;
        private string _strCapacity = string.Empty;
        private WeaponMount _objWeaponMountParent;

        private XmlNode _objCachedMyXmlNode;
        private string _strCachedXmlNodeLanguage = string.Empty;
        private string _strAmmoReplace;
        private int _intAmmoBonus;

        private readonly Character _objCharacter;

        #region Constructor, Create, Save, Load, and Print Methods
        public VehicleMod(Character objCharacter)
        {
            // Create the GUID for the new VehicleMod.
            _guiID = Guid.NewGuid();
            _objCharacter = objCharacter;
        }

        /// Create a Vehicle Modification from an XmlNode and return the TreeNodes for it.
        /// <param name="objXmlMod">XmlNode to create the object from.</param>
        /// <param name="objNode">TreeNode to populate a TreeView.</param>
        /// <param name="intRating">Selected Rating for the Gear.</param>
        /// <param name="objParent">Vehicle that the mod will be attached to.</param>
        /// <param name="intMarkup">Discount or markup that applies to the base cost of the mod.</param>
        public void Create(XmlNode objXmlMod, int intRating, Vehicle objParent, decimal decMarkup = 0)
        {
            Parent = objParent ?? throw new ArgumentNullException(nameof(objParent));
            if (objXmlMod == null) Utils.BreakIfDebug();
            if (objXmlMod.TryGetStringFieldQuickly("name", ref _strName))
                _objCachedMyXmlNode = null;
            objXmlMod.TryGetStringFieldQuickly("category", ref _strCategory);
            objXmlMod.TryGetStringFieldQuickly("limit", ref _strLimit);
            objXmlMod.TryGetStringFieldQuickly("slots", ref _strSlots);
            _intRating = intRating;
            if (objXmlMod["downgrade"] != null)
            {
                _blnDowngrade = true;
            }
            if (!objXmlMod.TryGetStringFieldQuickly("altnotes", ref _strNotes))
                objXmlMod.TryGetStringFieldQuickly("notes", ref _strNotes);
            objXmlMod.TryGetStringFieldQuickly("capacity", ref _strCapacity);
            objXmlMod.TryGetStringFieldQuickly("rating", ref _strMaxRating);
            objXmlMod.TryGetInt32FieldQuickly("conditionmonitor", ref _intConditionMonitor);
            objXmlMod.TryGetStringFieldQuickly("weaponmountcategories", ref _strWeaponMountCategories);
            objXmlMod.TryGetStringFieldQuickly("ammoreplace", ref _strAmmoReplace);
            objXmlMod.TryGetInt32FieldQuickly("ammobonus", ref _intAmmoBonus);
            // Add Subsytem information if applicable.
            if (objXmlMod.InnerXml.Contains("subsystems"))
            {
                StringBuilder objSubsystem = new StringBuilder();
                foreach (XmlNode objXmlSubsystem in objXmlMod.SelectNodes("subsystems/subsystem"))
                {
                    objSubsystem.Append(objXmlSubsystem.InnerText + ",");
                }
                // Remove last ","
                if (objSubsystem.Length > 0)
                    objSubsystem.Length -= 1;
                _strSubsystems = objSubsystem.ToString();
            }
            objXmlMod.TryGetStringFieldQuickly("avail", ref _strAvail);

            _strCost = objXmlMod["cost"]?.InnerText ?? string.Empty;
            // Check for a Variable Cost.
            if (_strCost.StartsWith("Variable("))
            {
                decimal decMin;
                decimal decMax = decimal.MaxValue;
                string strCost = _strCost.TrimStart("Variable(", true).TrimEnd(')');
                if (strCost.Contains('-'))
                {
                    string[] strValues = strCost.Split('-');
                    decMin = Convert.ToDecimal(strValues[0], GlobalOptions.InvariantCultureInfo);
                    decMax = Convert.ToDecimal(strValues[1], GlobalOptions.InvariantCultureInfo);
                }
                else
                    decMin = Convert.ToDecimal(strCost.FastEscape('+'), GlobalOptions.InvariantCultureInfo);

                if (decMin != 0 || decMax != decimal.MaxValue)
                {
                    string strNuyenFormat = _objCharacter.Options.NuyenFormat;
                    int intDecimalPlaces = strNuyenFormat.IndexOf('.');
                    if (intDecimalPlaces == -1)
                        intDecimalPlaces = 0;
                    else
                        intDecimalPlaces = strNuyenFormat.Length - intDecimalPlaces - 1;
                    frmSelectNumber frmPickNumber = new frmSelectNumber(intDecimalPlaces);
                    if (decMax > 1000000)
                        decMax = 1000000;
                    frmPickNumber.Minimum = decMin;
                    frmPickNumber.Maximum = decMax;
                    frmPickNumber.Description = LanguageManager.GetString("String_SelectVariableCost", GlobalOptions.Language).Replace("{0}", DisplayNameShort(GlobalOptions.Language));
                    frmPickNumber.AllowCancel = false;
                    frmPickNumber.ShowDialog();
                    _strCost = frmPickNumber.SelectedValue.ToString(GlobalOptions.InvariantCultureInfo);
                }
            }
            _decMarkup = decMarkup;

            objXmlMod.TryGetStringFieldQuickly("source", ref _strSource);
            objXmlMod.TryGetStringFieldQuickly("page", ref _strPage);
            _nodBonus = objXmlMod["bonus"];
            _nodWirelessBonus = objXmlMod["wirelessbonus"];
            _blnWirelessOn = _nodWirelessBonus != null;
        }

        /// <summary>
        /// Save the object's XML to the XmlWriter.
        /// </summary>
        /// <param name="objWriter">XmlTextWriter to write with.</param>
        public void Save(XmlTextWriter objWriter)
        {
            objWriter.WriteStartElement("mod");
            objWriter.WriteElementString("guid", _guiID.ToString("D"));
            objWriter.WriteElementString("name", _strName);
            objWriter.WriteElementString("category", _strCategory);
            objWriter.WriteElementString("limit", _strLimit);
            objWriter.WriteElementString("slots", _strSlots);
            objWriter.WriteElementString("capacity", _strCapacity);
            objWriter.WriteElementString("rating", _intRating.ToString(GlobalOptions.InvariantCultureInfo));
            objWriter.WriteElementString("maxrating", _strMaxRating);
            objWriter.WriteElementString("conditionmonitor", _intConditionMonitor.ToString(GlobalOptions.InvariantCultureInfo));
            objWriter.WriteElementString("avail", _strAvail);
            objWriter.WriteElementString("cost", _strCost);
            objWriter.WriteElementString("markup", _decMarkup.ToString(GlobalOptions.InvariantCultureInfo));
            objWriter.WriteElementString("extra", _strExtra);
            objWriter.WriteElementString("source", _strSource);
            objWriter.WriteElementString("page", _strPage);
            objWriter.WriteElementString("included", _blnIncludeInVehicle.ToString());
            objWriter.WriteElementString("installed", _blnInstalled.ToString());
            objWriter.WriteElementString("wirelesson", _blnWirelessOn.ToString());
            objWriter.WriteElementString("subsystems", _strSubsystems);
            objWriter.WriteElementString("weaponmountcategories", _strWeaponMountCategories);
            objWriter.WriteElementString("ammobonus", _intAmmoBonus.ToString());
            objWriter.WriteElementString("ammoreplace", _strAmmoReplace);
            objWriter.WriteStartElement("weapons");
            foreach (Weapon objWeapon in _lstVehicleWeapons)
                objWeapon.Save(objWriter);
            objWriter.WriteEndElement();
            if (_lstCyberware.Count > 0)
            {
                objWriter.WriteStartElement("cyberwares");
                foreach (Cyberware objCyberware in _lstCyberware)
                    objCyberware.Save(objWriter);
                objWriter.WriteEndElement();
            }
            if (_nodBonus != null)
                objWriter.WriteRaw(_nodBonus.OuterXml);
            if (_nodWirelessBonus != null)
                objWriter.WriteRaw(_nodWirelessBonus.OuterXml);
            objWriter.WriteElementString("notes", _strNotes);
            objWriter.WriteElementString("discountedcost", _blnDiscountCost.ToString());
            objWriter.WriteEndElement();
            _objCharacter.SourceProcess(_strSource);
        }

        /// <summary>
        /// Load the VehicleMod from the XmlNode.
        /// </summary>
        /// <param name="objNode">XmlNode to load.</param>
        /// <param name="blnCopy">Indicates whether a new item will be created as a copy of this one.</param>
        public void Load(XmlNode objNode, bool blnCopy = false)
        {
            if (blnCopy)
            {
                _guiID = Guid.NewGuid();
            }
            else
            {
                objNode.TryGetField("guid", Guid.TryParse, out _guiID);
            }
            if (objNode.TryGetStringFieldQuickly("name", ref _strName))
                _objCachedMyXmlNode = null;
            objNode.TryGetStringFieldQuickly("category", ref _strCategory);
            objNode.TryGetStringFieldQuickly("limit", ref _strLimit);
            objNode.TryGetStringFieldQuickly("slots", ref _strSlots);
            objNode.TryGetInt32FieldQuickly("rating", ref _intRating);
            objNode.TryGetStringFieldQuickly("maxrating", ref _strMaxRating);
            objNode.TryGetStringFieldQuickly("capacity", ref _strCapacity);
            objNode.TryGetStringFieldQuickly("weaponmountcategories", ref _strWeaponMountCategories);
            objNode.TryGetStringFieldQuickly("page", ref _strPage);
            objNode.TryGetStringFieldQuickly("avail", ref _strAvail);
            objNode.TryGetInt32FieldQuickly("conditionmonitor", ref _intConditionMonitor);
            objNode.TryGetStringFieldQuickly("cost", ref _strCost);
            objNode.TryGetDecFieldQuickly("markup", ref _decMarkup);
            objNode.TryGetStringFieldQuickly("source", ref _strSource);
            objNode.TryGetBoolFieldQuickly("included", ref _blnIncludeInVehicle);
            objNode.TryGetBoolFieldQuickly("installed", ref _blnInstalled);
            objNode.TryGetInt32FieldQuickly("ammobonus", ref _intAmmoBonus);
            objNode.TryGetStringFieldQuickly("ammoreplace", ref _strAmmoReplace);
            objNode.TryGetStringFieldQuickly("subsystems", ref _strSubsystems);
            // Legacy Shims
            if (Name.StartsWith("Gecko Tips (Bod"))
            {
                Name = "Gecko Tips";
                XmlNode objNewNode = GetNode();
                if (objNewNode != null)
                {
                    objNewNode.TryGetStringFieldQuickly("cost", ref _strCost);
                    objNewNode.TryGetStringFieldQuickly("slots", ref _strSlots);
                }
            }
            if (Name.StartsWith("Gliding System (Bod"))
            {
                Name = "Gliding System";
                XmlNode objNewNode = GetNode();
                if (objNewNode != null)
                {
                    objNewNode.TryGetStringFieldQuickly("cost", ref _strCost);
                    objNewNode.TryGetStringFieldQuickly("slots", ref _strSlots);
                    objNewNode.TryGetStringFieldQuickly("avail", ref _strAvail);
                }
            }

            XmlNode xmlChildrenNode = objNode["weapons"];
            if (xmlChildrenNode != null)
            {
                foreach (XmlNode nodChild in xmlChildrenNode.SelectNodes("weapon"))
                {
                    Weapon objWeapon = new Weapon(_objCharacter)
                    {
                        ParentVehicle = Parent
                    };
                    objWeapon.Load(nodChild, blnCopy);
                    _lstVehicleWeapons.Add(objWeapon);
                }
            }
            xmlChildrenNode = objNode["cyberwares"];
            using (XmlNodeList xmlNodeList = xmlChildrenNode?.SelectNodes("cyberware"))
                if (xmlNodeList != null)
                    foreach (XmlNode nodChild in xmlNodeList)
                    {
                        Cyberware objCyberware = new Cyberware(_objCharacter)
                        {
                            ParentVehicle = Parent
                        };
                        objCyberware.Load(nodChild, blnCopy);
                        _lstCyberware.Add(objCyberware);
                    }

            _nodBonus = objNode["bonus"];
            _nodWirelessBonus = objNode["wirelessbonus"];
            if (!objNode.TryGetBoolFieldQuickly("wirelesson", ref _blnWirelessOn))
                _blnWirelessOn = _nodWirelessBonus != null;
            objNode.TryGetStringFieldQuickly("notes", ref _strNotes);
            objNode.TryGetBoolFieldQuickly("discountedcost", ref _blnDiscountCost);
            objNode.TryGetStringFieldQuickly("extra", ref _strExtra);
        }

        /// <summary>
        /// Print the object's XML to the XmlWriter.
        /// </summary>
        /// <param name="objWriter">XmlTextWriter to write with.</param>
        public void Print(XmlTextWriter objWriter, CultureInfo objCulture, string strLanguageToPrint)
        {
            objWriter.WriteStartElement("mod");
            objWriter.WriteElementString("name", DisplayNameShort(strLanguageToPrint));
            objWriter.WriteElementString("category", DisplayCategory(strLanguageToPrint));
            objWriter.WriteElementString("limit", Limit);
            objWriter.WriteElementString("slots", Slots);
            objWriter.WriteElementString("rating", Rating.ToString(objCulture));
            objWriter.WriteElementString("avail", TotalAvail(objCulture, strLanguageToPrint));
            objWriter.WriteElementString("cost", TotalCost.ToString(_objCharacter.Options.NuyenFormat, objCulture));
            objWriter.WriteElementString("owncost", OwnCost.ToString(_objCharacter.Options.NuyenFormat, objCulture));
            objWriter.WriteElementString("source", CommonFunctions.LanguageBookShort(Source, strLanguageToPrint));
            objWriter.WriteElementString("wirelesson", WirelessOn.ToString());
            objWriter.WriteElementString("page", Page(strLanguageToPrint));
            objWriter.WriteElementString("included", IncludedInVehicle.ToString());
            objWriter.WriteStartElement("weapons");
            foreach (Weapon objWeapon in Weapons)
                objWeapon.Print(objWriter, objCulture, strLanguageToPrint);
            objWriter.WriteEndElement();
            objWriter.WriteStartElement("cyberwares");
            foreach (Cyberware objCyberware in Cyberware)
                objCyberware.Print(objWriter, objCulture, strLanguageToPrint);
            objWriter.WriteEndElement();
            if (_objCharacter.Options.PrintNotes)
                objWriter.WriteElementString("notes", Notes);
            objWriter.WriteEndElement();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Weapons.
        /// </summary>
        public TaggedObservableCollection<Weapon> Weapons => _lstVehicleWeapons;

        public TaggedObservableCollection<Cyberware> Cyberware => _lstCyberware;

        public WeaponMount WeaponMountParent
        {
            get => _objWeaponMountParent;
            set => _objWeaponMountParent = value;
        }

        /// <summary>
        /// Internal identifier which will be used to identify this piece of Gear in the Character.
        /// </summary>
        public string InternalId => _guiID.ToString("D");

        /// <summary>
        /// Name.
        /// </summary>
        public string Name
        {
            get => _strName;
            set
            {
                if (_strName != value)
                {
                    _objCachedMyXmlNode = null;
                    _strName = value;
                }
            }
        }

        /// <summary>
        /// Translated Category.
        /// </summary>
        public string DisplayCategory(string strLanguage)
        {
            if (strLanguage == GlobalOptions.DefaultLanguage)
                return Category;

            return XmlManager.Load("vehicles.xml", strLanguage).SelectSingleNode("/chummer/categories/category[. = \"" + Category + "\"]/@translate")?.InnerText ?? Category;
        }

        /// <summary>
        /// Category.
        /// </summary>
        public string Category
        {
            get => _strCategory;
            set => _strCategory = value;
        }

        /// <summary>
        /// Limits the Weapon Selection form to specified categories.
        /// </summary>
        public string WeaponMountCategories
        {
            set => _strWeaponMountCategories = value;
            get => _strWeaponMountCategories;
        }

        /// <summary>
        /// Which Vehicle types the Mod is limited to.
        /// </summary>
        public string Limit
        {
            get => _strLimit;
            set => _strLimit = value;
        }

        /// <summary>
        /// Number of Slots the Mod uses.
        /// </summary>
        public string Slots
        {
            get => _strSlots;
            set => _strSlots = value;
        }

        /// <summary>
        /// Rating.
        /// </summary>
        public int Rating
        {
            get => _intRating;
            set => _intRating = Math.Max(0, value);
        }

        /// <summary>
        /// Maximum Rating.
        /// </summary>
        public string MaxRating
        {
            get => _strMaxRating;
            set => _strMaxRating = value;
        }

        /// <summary>
        /// Cost.
        /// </summary>
        public string Cost
        {
            get => _strCost;
            set => _strCost = value;
        }

        /// <summary>
        /// Markup.
        /// </summary>
        public decimal Markup
        {
            get => _decMarkup;
            set => _decMarkup = value;
        }

        /// <summary>
        /// Availability.
        /// </summary>
        public string Avail
        {
            get => _strAvail;
            set => _strAvail = value;
        }

        /// <summary>
        /// Sourcebook.
        /// </summary>
        public string Source
        {
            get => _strSource;
            set => _strSource = value;
        }

        /// <summary>
        /// Sourcebook Page Number.
        /// </summary>
        public string Page(string strLanguage)
        {
            if (strLanguage == GlobalOptions.DefaultLanguage)
                return _strPage;

            return GetNode(strLanguage)?["altpage"]?.InnerText ?? _strPage;
        }

        /// <summary>
        /// Bonus node.
        /// </summary>
        public XmlNode Bonus
        {
            get => _nodBonus;
            set => _nodBonus = value;
        }

        /// <summary>
        /// Wireless Bonus node.
        /// </summary>
        public XmlNode WirelessBonus
        {
            get => _nodWirelessBonus;
            set => _nodWirelessBonus = value;
        }

        /// <summary>
        /// Whether the vehicle mod's wireless is enabled
        /// </summary>
        public bool WirelessOn
        {
            get => _blnWirelessOn;
            set => _blnWirelessOn = value;
        }

        /// <summary>
        /// Whether or not the Mod included with the Vehicle by default.
        /// </summary>
        public bool IncludedInVehicle
        {
            get => _blnIncludeInVehicle;
            set => _blnIncludeInVehicle = value;
        }

        /// <summary>
        /// Whether or not this Mod is installed and contributing towards the Vehicle's stats.
        /// </summary>
        public bool Installed
        {
            get => _blnInstalled;
            set => _blnInstalled = value;
        }

        /// <summary>
        /// Notes.
        /// </summary>
        public string Notes
        {
            get => _strNotes;
            set => _strNotes = value;
        }

        /// <summary>
        /// Whether or not the Vehicle Mod allows Cyberware Plugins.
        /// </summary>
        public bool AllowCyberware => !string.IsNullOrEmpty(_strSubsystems);

        /// <summary>
        /// Allowed Cyberwarre Subsystems.
        /// </summary>
        public string Subsystems
        {
            get => _strSubsystems;
            set => _strSubsystems = value;
        }

        /// <summary>
        /// Value that was selected during an ImprovementManager dialogue.
        /// </summary>
        public string Extra
        {
            get => _strExtra;
            set => _strExtra = value;
        }

        /// <summary>
        /// Whether or not the Vehicle Mod's cost should be discounted by 10% through the Black Market Pipeline Quality.
        /// </summary>
        public bool DiscountCost
        {
            get => _blnDiscountCost && _objCharacter.BlackMarketDiscount;
            set => _blnDiscountCost = value;
        }

        /// <summary>
        /// Whether or not the Vehicle Mod is a downgrade for drone attributes
        /// </summary>
        public bool Downgrade => _blnDowngrade;

        /// <summary>
        /// Bonus/Penalty to the parent vehicle that this mod provides.
        /// </summary>
        public int ConditionMonitor => _intConditionMonitor;

        /// <summary>
        /// Vehicle that the Mod is attached to. 
        /// </summary>
        public Vehicle Parent { internal get; set; }

        /// <summary>
        /// Adjust the Weapon's Ammo amount by the specified percent.
        /// </summary>
        public int AmmoBonus
        {
            get => _intAmmoBonus;
            set => _intAmmoBonus = value;
        }

        /// <summary>
        /// Replace the Weapon's Ammo value with the Weapon Mod's value.
        /// </summary>
        public string AmmoReplace
        {
            get => _strAmmoReplace;
            set => _strAmmoReplace = value;
        }
        #endregion

        #region Complex Properties
        /// <summary>
        /// Total Availablility of the VehicleMod.
        /// </summary>
        public string TotalAvail(CultureInfo objCulture, string strLanguage)
        {
            return TotalAvailTuple().ToString(objCulture, strLanguage);
        }

        /// <summary>
        /// Total Availability as a triple.
        /// </summary>
        public AvailabilityValue TotalAvailTuple(bool blnCheckChildren = true)
        {
            bool blnModifyParentAvail = false;
            string strAvail = Avail;
            char chrLastAvailChar = ' ';
            int intAvail = 0;
            if (strAvail.Length > 0)
            {
                // Reordered to process fixed value strings
                if (strAvail.StartsWith("FixedValues("))
                {
                    string[] strValues = strAvail.TrimStart("FixedValues(", true).TrimEnd(')').Split(',');
                    strAvail = strValues[Math.Max(Math.Min(Rating, strValues.Length) - 1, 0)];
                }

                if (strAvail.StartsWith("Range("))
                {
                    // If the Availability code is based on the current Rating of the item, separate the Availability string into an array and find the first bracket that the Rating is lower than or equal to.
                    string[] strValues = strAvail.Replace("MaxRating", MaxRating).TrimStart("Range(", true).TrimEnd(')').Split(',');
                    foreach (string strValue in strValues)
                    {
                        string strAvailCode = strValue.Split('[')[1].Trim('[', ']');
                        int intMax = Convert.ToInt32(strValue.Split('[')[0]);
                        if (Rating > intMax)
                            continue;
                        strAvail = $"{Rating}{strAvailCode}";
                        break;
                    }
                }

                chrLastAvailChar = strAvail[strAvail.Length - 1];
                if (chrLastAvailChar == 'F' || chrLastAvailChar == 'R')
                {
                    strAvail = strAvail.Substring(0, strAvail.Length - 1);
                }

                blnModifyParentAvail = strAvail.StartsWith('+', '-');
                strAvail = strAvail.TrimStart('+');

                // If the availability is determined by the Rating, evaluate the expression.

                string strAvailExpr = strAvail.Replace("Rating", Rating.ToString());
                if (Parent != null)
                {
                    strAvailExpr = strAvailExpr.CheapReplace("Vehicle Cost",
                        () => Parent.OwnCost.ToString(GlobalOptions.InvariantCultureInfo));
                    // If the Body is 0 (Microdrone), treat it as 0.5 for the purposes of determine Modification cost.
                    strAvailExpr = strAvailExpr.Replace("Body", Parent.Body > 0 ? Parent.Body.ToString() : "0.5");
                    strAvailExpr = strAvailExpr.Replace("Speed", Parent.Speed.ToString());
                    strAvailExpr = strAvailExpr.Replace("Acceleration", Parent.Accel.ToString());
                    strAvailExpr = strAvailExpr.Replace("Handling", Parent.Handling.ToString());
                }
                try
                {
                    intAvail += Convert.ToInt32(CommonFunctions.EvaluateInvariantXPath(strAvailExpr));
                }
                catch (XPathException)
                {
                }
            }

            if (blnCheckChildren)
            {
                // Run through cyberware children and increase the Avail by any Mod whose Avail starts with "+" or "-".
                foreach (Cyberware objChild in Cyberware)
                {
                    if (objChild.ParentID != InternalId)
                    {
                        AvailabilityValue objLoopAvailTuple = objChild.TotalAvailTuple();
                        if (objLoopAvailTuple.AddToParent)
                            intAvail += objLoopAvailTuple.Value;
                        if (objLoopAvailTuple.Suffix == 'F')
                            chrLastAvailChar = 'F';
                        else if (chrLastAvailChar != 'F' && objLoopAvailTuple.Suffix == 'R')
                            chrLastAvailChar = 'R';
                    }
                }

                // Run through weapon children and increase the Avail by any Mod whose Avail starts with "+" or "-".
                foreach (Weapon objChild in Weapons)
                {
                    if (objChild.ParentID != InternalId)
                    {
                        AvailabilityValue objLoopAvailTuple = objChild.TotalAvailTuple();
                        if (objLoopAvailTuple.AddToParent)
                            intAvail += objLoopAvailTuple.Value;
                        if (objLoopAvailTuple.Suffix == 'F')
                            chrLastAvailChar = 'F';
                        else if (chrLastAvailChar != 'F' && objLoopAvailTuple.Suffix == 'R')
                            chrLastAvailChar = 'R';
                    }
                }
            }

            return new AvailabilityValue(intAvail, chrLastAvailChar, blnModifyParentAvail);
        }

        /// <summary>
        /// Caculated Capacity of the Vehicle Mod.
        /// </summary>
        public string CalculatedCapacity
        {
            get
            {
                string strReturn;
                int intPos = _strCapacity.IndexOf("/[", StringComparison.Ordinal);
                if (!string.IsNullOrEmpty(_strCapacity) && intPos != -1)
                {
                    string strFirstHalf = _strCapacity.Substring(0, intPos);
                    string strSecondHalf = _strCapacity.Substring(intPos + 1, _strCapacity.Length - intPos - 1);
                    bool blnSquareBrackets = strFirstHalf.Contains('[');
                    string strCapacity = strFirstHalf;

                    if (blnSquareBrackets && strCapacity.Length > 2)
                        strCapacity = strCapacity.Substring(1, strCapacity.Length - 2);

                    if (_strCapacity == "[*]")
                        strReturn = "*";
                    else
                    {
                        if (_strCapacity.StartsWith("FixedValues("))
                        {
                            string[] strValues = _strCapacity.TrimStart("FixedValues(", true).TrimEnd(')').Split(',');
                            strReturn = strValues[Math.Max(Math.Min(Rating, strValues.Length) - 1, 0)];
                        }
                        else
                        {
                            try
                            {
                                strReturn = ((double)CommonFunctions.EvaluateInvariantXPath(strCapacity.Replace("Rating", Rating.ToString()))).ToString("#,0.##", GlobalOptions.CultureInfo);
                            }
                            catch (XPathException)
                            {
                                strReturn = strCapacity;
                            }
                            catch (OverflowException) // Result is text and not a double
                            {
                                strReturn = strCapacity;
                            }
                            catch (InvalidCastException) // Result is text and not a double
                            {
                                strReturn = strCapacity;
                            }
                        }
                    }
                    if (blnSquareBrackets)
                        strReturn = '[' + strCapacity + ']';

                    if (strSecondHalf.Contains("Rating"))
                    {
                        strSecondHalf = strSecondHalf.Trim('[', ']');
                        try
                        {
                            strSecondHalf = '[' + ((double)CommonFunctions.EvaluateInvariantXPath(strSecondHalf.Replace("Rating", Rating.ToString()))).ToString("#,0.##", GlobalOptions.CultureInfo) + ']';
                        }
                        catch (XPathException)
                        {
                            strSecondHalf = '[' + strSecondHalf + ']';
                        }
                        catch (OverflowException) // Result is text and not a double
                        {
                            strSecondHalf = '[' + strSecondHalf + ']';
                        }
                        catch (InvalidCastException) // Result is text and not a double
                        {
                            strSecondHalf = '[' + strSecondHalf + ']';
                        }
                    }

                    strReturn += "/" + strSecondHalf;
                }
                else if (_strCapacity.Contains("Rating"))
                {
                    // If the Capaicty is determined by the Rating, evaluate the expression.
                    // XPathExpression cannot evaluate while there are square brackets, so remove them if necessary.
                    bool blnSquareBrackets = _strCapacity.Contains('[');
                    string strCapacity = _strCapacity;
                    if (blnSquareBrackets)
                        strCapacity = strCapacity.Substring(1, strCapacity.Length - 2);
                    strReturn = ((double)CommonFunctions.EvaluateInvariantXPath(strCapacity.Replace("Rating", Rating.ToString()))).ToString("#,0.##", GlobalOptions.CultureInfo);
                    if (blnSquareBrackets)
                        strReturn = '[' + strReturn + ']';
                }
                else
                {
                    if (_strCapacity.StartsWith("FixedValues("))
                    {
                        string[] strValues = _strCapacity.TrimStart("FixedValues(", true).TrimEnd(')').Split(',');
                        strReturn = strValues[Math.Max(Math.Min(Rating, strValues.Length) - 1, 0)];
                    }
                    else
                    {
                        // Just a straight Capacity, so return the value.
                        strReturn = _strCapacity;
                    }
                }
                if (string.IsNullOrEmpty(strReturn))
                    strReturn = "0";
                if (decimal.TryParse(strReturn, NumberStyles.Any, GlobalOptions.InvariantCultureInfo, out decimal decReturn))
                    return decReturn.ToString("#,0.##", GlobalOptions.CultureInfo);
                return strReturn;
            }
        }

        /// <summary>
        /// The amount of Capacity remaining in the Cyberware.
        /// </summary>
        public decimal CapacityRemaining
        {
            get
            {
                if (string.IsNullOrEmpty(_strCapacity))
                    return 0.0m;
                decimal decCapacity = 0;
                if (_strCapacity.Contains("/["))
                {
                    // Get the Cyberware base Capacity.
                    string strBaseCapacity = CalculatedCapacity;
                    strBaseCapacity = strBaseCapacity.Substring(0, strBaseCapacity.IndexOf('/'));
                    decCapacity = Convert.ToDecimal(strBaseCapacity, GlobalOptions.CultureInfo);

                    // Run through its Children and deduct the Capacity costs.
                    foreach (Cyberware objChildCyberware in Cyberware)
                    {
                        string strCapacity = objChildCyberware.CalculatedCapacity;
                        if (strCapacity.Contains("/["))
                            strCapacity = strCapacity.Substring(strCapacity.IndexOf('[') + 1, strCapacity.IndexOf(']') - strCapacity.IndexOf('[') - 1);
                        else if (strCapacity.Contains('['))
                            strCapacity = strCapacity.Substring(1, strCapacity.Length - 2);
                        if (strCapacity == "*")
                            strCapacity = "0";
                        decCapacity -= Convert.ToDecimal(strCapacity, GlobalOptions.CultureInfo);
                    }
                }
                else if (!_strCapacity.Contains('['))
                {
                    // Get the Cyberware base Capacity.
                    decCapacity = Convert.ToDecimal(CalculatedCapacity, GlobalOptions.CultureInfo);

                    // Run through its Children and deduct the Capacity costs.
                    foreach (Cyberware objChildCyberware in Cyberware)
                    {
                        string strCapacity = objChildCyberware.CalculatedCapacity;
                        if (strCapacity.Contains("/["))
                            strCapacity = strCapacity.Substring(strCapacity.IndexOf('[') + 1, strCapacity.IndexOf(']') - strCapacity.IndexOf('[') - 1);
                        else if (strCapacity.Contains('['))
                            strCapacity = strCapacity.Substring(1, strCapacity.Length - 2);
                        if (strCapacity == "*")
                            strCapacity = "0";
                        decCapacity -= Convert.ToDecimal(strCapacity, GlobalOptions.CultureInfo);
                    }
                }

                return decCapacity;
            }
        }

        /// <summary>
        /// Total cost of the VehicleMod.
        /// </summary>
        public decimal TotalCostInMountCreation(int intSlots)
        {
            // If the cost is determined by the Rating, evaluate the expression.
            string strCostExpression = _strCost;
            if (strCostExpression.StartsWith("FixedValues("))
            {
                string[] strValues = strCostExpression.TrimStart("FixedValues(", true).TrimEnd(')').Split(',');
                strCostExpression = strValues[Math.Max(Math.Min(Rating, strValues.Length) - 1, 0)];
            }
            string strCost = strCostExpression.Replace("Rating", Rating.ToString());
            if (Parent != null)
            {
                strCost = strCost.CheapReplace("Vehicle Cost",
                    () => Parent.OwnCost.ToString(GlobalOptions.InvariantCultureInfo));
                // If the Body is 0 (Microdrone), treat it as 0.5 for the purposes of determine Modification cost.
                strCost = strCost.Replace("Body", Parent.Body > 0 ? Parent.Body.ToString() : "0.5");
                strCost = strCost.Replace("Speed", Parent.Speed.ToString());
                strCost = strCost.Replace("Acceleration", Parent.Accel.ToString());
                strCost = strCost.Replace("Handling", Parent.Handling.ToString());
            }
            strCost = strCost.CheapReplace("Slots", () => intSlots.ToString());
            decimal decReturn = Convert.ToDecimal(CommonFunctions.EvaluateInvariantXPath(strCost), GlobalOptions.InvariantCultureInfo);

            if (DiscountCost)
                decReturn *= 0.9m;

            // Apply a markup if applicable.
            if (_decMarkup != 0)
            {
                decReturn *= 1 + (_decMarkup / 100.0m);
            }

            return decReturn + Weapons.AsParallel().Sum(objWeapon => objWeapon.TotalCost) + Cyberware.AsParallel().Sum(objCyberware => objCyberware.TotalCost);
        }

        /// <summary>
        /// Total cost of the VehicleMod.
        /// </summary>
        public decimal TotalCost
        {
            get
            {
                return OwnCost + Weapons.AsParallel().Sum(objWeapon => objWeapon.TotalCost) + Cyberware.AsParallel().Sum(objCyberware => objCyberware.TotalCost);
            }
        }

        /// <summary>
        /// The cost of just the Vehicle Mod itself.
        /// </summary>
        public decimal OwnCost
        {
            get
            {
                // If the cost is determined by the Rating, evaluate the expression.
                string strCostExpression = _strCost;
                if (strCostExpression.StartsWith("FixedValues("))
                {
                    string[] strValues = strCostExpression.TrimStart("FixedValues(", true).TrimEnd(')').Split(',');
                    strCostExpression = strValues[Math.Max(Math.Min(Rating, strValues.Length) - 1, 0)];
                }
                string strCost = strCostExpression.Replace("Rating", Rating.ToString());
                if (Parent != null)
                {
                    strCost = strCost.CheapReplace("Vehicle Cost",
                        () => Parent.OwnCost.ToString(GlobalOptions.InvariantCultureInfo));
                    // If the Body is 0 (Microdrone), treat it as 0.5 for the purposes of determine Modification cost.
                    strCost = strCost.Replace("Body", Parent.Body > 0 ? Parent.Body.ToString() : "0.5");
                    strCost = strCost.Replace("Speed", Parent.Speed.ToString());
                    strCost = strCost.Replace("Acceleration", Parent.Accel.ToString());
                    strCost = strCost.Replace("Handling", Parent.Handling.ToString());
                }
                strCost = strCost.CheapReplace("Slots", () => WeaponMountParent?.CalculatedSlots.ToString() ?? "0");
                decimal decReturn = Convert.ToDecimal(CommonFunctions.EvaluateInvariantXPath(strCost), GlobalOptions.InvariantCultureInfo);

                if (DiscountCost)
                    decReturn *= 0.9m;

                // Apply a markup if applicable.
                if (_decMarkup != 0)
                {
                    decReturn *= 1 + (_decMarkup / 100.0m);
                }

                return decReturn;
            }
        }

        /// <summary>
        /// The number of Slots the Mod consumes.
        /// </summary>
        public int CalculatedSlots
        {
            get
            {
                // If the slots is determined by the Rating, evaluate the expression.
                string strSlotsExpression = Slots;
                if (strSlotsExpression.StartsWith("FixedValues("))
                {
                    string[] strValues = strSlotsExpression.TrimStart("FixedValues(", true).TrimEnd(')').Split(',');
                    strSlotsExpression = (strValues[Math.Max(Math.Min(Rating, strValues.Length) - 1, 0)]);
                }
                string strSlots = strSlotsExpression.Replace("Rating", Rating.ToString());
                if (Parent != null)
                {
                    strSlots = strSlots.CheapReplace("Vehicle Cost",
                        () => Parent.OwnCost.ToString(GlobalOptions.InvariantCultureInfo));
                    // If the Body is 0 (Microdrone), treat it as 0.5 for the purposes of determine Modification cost.
                    strSlots = strSlots.Replace("Body", Parent.Body > 0 ? Parent.Body.ToString() : "0.5");
                    strSlots = strSlots.Replace("Speed", Parent.Speed.ToString());
                    strSlots = strSlots.Replace("Acceleration", Parent.Accel.ToString());
                    strSlots = strSlots.Replace("Handling", Parent.Handling.ToString());
                }
                return Convert.ToInt32(CommonFunctions.EvaluateInvariantXPath(strSlots));
            }
        }

        /// <summary>
        /// The name of the object as it should be displayed on printouts (translated name only).
        /// </summary>
        public string DisplayNameShort(string strLanguage)
        {
            if (strLanguage == GlobalOptions.DefaultLanguage)
                return Name;

            return GetNode(strLanguage)?["translate"]?.InnerText ?? Name;
        }

        /// <summary>
        /// The name of the object as it should be displayed in lists. Qty Name (Rating) (Extra).
        /// </summary>
        public string DisplayName(string strLanguage)
        {
            string strReturn = DisplayNameShort(strLanguage);

            if (!string.IsNullOrEmpty(_strExtra))
                strReturn += " (" + LanguageManager.TranslateExtra(_strExtra, strLanguage) + ')';
            if (Rating > 0)
                strReturn += " (" + LanguageManager.GetString("String_Rating", strLanguage) + ' ' + Rating.ToString() + ')';
            return strReturn;
        }

        /// <summary>
        /// Vehicle arm/leg Strength.
        /// </summary>
        public int TotalStrength
        {
            get
            {
                string strName = Name.ToLower();
                if (!strName.Contains("arm") && !strName.Contains("leg"))
                    return 0;
                int intAttribute = 0;
                int bod = 1;
                if (Parent != null)
                {
                    bod = Parent.TotalBody * 2;
                    intAttribute = Math.Max(Parent.TotalBody, 0);
                }
                int intBonus = 0;

                foreach (Cyberware objChild in Cyberware)
                {
                    // If the limb has Customized Strength, this is its new base value.
                    if (objChild.Name == "Customized Strength")
                        intAttribute = objChild.Rating;
                    // If the limb has Enhanced Strength, this adds to the limb's value.
                    if (objChild.Name == "Enhanced Strength")
                        intBonus = objChild.Rating;
                }
                return Math.Min(intAttribute + intBonus, Math.Max(bod, 1));
            }
        }
        
        /// <summary>
        /// Vehicle arm/leg Agility.
        /// </summary>
        public int TotalAgility
        {
            get
            {
                string strName = Name.ToLower();
                if (!strName.Contains("arm") && !strName.Contains("leg"))
                    return 0;

                int intAttribute = 0;
                int pilot = 1;
                if (Parent != null)
                {
                    pilot = Parent.TotalBody * 2;
                    intAttribute = Math.Max(Parent.Pilot, 0);
                }
                int intBonus = 0;

                foreach (Cyberware objChild in Cyberware)
                {
                    // If the limb has Customized Agility, this is its new base value.
                    if (objChild.Name == "Customized Agility")
                        intAttribute = objChild.Rating;
                    // If the limb has Enhanced Agility, this adds to the limb's value.
                    if (objChild.Name == "Enhanced Agility")
                        intBonus = objChild.Rating;
                }

                return Math.Min(intAttribute + intBonus, Math.Max(pilot, 1));
            }
        }

        /// <summary>
        /// Whether or not the Mod is allowed to accept Cyberware Modular Plugins.
        /// </summary>
        public bool AllowModularPlugins
        {
            get
            {
                return Cyberware.Any(objChild => objChild.AllowedSubsystems.Contains("Modular Plug-In"));
            }
        }

        public XmlNode GetNode()
        {
            return GetNode(GlobalOptions.Language);
        }

        public XmlNode GetNode(string strLanguage)
        {
            if (_objCachedMyXmlNode == null || strLanguage != _strCachedXmlNodeLanguage || GlobalOptions.LiveCustomData)
            {
                XmlDocument objDoc = XmlManager.Load("vehicles.xml", strLanguage);
                _objCachedMyXmlNode = objDoc.SelectSingleNode("/chummer/mods/mod[name = \"" + Name + "\"]") ?? objDoc.SelectSingleNode("/chummer/weaponmountmods/mod[name = \"" + Name + "\"]");
                _strCachedXmlNodeLanguage = strLanguage;
            }
            return _objCachedMyXmlNode;
        }
        #endregion

        #region Methods
        public decimal DeleteVehicleMod()
        {
            decimal decReturn = 0;

            foreach (Weapon objLoopWeapon in Weapons)
            {
                decReturn += objLoopWeapon.DeleteWeapon();
            }
            foreach (Cyberware objLoopCyberware in Cyberware)
            {
                decReturn += objLoopCyberware.DeleteCyberware();
            }

            return decReturn;
        }

        /// <summary>
        /// Add a piece of Armor to the Armor TreeView.
        /// </summary>
        /// <param name="treArmor">Armor TreeView.</param>
        /// <param name="cmsArmor">ContextMenuStrip for the Armor Node.</param>
        /// <param name="cmsArmorMod">ContextMenuStrip for Armor Mod Nodes.</param>
        /// <param name="cmsArmorGear">ContextMenuStrip for Armor Gear Nodes.</param>
        public TreeNode CreateTreeNode(ContextMenuStrip cmsVehicleMod, ContextMenuStrip cmsCyberware, ContextMenuStrip cmsCyberwareGear, ContextMenuStrip cmsVehicleWeapon, ContextMenuStrip cmsVehicleWeaponAccessory, ContextMenuStrip cmsVehicleWeaponAccessoryGear)
        {
            TreeNode objNode = new TreeNode
            {
                Name = InternalId,
                Text = DisplayName(GlobalOptions.Language),
                Tag = InternalId,
                ContextMenuStrip = cmsVehicleMod
            };
            if (!string.IsNullOrEmpty(Notes))
                objNode.ForeColor = Color.SaddleBrown;
            else if (IncludedInVehicle)
                objNode.ForeColor = SystemColors.GrayText;
            objNode.ToolTipText = Notes.WordWrap(100);

            // Cyberware.
            foreach (Cyberware objCyberware in Cyberware)
            {
                objNode.Nodes.Add(objCyberware.CreateTreeNode(cmsCyberware, cmsCyberwareGear));
                objNode.Expand();
            }
            
            // VehicleWeapons.
            foreach (Weapon objWeapon in Weapons)
            {
                objNode.Nodes.Add(objWeapon.CreateTreeNode(cmsVehicleWeapon, cmsVehicleWeaponAccessory, cmsVehicleWeaponAccessoryGear));
                objNode.Expand();
            }

            return objNode;
        }
        #endregion
    }
}
