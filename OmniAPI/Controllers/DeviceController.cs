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
<<<<<<< HEAD
=======
        [JsonProperty("deviceDataID")]
        public long? DeviceDataID { get; set; }

>>>>>>> origin/codex/add-post-endpoint-for-/updatedevice/devicedataid-xe5ak7
        [JsonProperty("highLimit")]
        public double? HighLimit { get; set; }

        [JsonProperty("lowLimit")]
        public double? LowLimit { get; set; }

        [JsonProperty("thresholdEnabled")]
        public bool? ThresholdEnabled { get; set; }

        [JsonProperty("primaryContact")]
        public string PrimaryContact { get; set; }

        [JsonProperty("secondaryContact")]
        public string SecondaryContact { get; set; }

        [JsonProperty("secondaryContactDelay")]
        public int? SecondaryContactDelay { get; set; }

        [JsonProperty("fuzzyLimits")]
        public bool? FuzzyLimits { get; set; }
<<<<<<< HEAD
=======

        [JsonProperty("fuzzy_Low")]
        public double? FuzzyLow { get; set; }

        [JsonProperty("fuzzy_High")]
        public double? FuzzyHigh { get; set; }
>>>>>>> origin/codex/add-post-endpoint-for-/updatedevice/devicedataid-xe5ak7
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

<<<<<<< HEAD
=======
            if (limits.DeviceDataID.HasValue && limits.DeviceDataID.Value != deviceDataID)
            {
                return false;
            }

>>>>>>> origin/codex/add-post-endpoint-for-/updatedevice/devicedataid-xe5ak7
            try
            {
                using (omnioEntities en = new omnioEntities())
                {
                    const string updateSql = @"UPDATE tbl_DeviceLimits
<<<<<<< HEAD
SET HighLimit = @HighLimit,
    LowLimit = @LowLimit,
    ThresholdEnabled = @ThresholdEnabled,
    PrimaryContact = @PrimaryContact,
    SecondaryContact = @SecondaryContact,
    SecondaryContactDelay = @SecondaryContactDelay,
    FuzzyLimits = @FuzzyLimits
=======
SET HighLimit = COALESCE(@HighLimit, HighLimit),
    LowLimit = COALESCE(@LowLimit, LowLimit),
    ThresholdEnabled = COALESCE(@ThresholdEnabled, ThresholdEnabled),
    PrimaryContact = COALESCE(@PrimaryContact, PrimaryContact),
    SecondaryContact = COALESCE(@SecondaryContact, SecondaryContact),
    SecondaryContactDelay = COALESCE(@SecondaryContactDelay, SecondaryContactDelay),
    FuzzyLimits = COALESCE(@FuzzyLimits, FuzzyLimits),
    Fuzzy_Low = COALESCE(@FuzzyLow, Fuzzy_Low),
    Fuzzy_High = COALESCE(@FuzzyHigh, Fuzzy_High)
>>>>>>> origin/codex/add-post-endpoint-for-/updatedevice/devicedataid-xe5ak7
WHERE DeviceDataID = @DeviceDataID";

                    SqlParameter[] parameters = new[]
                    {
                        CreateSqlParameter("@HighLimit", SqlDbType.Float, limits.HighLimit),
                        CreateSqlParameter("@LowLimit", SqlDbType.Float, limits.LowLimit),
                        CreateSqlParameter("@ThresholdEnabled", SqlDbType.Bit, limits.ThresholdEnabled),
                        CreateSqlParameter("@PrimaryContact", SqlDbType.VarChar, limits.PrimaryContact, 100),
                        CreateSqlParameter("@SecondaryContact", SqlDbType.VarChar, limits.SecondaryContact, 100),
                        CreateSqlParameter("@SecondaryContactDelay", SqlDbType.Int, limits.SecondaryContactDelay),
                        CreateSqlParameter("@FuzzyLimits", SqlDbType.Bit, limits.FuzzyLimits),
<<<<<<< HEAD
=======
                        CreateSqlParameter("@FuzzyLow", SqlDbType.Float, limits.FuzzyLow),
                        CreateSqlParameter("@FuzzyHigh", SqlDbType.Float, limits.FuzzyHigh),
>>>>>>> origin/codex/add-post-endpoint-for-/updatedevice/devicedataid-xe5ak7
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
