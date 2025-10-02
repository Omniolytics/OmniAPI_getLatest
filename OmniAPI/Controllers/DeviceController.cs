using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace OmniAPI.Controllers
{
    public class DeviceLimitUpdateRequest
    {
        private long? _deviceDataID;
        private double? _highLimit;
        private double? _lowLimit;
        private bool? _thresholdEnabled;
        private string _primaryContact;
        private string _secondaryContact;
        private int? _secondaryContactDelay;
        private bool? _fuzzyLimits;
        private double? _fuzzyLow;
        private double? _fuzzyHigh;

        [JsonProperty("deviceDataID")]
        public long? DeviceDataID
        {
            get => _deviceDataID;
            set
            {
                _deviceDataID = value;
                DeviceDataIDProvided = true;
            }
        }

        [JsonIgnore]
        public bool DeviceDataIDProvided { get; private set; }

        [JsonProperty("highLimit")]
        public double? HighLimit
        {
            get => _highLimit;
            set
            {
                _highLimit = value;
                HighLimitProvided = true;
            }
        }

        [JsonIgnore]
        public bool HighLimitProvided { get; private set; }

        [JsonProperty("lowLimit")]
        public double? LowLimit
        {
            get => _lowLimit;
            set
            {
                _lowLimit = value;
                LowLimitProvided = true;
            }
        }

        [JsonIgnore]
        public bool LowLimitProvided { get; private set; }

        [JsonProperty("thresholdEnabled")]
        public bool? ThresholdEnabled
        {
            get => _thresholdEnabled;
            set
            {
                _thresholdEnabled = value;
                ThresholdEnabledProvided = true;
            }
        }

        [JsonIgnore]
        public bool ThresholdEnabledProvided { get; private set; }

        [JsonProperty("primaryContact")]
        public string PrimaryContact
        {
            get => _primaryContact;
            set
            {
                _primaryContact = value;
                PrimaryContactProvided = true;
            }
        }

        [JsonIgnore]
        public bool PrimaryContactProvided { get; private set; }

        [JsonProperty("secondaryContact")]
        public string SecondaryContact
        {
            get => _secondaryContact;
            set
            {
                _secondaryContact = value;
                SecondaryContactProvided = true;
            }
        }

        [JsonIgnore]
        public bool SecondaryContactProvided { get; private set; }

        [JsonProperty("secondaryContactDelay")]
        public int? SecondaryContactDelay
        {
            get => _secondaryContactDelay;
            set
            {
                _secondaryContactDelay = value;
                SecondaryContactDelayProvided = true;
            }
        }

        [JsonIgnore]
        public bool SecondaryContactDelayProvided { get; private set; }

        [JsonProperty("fuzzyLimits")]
        public bool? FuzzyLimits
        {
            get => _fuzzyLimits;
            set
            {
                _fuzzyLimits = value;
                FuzzyLimitsProvided = true;
            }
        }

        [JsonIgnore]
        public bool FuzzyLimitsProvided { get; private set; }

        [JsonProperty("fuzzy_Low")]
        public double? FuzzyLow
        {
            get => _fuzzyLow;
            set
            {
                _fuzzyLow = value;
                FuzzyLowProvided = true;
            }
        }

        [JsonIgnore]
        public bool FuzzyLowProvided { get; private set; }

        [JsonProperty("fuzzy_High")]
        public double? FuzzyHigh
        {
            get => _fuzzyHigh;
            set
            {
                _fuzzyHigh = value;
                FuzzyHighProvided = true;
            }
        }

        [JsonIgnore]
        public bool FuzzyHighProvided { get; private set; }
    }

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DeviceController : ApiController
    {


        [Route("getDeviceType")]
        [HttpGet]
        public List<tbl_DeviceType> getDeviceType()
        {
            try
            {
                omnioEntities en = new omnioEntities();
                
                return en.tbl_DeviceType.Where(x => x.ID > 0).ToList();
            }
            catch
            {
                return null;
            }
        }

        [Route("getDevices")]
        [HttpGet]
        public List<tbl_Devices> getDevices()
        {
            try
            {
                omnioEntities en = new omnioEntities();

                return en.tbl_Devices.Where(x => x.BriolerID == null).ToList();
            }
            catch
            {
                return null;
            }
        }



        [Route("addDevice")]
        [HttpPost]
        public bool addDevice(tbl_Devices device)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_Devices.AddOrUpdate(device);
                en.SaveChanges();

                return true;
            }
            catch
            {

                return false;
            }

            // return ByteArrToString(Encrypt(name));
        }

        [Route("updateDevice")]
        [HttpPost]
        public string updateDevice(tbl_Devices device)
        {
            try
            {
                string[] prevDeviceID = device.DeviceID.Split(':');
                
                omnioEntities en = new omnioEntities();
                tbl_Devices prevDevice = en.tbl_Devices.Find(prevDeviceID[0]);
                prevDevice.BriolerID = null;
                prevDevice.Name = null;
                prevDevice.Description = null;

                en.SaveChanges();

                device.DeviceID = prevDeviceID[1];

                en.tbl_Devices.AddOrUpdate(device);
                en.SaveChanges();

                return prevDeviceID[0];
            }
            catch
            {

                return "";
            }

            // return ByteArrToString(Encrypt(name));
        }

        [Route("updateDevice/{deviceDataID}")]
        [HttpPost]
        public bool updateDevice(long deviceDataID, DeviceLimitUpdateRequest limits)
        {
            if (limits == null)
            {
                return false;
            }

            if (limits.DeviceDataIDProvided)
            {
                if (!limits.DeviceDataID.HasValue || limits.DeviceDataID.Value != deviceDataID)
                {
                    return false;
                }
            }

            try
            {
                using (omnioEntities en = new omnioEntities())
                {
                    List<string> assignments = new List<string>();
                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (limits.HighLimitProvided)
                    {
                        assignments.Add("HighLimit = @HighLimit");
                        parameters.Add(CreateSqlParameter("@HighLimit", SqlDbType.Float, limits.HighLimit));
                    }

                    if (limits.LowLimitProvided)
                    {
                        assignments.Add("LowLimit = @LowLimit");
                        parameters.Add(CreateSqlParameter("@LowLimit", SqlDbType.Float, limits.LowLimit));
                    }

                    if (limits.ThresholdEnabledProvided)
                    {
                        assignments.Add("ThresholdEnabled = @ThresholdEnabled");
                        parameters.Add(CreateSqlParameter("@ThresholdEnabled", SqlDbType.Bit, limits.ThresholdEnabled));
                    }

                    if (limits.PrimaryContactProvided)
                    {
                        assignments.Add("PrimaryContact = @PrimaryContact");
                        parameters.Add(CreateSqlParameter("@PrimaryContact", SqlDbType.VarChar, limits.PrimaryContact, 100));
                    }

                    if (limits.SecondaryContactProvided)
                    {
                        assignments.Add("SecondaryContact = @SecondaryContact");
                        parameters.Add(CreateSqlParameter("@SecondaryContact", SqlDbType.VarChar, limits.SecondaryContact, 100));
                    }

                    if (limits.SecondaryContactDelayProvided)
                    {
                        assignments.Add("SecondaryContactDelay = @SecondaryContactDelay");
                        parameters.Add(CreateSqlParameter("@SecondaryContactDelay", SqlDbType.Int, limits.SecondaryContactDelay));
                    }

                    if (limits.FuzzyLimitsProvided)
                    {
                        assignments.Add("FuzzyLimits = @FuzzyLimits");
                        parameters.Add(CreateSqlParameter("@FuzzyLimits", SqlDbType.Bit, limits.FuzzyLimits));
                    }

                    if (limits.FuzzyLowProvided)
                    {
                        assignments.Add("Fuzzy_Low = @FuzzyLow");
                        parameters.Add(CreateSqlParameter("@FuzzyLow", SqlDbType.Float, limits.FuzzyLow));
                    }

                    if (limits.FuzzyHighProvided)
                    {
                        assignments.Add("Fuzzy_High = @FuzzyHigh");
                        parameters.Add(CreateSqlParameter("@FuzzyHigh", SqlDbType.Float, limits.FuzzyHigh));
                    }

                    if (!assignments.Any())
                    {
                        return true;
                    }

                    string updateSql = $"UPDATE tbl_DeviceLimits SET {string.Join(", ", assignments)} WHERE DeviceDataID = @DeviceDataID";

                    parameters.Add(CreateSqlParameter("@DeviceDataID", SqlDbType.BigInt, deviceDataID));

                    int rowsAffected = en.Database.ExecuteSqlCommand(updateSql, parameters.ToArray());

                    return rowsAffected > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        //This does not delete the device but just remove the link to the brioler house;
        [Route("deleteDevice")]
        [HttpPost]
        public bool deleteDevice(tbl_Devices device)
        {
            try
            {
                //string[] prevDeviceID = device.DeviceID.Split(':');

                omnioEntities en = new omnioEntities();
                tbl_Devices Device = en.tbl_Devices.Find(device.DeviceID);
                Device.Name = null;
                Device.BriolerID = null;
                Device.Description = null;

                en.SaveChanges();

              

                return true;
            }
            catch
            {

                return false;
            }

            // return ByteArrToString(Encrypt(name));
        }

        //This does not delete the device but just remove the link to the brioler house;
        [Route("getDeviceLimits/{id}")]
        [HttpGet]
        public List<tbl_DeviceLimits> getDeviceLimits(string id)
        {
            try
            {
                //string[] prevDeviceID = device.DeviceID.Split(':');

                omnioEntities en = new omnioEntities();
                List<tbl_DeviceLimits> Device = en.tbl_DeviceLimits.Where(x => x.DeviceID == id).ToList();

                return Device;
            }
            catch
            {

                return null;
            }

            // return ByteArrToString(Encrypt(name));
        }

        private static SqlParameter CreateSqlParameter(string name, SqlDbType type, object value, int? size = null)
        {
            SqlParameter parameter = new SqlParameter(name, type);

            if (size.HasValue)
            {
                parameter.Size = size.Value;
            }

            parameter.Value = value ?? DBNull.Value;
            parameter.IsNullable = true;

            return parameter;
        }
    }
}
