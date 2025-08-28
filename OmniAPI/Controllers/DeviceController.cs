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

namespace OmniAPI.Controllers
{

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
    }
}
