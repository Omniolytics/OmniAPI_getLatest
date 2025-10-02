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

        public bool DeviceDataIDProvided { get; private set; }
        public bool HighLimitProvided { get; private set; }
        public bool LowLimitProvided { get; private set; }
        public bool ThresholdEnabledProvided { get; private set; }
        public bool PrimaryContactProvided { get; private set; }
        public bool SecondaryContactProvided { get; private set; }
        public bool SecondaryContactDelayProvided { get; private set; }
        public bool FuzzyLimitsProvided { get; private set; }
        public bool FuzzyLowProvided { get; private set; }
        public bool FuzzyHighProvided { get; private set; }

        [JsonProperty("deviceDataID")]
        public long? DeviceDataID
        {
            get => _deviceDataID;
            set
            {
                DeviceDataIDProvided = true;
                _deviceDataID = value;
            }
        }

        [JsonProperty("highLimit")]
        public double? HighLimit
        {
            get => _highLimit;
            set
            {
                HighLimitProvided = true;
                _highLimit = value;
            }
        }

        [JsonProperty("lowLimit")]
        public double? LowLimit
        {
            get => _lowLimit;
            set
            {
                LowLimitProvided = true;
                _lowLimit = value;
            }
        }

        [JsonProperty("thresholdEnabled")]
        public bool? ThresholdEnabled
        {
            get => _thresholdEnabled;
            set
            {
                ThresholdEnabledProvided = true;
                _thresholdEnabled = value;
            }
        }

        [JsonProperty("primaryContact")]
        public string PrimaryContact
        {
            get => _primaryContact;
            set
            {
                PrimaryContactProvided = true;
                _primaryContact = value;
            }
        }

        [JsonProperty("secondaryContact")]
        public string SecondaryContact
        {
            get => _secondaryContact;
            set
            {
                SecondaryContactProvided = true;
                _secondaryContact = value;
            }
        }

        [JsonProperty("secondaryContactDelay")]
        public int? SecondaryContactDelay
        {
            get => _secondaryContactDelay;
            set
            {
                SecondaryContactDelayProvided = true;
                _secondaryContactDelay = value;
            }
        }

        [JsonProperty("fuzzyLimits")]
        public bool? FuzzyLimits
        {
            get => _fuzzyLimits;
            set
            {
                FuzzyLimitsProvided = true;
                _fuzzyLimits = value;
            }
        }

        [JsonProperty("fuzzy_Low")]
        public double? FuzzyLow
        {
            get => _fuzzyLow;
            set
            {
                FuzzyLowProvided = true;
                _fuzzyLow = value;
            }
        }

        [JsonProperty("fuzzy_High")]
        public double? FuzzyHigh
        {
            get => _fuzzyHigh;
            set
            {
                FuzzyHighProvided = true;
                _fuzzyHigh = value;
            }
        }
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

            if (limits.DeviceDataIDProvided && limits.DeviceDataID.HasValue && limits.DeviceDataID.Value != deviceDataID)
            {
                return false;
            }

            try
            {
                using (omnioEntities en = new omnioEntities())
                {
                    const string updateSql = @"UPDATE tbl_DeviceLimits
SET HighLimit = CASE WHEN @HighLimitProvided = 1 THEN @HighLimit ELSE HighLimit END,
    LowLimit = CASE WHEN @LowLimitProvided = 1 THEN @LowLimit ELSE LowLimit END,
    ThresholdEnabled = CASE WHEN @ThresholdEnabledProvided = 1 THEN @ThresholdEnabled ELSE ThresholdEnabled END,
    PrimaryContact = CASE WHEN @PrimaryContactProvided = 1 THEN @PrimaryContact ELSE PrimaryContact END,
    SecondaryContact = CASE WHEN @SecondaryContactProvided = 1 THEN @SecondaryContact ELSE SecondaryContact END,
    SecondaryContactDelay = CASE WHEN @SecondaryContactDelayProvided = 1 THEN @SecondaryContactDelay ELSE SecondaryContactDelay END,
    FuzzyLimits = CASE WHEN @FuzzyLimitsProvided = 1 THEN @FuzzyLimits ELSE FuzzyLimits END,
    Fuzzy_Low = CASE WHEN @FuzzyLowProvided = 1 THEN @FuzzyLow ELSE Fuzzy_Low END,
    Fuzzy_High = CASE WHEN @FuzzyHighProvided = 1 THEN @FuzzyHigh ELSE Fuzzy_High END
WHERE DeviceDataID = @DeviceDataID";

                    SqlParameter[] parameters = new[]
                    {
                        CreateSqlParameter("@HighLimitProvided", SqlDbType.Bit, limits.HighLimitProvided),
                        CreateSqlParameter("@HighLimit", SqlDbType.Float, limits.HighLimit),
                        CreateSqlParameter("@LowLimitProvided", SqlDbType.Bit, limits.LowLimitProvided),
                        CreateSqlParameter("@LowLimit", SqlDbType.Float, limits.LowLimit),
                        CreateSqlParameter("@ThresholdEnabledProvided", SqlDbType.Bit, limits.ThresholdEnabledProvided),
                        CreateSqlParameter("@ThresholdEnabled", SqlDbType.Bit, limits.ThresholdEnabled),
                        CreateSqlParameter("@PrimaryContactProvided", SqlDbType.Bit, limits.PrimaryContactProvided),
                        CreateSqlParameter("@PrimaryContact", SqlDbType.VarChar, limits.PrimaryContact, 100),
                        CreateSqlParameter("@SecondaryContactProvided", SqlDbType.Bit, limits.SecondaryContactProvided),
                        CreateSqlParameter("@SecondaryContact", SqlDbType.VarChar, limits.SecondaryContact, 100),
                        CreateSqlParameter("@SecondaryContactDelayProvided", SqlDbType.Bit, limits.SecondaryContactDelayProvided),
                        CreateSqlParameter("@SecondaryContactDelay", SqlDbType.Int, limits.SecondaryContactDelay),
                        CreateSqlParameter("@FuzzyLimitsProvided", SqlDbType.Bit, limits.FuzzyLimitsProvided),
                        CreateSqlParameter("@FuzzyLimits", SqlDbType.Bit, limits.FuzzyLimits),
                        CreateSqlParameter("@FuzzyLowProvided", SqlDbType.Bit, limits.FuzzyLowProvided),
                        CreateSqlParameter("@FuzzyLow", SqlDbType.Float, limits.FuzzyLow),
                        CreateSqlParameter("@FuzzyHighProvided", SqlDbType.Bit, limits.FuzzyHighProvided),
                        CreateSqlParameter("@FuzzyHigh", SqlDbType.Float, limits.FuzzyHigh),
                        CreateSqlParameter("@DeviceDataID", SqlDbType.BigInt, deviceDataID)
                    };

                    int rowsAffected = en.Database.ExecuteSqlCommand(updateSql, parameters);

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
