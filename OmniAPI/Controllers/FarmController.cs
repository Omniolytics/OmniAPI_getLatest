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
using System.Data.Entity.Migrations;
using OmniAPI.Classes;
using Newtonsoft.Json;

namespace OmniAPI.Controllers
{
    public class BroilerDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? farmID { get; set; }
        public int? NoOfBirds { get; set; }
    }

    public class DeviceDetailsDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? BroilerID { get; set; }
        public string BriolerName { get; set; }
        public string BriolerDescription { get; set; }
        public string FarmName { get; set; }
        public string FarmDescription { get; set; }
        public string DeviceID { get; set; }
        public string DeviceType { get; set; }
        public DateTime? TImestamp { get; set; }
        public string Unit { get; set; }
        [JsonProperty("device_limits", NullValueHandling = NullValueHandling.Ignore)]
        public List<DeviceLimitDto> DeviceLimits { get; set; }
    }

    public class DeviceLimitDto
    {
        [JsonProperty("DeviceDataID")]
        public long DeviceDataID { get; set; }

        [JsonProperty("ValueType")]
        public string ValueType { get; set; }

        [JsonProperty("HighLimit")]
        public double? HighLimit { get; set; }

        [JsonProperty("LowLimit")]
        public double? LowLimit { get; set; }

        [JsonProperty("ThresholdEnabled")]
        public bool? ThresholdEnabled { get; set; }

        [JsonProperty("PrimaryContact")]
        public string PrimaryContact { get; set; }

        [JsonProperty("SecondaryContact")]
        public string SecondaryContact { get; set; }

        [JsonProperty("SecondaryContactDelay")]
        public int? SecondaryContactDelay { get; set; }

        [JsonProperty("FuzzyLimits")]
        public bool? FuzzyLimits { get; set; }

        [JsonProperty("Fuzzy_Low")]
        public double? FuzzyLow { get; set; }

        [JsonProperty("Fuzzy_High")]
        public double? FuzzyHigh { get; set; }
    }

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FarmController : ApiController
    {
        private byte[] Key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
        private byte[] Vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 252, 112, 79, 32, 114, 156 };

        private ICryptoTransform EncryptorTransform, DecryptorTransform;
        private System.Text.UTF8Encoding UTFEncoder;


       



        [Route("Farmer/addFarmer/")]
         [HttpPost]
        public bool addFarmer(tbl_Users farmer)
        {
            try
            {
                RijndaelManaged rm = new RijndaelManaged();

                //Create an encryptor and a decryptor using our encryption method, key, and vector.
                EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
                DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

                //Used to translate bytes to text and vice versa
                UTFEncoder = new System.Text.UTF8Encoding();

                tbl_Users user = new tbl_Users();

                user.Name = ByteArrToString(Encrypt(farmer.Name));
                user.Surname = ByteArrToString(Encrypt(farmer.Surname));
                user.Email = ByteArrToString(Encrypt(farmer.Email));
                user.ContactNumber = ByteArrToString(Encrypt(farmer.ContactNumber));
                user.Username = ByteArrToString(Encrypt(farmer.Username));
                user.Password = "";
                user.UserTypeID = 2;
                user.Registered = false;
                user.UniqueID = Guid.NewGuid().ToString();

                omnioEntities en = new omnioEntities();
        //        en.tbl_Users.Add(user);
            //    en.SaveChanges();

                if (en.tbl_UsersAdditional.Any(x => x.Email == user.Email) && en.tbl_Users.Any(x => x.Email == user.Email))
                {
                    return false;
                }
                else
                {

                    en.tbl_Users.Add(user);
                    en.SaveChanges();

                    sendEmail(farmer.Email, user.UniqueID, farmer.Name, farmer.Surname);
                }


           

             

                return true;
            }
            catch
            {

                return false;
            }

           // return ByteArrToString(Encrypt(name));
        }

        [Route("Farmer/editFarmer/")]
        [HttpPost]
        public bool editFarmer(tbl_Users farmer)
        {
            try
            {
                RijndaelManaged rm = new RijndaelManaged();

                //Create an encryptor and a decryptor using our encryption method, key, and vector.
                EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
                DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

                //Used to translate bytes to text and vice versa
                UTFEncoder = new System.Text.UTF8Encoding();

                omnioEntities en = new omnioEntities();
                tbl_Users user = en.tbl_Users.Find(farmer.ID);

                Encryption enc = new Encryption();
                string username = enc.ByteArrToString(enc.Encrypt(farmer.Email));
                string pass = enc.ByteArrToString(enc.Encrypt("12345"));



             
                if (user.Email == enc.ByteArrToString(enc.Encrypt(farmer.Email)))
                {

                    user.Name = enc.ByteArrToString(enc.Encrypt(farmer.Name));
                    user.Surname = enc.ByteArrToString(enc.Encrypt(farmer.Surname ?? "" ));
                    user.Email = enc.ByteArrToString(enc.Encrypt(farmer.Email));
                    user.ContactNumber = enc.ByteArrToString(enc.Encrypt(farmer.ContactNumber ?? ""));

                    //user.Registered = farmer.Registered;

                    if(farmer.Password != "")
                    {
                        user.Password   = enc.ByteArrToString(enc.Encrypt(farmer.Password));
                    }


                    en.SaveChanges();
                }
                else
                {
                    string email = enc.ByteArrToString(enc.Encrypt(farmer.Email));

                    if (en.tbl_UsersAdditional.Any(x => x.Email == email) && en.tbl_Users.Any(x => x.Email == email))
                    {
                        return false;

                    }
                    else
                    {

                        user.Name = enc.ByteArrToString(enc.Encrypt(farmer.Name));
                        user.Surname = enc.ByteArrToString(enc.Encrypt(farmer.Surname));
                        user.Email = enc.ByteArrToString(enc.Encrypt(farmer.Email));
                        user.ContactNumber = enc.ByteArrToString(enc.Encrypt(farmer.ContactNumber));

                        user.Registered = false; // have to reset the 

                        if (farmer.Password != "")
                        {
                            user.Password = enc.ByteArrToString(enc.Encrypt(farmer.Password));
                        }


                        sendEmail(farmer.Email, user.UniqueID, farmer.Name, farmer.Surname);
                        en.SaveChanges();
                    }
                }



              /*  user.Name = ByteArrToString(Encrypt(farmer.Name));
                user.Surname = ByteArrToString(Encrypt(farmer.Surname));
                user.Email = ByteArrToString(Encrypt(farmer.Email));
                user.ContactNumber = ByteArrToString(Encrypt(farmer.ContactNumber));
              
                user.Registered = farmer.Registered;
                      
            
                en.SaveChanges();*/

               

                return true;
            }
            catch
            {

                return false;
            }

            // return ByteArrToString(Encrypt(name));
        }

        [Route("Farmer/removeFarmer/")]
        [HttpPost]
        public bool removeFarmer(tbl_Users farmer)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_Users user = en.tbl_Users.Find(farmer.ID);
                en.tbl_Users.Remove(user);
                
                en.SaveChanges();

                return true;
            }
            catch(Exception e)
            {

                return false;
            }

            // return ByteArrToString(Encrypt(name));
        }

        [Route("Farmer/addFarm/")]
        [HttpPost]
        public int addFarm(tbl_Farm farm)
        {
            try
            {
                omnioEntities en = new omnioEntities();
               tbl_Farm newFarm = en.tbl_Farm.Add(farm);
                en.SaveChanges();

                return farm.ID;
            }
            catch
            {

                return 0;
            }

            // return ByteArrToString(Encrypt(name));
        }

        [Route("Farmer/updateFarm/")]
        [HttpPost]
        public int updateFarm(tbl_Farm farm)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_Farm.AddOrUpdate(farm);


                en.SaveChanges();

                return farm.ID;
            }
            catch
            {

                return 0;
            }

            // return ByteArrToString(Encrypt(name));
        }

        [Route("Farm/removeFarm/")]
        [HttpPost]
        public bool removeFarm(tbl_Farm farm)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_Farm removeFarm = en.tbl_Farm.Find(farm.ID);
                en.tbl_Farm.Remove(removeFarm);

                en.SaveChanges();

                return false;
            }
            catch(Exception e)
            {

                return true;
            }

            // return ByteArrToString(Encrypt(name));
        }




        [Route("Farmer/addBrioler/")]
        [HttpPost]
        public int addBrioler(tbl_Briolers brioler)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_Briolers newBrioler = en.tbl_Briolers.Add(brioler);
                en.SaveChanges();

                return newBrioler.ID;
            }
            catch
            {

                return 0;
            }

            // return ByteArrToString(Encrypt(name));
        }

        [Route("Farmer/updateBrioler/")]
        [HttpPost]
        public int updateBrioler(tbl_Briolers brioler)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_Briolers.AddOrUpdate(brioler);


                en.SaveChanges();

                return brioler.ID;
            }
            catch
            {

                return 0;
            }

            // return ByteArrToString(Encrypt(name));
        }

        [Route("Farmer/deleteBrioler/")]
        [HttpPost]
        public bool deleteBrioler(tbl_Briolers brioler)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_Briolers brio = en.tbl_Briolers.Find(brioler.ID);
                en.tbl_Briolers.Remove(brio);


                en.SaveChanges();

                return true;
            }
            catch
            {

                return false;
            }

            // return ByteArrToString(Encrypt(name));
        }

        [Route("getAllFarms")]
        [HttpGet]
        public List<tbl_Farm> getAllFarms()
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                return en.tbl_Farm.ToList();
            }
            catch
            {
                return null;
            }
        }

        [Route("getAllBroilers")]
        [HttpGet]
        public List<BroilerDto> getAllBroilers()
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                return en.tbl_Briolers.Select(x => new BroilerDto
                {
                    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                    farmID = x.farmID,
                    NoOfBirds = x.NoOfBirds
                }).ToList();
            }
            catch
            {
                return null;
            }
        }

        [Route("getBroilerData")]
        [HttpGet]
        public List<tbl_BroilerInfo> getBroilerData()
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                return en.tbl_BroilerInfo.ToList();
            }
            catch
            {
                return null;
            }
        }

        [Route("updateBroilerData")]
        [HttpPost]
        public List<tbl_BroilerInfo> updateBroilerData(tbl_BroilerInfo info)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_BroilerInfo.AddOrUpdate(info);
                en.SaveChanges();
                return en.tbl_BroilerInfo.ToList();
            }
            catch
            {
                return null;
            }
        }

        [Route("removeBroilerData")]
        [HttpPost]
        public List<tbl_BroilerInfo> removeBroilerData(tbl_BroilerInfo info)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_BroilerInfo d = en.tbl_BroilerInfo.Find(info.id);

                en.tbl_BroilerInfo.Remove(d);


                en.SaveChanges();
                return en.tbl_BroilerInfo.ToList();
            }
            catch
            {
                return null;
            }
        }



        [Route("getFarms")]
        [HttpGet]
        public List<tbl_Users> getFarms()
        {
            try
            {
                RijndaelManaged rm = new RijndaelManaged();

                //Create an encryptor and a decryptor using our encryption method, key, and vector.
                EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
                DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

                //Used to translate bytes to text and vice versa
                UTFEncoder = new System.Text.UTF8Encoding();


                omnioEntities en = new omnioEntities();
             //   en.Configuration.LazyLoadingEnabled = false;
                List<tbl_Users> farmers = en.tbl_Users.Where(x => x.ID > 0).ToList();

                List<tbl_Users> farmersDecode = new List<tbl_Users>();

                foreach(tbl_Users farmer in farmers)
                {
                    if (farmer.Username != "michael.samson@omniolytics.co.uk")
                    {
                        tbl_Users decode = new tbl_Users();
                        decode.ID = farmer.ID;
                        decode.Name = Decrypt(StrToByteArray(farmer.Name));
                        decode.Surname = Decrypt(StrToByteArray(farmer.Surname));
                        decode.Email = Decrypt(StrToByteArray(farmer.Email));
                        decode.ContactNumber = Decrypt(StrToByteArray(farmer.ContactNumber));

                        decode.Registered = farmer.Registered;
                        decode.Username = farmer.Username;
                        decode.tbl_Farm = farmer.tbl_Farm;

                        farmersDecode.Add(decode);
                      
                    }

                }

                return farmersDecode;
            }
            catch
            {
                return null;
            }
        }


        [Route("getDeviceDetails")]
        [HttpGet]
        public List<vw_DeviceDetails> getDeviceDetails()
        {
            try
            {
                omnioEntities en = new omnioEntities();
               List<vw_DeviceDetails> details = en.vw_DeviceDetails.Where(x => x.DeviceID != "").ToList();


                List<vw_DeviceDetails> detailsList = new List<vw_DeviceDetails>();

                foreach (vw_DeviceDetails dd in details)
                {
                    if (detailsList.Exists(x => x.Name == dd.Name))
                    {

                    }
                    else
                    {

                        DateTime latest = DateTime.Now;
                        TimeSpan tp = latest.Subtract(dd.TImestamp.Value);

                        if (tp.TotalHours > 2)
                            dd.Unit = "Offline";
                        else
                            dd.Unit = "Online";


                        detailsList.Add(dd);
                    }


                }


                


                return detailsList;
            }
            catch
            {
                return null;
            }
        }

        [Route("getDeviceDetails/{id}")]
        [HttpGet]
        public List<DeviceDetailsDto> getDeviceDetails(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();

                var deviceDetails = (from detail in en.vw_DeviceDetails
                                     join device in en.tbl_Devices on detail.DeviceID equals device.DeviceID
                                     where detail.DeviceID != "" && device.BriolerID == id
                                     select new { Detail = detail, BroilerID = device.BriolerID }).ToList();

                List<DeviceDetailsDto> detailsList = new List<DeviceDetailsDto>();

                foreach (var item in deviceDetails)
                {
                    vw_DeviceDetails detail = item.Detail;

                    if (detailsList.Exists(x => x.Name == detail.Name))
                    {
                        continue;
                    }

                    string status = "Offline";

                    if (detail.TImestamp.HasValue)
                    {
                        DateTime latest = DateTime.Now;
                        TimeSpan tp = latest.Subtract(detail.TImestamp.Value);

                        if (tp.TotalHours <= 2)
                        {
                            status = "Online";
                        }
                    }

                    DeviceDetailsDto dto = new DeviceDetailsDto();

                    dto.Name = detail.Name;
                    dto.Description = detail.Description;
                    dto.BroilerID = item.BroilerID;
                    dto.BriolerName = detail.BriolerName;
                    dto.BriolerDescription = detail.BriolerDescription;
                    dto.FarmName = detail.FarmName;
                    dto.FarmDescription = detail.FarmDescription;
                    dto.DeviceID = detail.DeviceID;
                    dto.DeviceType = detail.DeviceType;
                    dto.TImestamp = detail.TImestamp;
                    dto.Unit = status;

                    detailsList.Add(dto);
                }

                return detailsList;
            }
            catch
            {
                return null;
            }
        }

        [Route("getDeviceDetails/{id}/{deviceID}")]
        [HttpGet]
        public List<DeviceDetailsDto> getDeviceDetails(int id, string deviceID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(deviceID))
                {
                    return new List<DeviceDetailsDto>();
                }

                string trimmedDeviceId = deviceID.Trim();

                using (omnioEntities en = new omnioEntities())
                {
                    var deviceDetails = (from detail in en.vw_DeviceDetails
                                         join device in en.tbl_Devices on detail.DeviceID equals device.DeviceID
                                         where detail.DeviceID != "" && device.BriolerID == id && detail.DeviceID == trimmedDeviceId
                                         select new { Detail = detail, BroilerID = device.BriolerID }).ToList();

                    List<DeviceDetailsDto> detailsList = new List<DeviceDetailsDto>();

                    foreach (var item in deviceDetails)
                    {
                        vw_DeviceDetails detail = item.Detail;

                        if (detailsList.Exists(x => x.DeviceID == detail.DeviceID))
                        {
                            continue;
                        }

                        string status = "Offline";

                        if (detail.TImestamp.HasValue)
                        {
                            DateTime latest = DateTime.Now;
                            TimeSpan tp = latest.Subtract(detail.TImestamp.Value);

                            if (tp.TotalHours <= 2)
                            {
                                status = "Online";
                            }
                        }

                        DeviceDetailsDto dto = new DeviceDetailsDto
                        {
                            Name = detail.Name,
                            Description = detail.Description,
                            BroilerID = item.BroilerID,
                            BriolerName = detail.BriolerName,
                            BriolerDescription = detail.BriolerDescription,
                            FarmName = detail.FarmName,
                            FarmDescription = detail.FarmDescription,
                            DeviceID = detail.DeviceID,
                            DeviceType = detail.DeviceType,
                            TImestamp = detail.TImestamp,
                            Unit = status,
                            DeviceLimits = GetDeviceLimits(en, detail.DeviceID)
                        };

                        detailsList.Add(dto);
                    }

                    return detailsList;
                }
            }
            catch
            {
                return null;
            }
        }

        private List<DeviceLimitDto> GetDeviceLimits(omnioEntities context, string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return new List<DeviceLimitDto>();
            }

            string trimmedDeviceId = deviceId.Trim();

            const string deviceLimitsQuery = @"
SELECT
    d.ID AS DeviceDataID,
    d.ValueType,
    l.HighLimit,
    l.LowLimit,
    l.ThresholdEnabled,
    l.PrimaryContact,
    l.SecondaryContact,
    l.SecondaryContactDelay,
    l.FuzzyLimits,
    l.Fuzzy_Low AS FuzzyLow,
    l.Fuzzy_High AS FuzzyHigh
FROM tbl_DeviceDataDetail AS d
LEFT JOIN tbl_DeviceLimits AS l ON d.ID = l.DeviceDataID
WHERE d.DeviceID = @p0
ORDER BY d.ValueType";

            return context.Database.SqlQuery<DeviceLimitDto>(deviceLimitsQuery, trimmedDeviceId).ToList();
        }

        [Route("getBroilerDevices/{broilerID}")]
        [HttpGet]
        public List<string> getBroilerDevices(int broilerID)
        {
            try
            {
                omnioEntities en = new omnioEntities();

                return en.tbl_Devices
                         .Where(x => x.BriolerID == broilerID && x.DeviceID != null && x.DeviceID != "")
                         .Select(x => x.DeviceID)
                         .ToList();
            }
            catch
            {
                return null;
            }
        }

        [Route("getBriolerDetails/{briolerid}")]
        [HttpGet]
        public tbl_Briolers getBriolerDetails(int briolerid)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_Briolers details = en.tbl_Briolers.Find(briolerid);


                return details;
            }
            catch
            {
                return null;
            }
        }

        [Route("getMortalities/{broilerid}")]
        [HttpGet]
        public List<tbl_BriolerData> getMortalities(int broilerid)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                return en.tbl_BriolerData.Where(x => x.briolerID == broilerid).ToList();
            }
            catch
            {
                return null;
            }
        }

        [Route("getMortalities/{broilerid}/{cycleId}")]
        [HttpGet]
        public List<tbl_BriolerData> getMortalities(int broilerid, int cycleId)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                return en.tbl_BriolerData.Where(x => x.briolerID == broilerid && x.cycleId == cycleId).ToList();
            }
            catch
            {
                return null;
            }
        }



        [Route("sendMail/{id}")]
        [HttpGet]
        public bool sendMail(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_Users user = en.tbl_Users.Find(id);

                RijndaelManaged rm = new RijndaelManaged();

                //Create an encryptor and a decryptor using our encryption method, key, and vector.
                EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
                DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

                //Used to translate bytes to text and vice versa
                UTFEncoder = new System.Text.UTF8Encoding();


                tbl_Users farmersDecode = new tbl_Users();

             
                    tbl_Users decode = new tbl_Users();
                    decode.ID = user.ID;
                    decode.Name = Decrypt(StrToByteArray(user.Name));
                    decode.Surname = Decrypt(StrToByteArray(user.Surname));
                    decode.Email = Decrypt(StrToByteArray(user.Email));
                    decode.ContactNumber = Decrypt(StrToByteArray(user.ContactNumber));
                    
                    decode.Registered = user.Registered;
                    decode.Username = user.Username;


                     sendEmail(decode.Email , user.UniqueID, decode.Name, decode.Surname);

                return true;
            }
            catch(Exception e)
            {
                
                return false;
            }
        }

        public bool sendEmail(string address, string uniqueID , string name , string surname)
        {
            try
            {
                if (1==1)
                {
                    omnioEntities db = new omnioEntities();
                    tbl_Users users = new tbl_Users();

                 

                    MailMessage mail = new MailMessage();



                    // Specify sender and recipient options for the e-mail message.
                    mail.From = new MailAddress("support@omniofarm.com", "OmnioFarm-Registration");
                    mail.To.Add(new MailAddress(address, name));

                    //  report.ExportOptions.Email.RecipientName));

                    // Specify other e-mail options.
                    mail.Subject = "OmnioFarm - Registration";
                    mail.Body = "Thank you for successfully registering" + Environment.NewLine + " " + "http://158.175.95.226/index.html?id=" + uniqueID.ToString() + "#frm_Register";

                    // Send the e-mail message via the specified SMTP server.
                    SmtpClient smtp = new SmtpClient("smtp.office365.com");
                    smtp.Port = 587;
                    smtp.Host = "smtp.office365.com";
                    smtp.EnableSsl = true;
                    smtp.Timeout = 100000;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("support@omniofarm.com", "Omni0F@rm_01");
                    smtp.Send(mail);





                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }

        }


        public byte[] StrToByteArray(string str)
        {
            if (str.Length == 0)
                throw new Exception("Invalid string value in StrToByteArray");

            byte val;
            byte[] byteArr = new byte[str.Length / 3];
            int i = 0;
            int j = 0;
            do
            {
                val = byte.Parse(str.Substring(i, 3));
                byteArr[j++] = val;
                i += 3;
            }
            while (i < str.Length);
            return byteArr;
        }

        public string ByteArrToString(byte[] byteArr)
        {
            byte val;
            string tempStr = "";
            for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
            {
                val = byteArr[i];
                if (val < (byte)10)
                    tempStr += "00" + val.ToString();
                else if (val < (byte)100)
                    tempStr += "0" + val.ToString();
                else
                    tempStr += val.ToString();
            }
            return tempStr;
        }

        public byte[] Encrypt(string TextValue)
        {
            //Translates our text value into a byte array.
            Byte[] bytes = UTFEncoder.GetBytes(TextValue);

            //Used to stream the data in and out of the CryptoStream.
            MemoryStream memoryStream = new MemoryStream();

            /*
             * We will have to write the unencrypted bytes to the stream,
             * then read the encrypted result back from the stream.
             */
            #region Write the decrypted value to the encryption stream
            CryptoStream cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            #endregion

            #region Read encrypted value back out of the stream
            memoryStream.Position = 0;
            byte[] encrypted = new byte[memoryStream.Length];
            memoryStream.Read(encrypted, 0, encrypted.Length);
            #endregion

            //Clean up.
            cs.Close();
            memoryStream.Close();

            return encrypted;
        }

        public string Decrypt(byte[] EncryptedValue)
        {
            #region Write the encrypted value to the decryption stream
            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
            decryptStream.FlushFinalBlock();
            #endregion

            #region Read the decrypted value from the stream.
            encryptedStream.Position = 0;
            Byte[] decryptedBytes = new Byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();
            #endregion
            return UTFEncoder.GetString(decryptedBytes);
        }


    }
}
