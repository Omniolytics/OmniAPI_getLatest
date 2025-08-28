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
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Twilio.AspNet.Common;
using Newtonsoft.Json;

namespace OmniAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EventController : ApiController
    {

        [Route("SMSResult")]
        [HttpPost]
        public string SMSResult(SmsRequest request)
        {

            string json = JsonConvert.SerializeObject(request);

            //write string to file
            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"C:\Downloads\path.txt", json);

            var requestBody = request.Body;

            omnioEntities en = new omnioEntities();
           tbl_activities act = en.tbl_activities.First(x => x.sid_1 == request.SmsSid || x.sid_2 == request.SmsSid || x.sid_3 == request.SmsSid  || x.sid_4 == request.SmsSid);

            if (act.sid_1  != "0")
            {
                act.response_1 = request.Body;
            }

            en.SaveChanges();
          

            return "done";
        }


        [Route("getCullEvents")]
        [HttpGet]
        // [Authorize]
        public List<tbl_CullsEvent> getCullEvents()
        {
            try
            {

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;
                List<tbl_CullsEvent> CullsEvents = en.tbl_CullsEvent.Where(x => x.ID > 0).ToList();

                return CullsEvents;
            }
            catch (Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }


        [Route("AddUpdateCullEvents")]
        [HttpPost]
        // [Authorize]
        public List<tbl_CullsEvent> AddUpdateCullEvents(tbl_CullsEvent cullEvent)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_CullsEvent.AddOrUpdate(cullEvent);
                en.SaveChanges();

                List<tbl_CullsEvent> CullsEvents = en.tbl_CullsEvent.Where(x => x.ID > 0).ToList();
                return CullsEvents;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                return null;
            }
        }

        [Route("removeCullEvents")]
        [HttpPost]
        // [Authorize]
        public bool removeCullEvents(tbl_CullsEvent cullEvent)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                tbl_CullsEvent w = en.tbl_CullsEvent.Find(cullEvent.ID);

                en.tbl_CullsEvent.Remove(w);
                en.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                return false;
            }
        }




        [Route("getMortEvents")]
        [HttpGet]
        // [Authorize]
        public List<tbl_MortalitiesEvent> getMortEvents()
        {
            try
            {

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;
                List<tbl_MortalitiesEvent> MortsEvents = en.tbl_MortalitiesEvent.Where(x => x.ID > 0).ToList();

                return MortsEvents;
            }
            catch (Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }


        [Route("AddUpdateMortEvents")]
        [HttpPost]
        // [Authorize]
        public List<tbl_MortalitiesEvent> AddUpdateMortEvents(tbl_MortalitiesEvent cullEvent)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_MortalitiesEvent.AddOrUpdate(cullEvent);
                en.SaveChanges();

                List<tbl_MortalitiesEvent> MortsEvents = en.tbl_MortalitiesEvent.Where(x => x.ID > 0).ToList();
                return MortsEvents;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                return null;
            }
        }

        [Route("removeMortEvents")]
        [HttpPost]
        // [Authorize]
        public bool removeMortEvents(tbl_MortalitiesEvent cullEvent)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                tbl_MortalitiesEvent w = en.tbl_MortalitiesEvent.Find(cullEvent.ID);

                en.tbl_MortalitiesEvent.Remove(w);
                en.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                return false;
            }
        }
    }
}
