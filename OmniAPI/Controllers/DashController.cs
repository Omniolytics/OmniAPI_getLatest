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



namespace OmniAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]


    public class DashController : ApiController
    {


        [Route("getFarmsForUser/{id}")]
        [HttpGet]
        // [Authorize]
        public List<FarmDto> getUser(int id)
        {
            try
            {
                Encryption ecn = new Encryption();

                omnioEntities en = new omnioEntities();
               // en.Configuration.LazyLoadingEnabled = false;

                List<FarmDto> farms = en.tbl_Farm.Where(x => x.UserID == id)
                    .Select(f => new FarmDto
                    {
                        ID = f.ID,
                        UserID = f.UserID,
                        Name = f.Name,
                        Description = f.Description,
                        tbl_Cycles = f.tbl_Cycles.ToList()
                    }).ToList();

                return farms;
            }
            catch(Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }

        

         [Route("updateBriolerLimits")]
        [HttpPost]
        // [Authorize]
        public bool updateBriolerLimits(tbl_DeviceLimits limits)
        {
            try
            {
                Encryption ecn = new Encryption();

                omnioEntities en = new omnioEntities();


                en.tbl_DeviceLimits.AddOrUpdate(limits);

                en.SaveChanges();


                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("updateBriolerLimitsArray")]
        [HttpPost]
        // [Authorize]
        public bool updateBriolerLimitsArray(List<tbl_DeviceLimits> limits)
        {
            try
            {
                Encryption ecn = new Encryption();

                omnioEntities en = new omnioEntities();

                foreach(tbl_DeviceLimits lim in limits)
                {
                    en.tbl_DeviceLimits.AddOrUpdate(lim);
                }


           

                en.SaveChanges();


                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("addUpdateBriolerData")]
        [HttpPost]
        // [Authorize]
        public bool addUpdateBriolerData(tbl_BriolerData data)
        {
            try
            {
                Encryption ecn = new Encryption();

              //  data.dateTime = data.dateTime.Value.ToLocalTime(); 

                omnioEntities en = new omnioEntities();


                en.tbl_BriolerData.AddOrUpdate(data);

                en.SaveChanges();


                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("updateMortalities")]
        [HttpPost]
        // [Authorize]
        public bool updateMortalities(tbl_BriolerData data)
        {
            try
            {
                Encryption ecn = new Encryption();

                omnioEntities en = new omnioEntities();

                if (data.ID > 0)
                {
                    tbl_BriolerData existing = en.tbl_BriolerData.Find(data.ID);
                    if (existing != null)
                    {
                        existing.briolerID = data.briolerID;
                        existing.fatalities = data.fatalities;
                        existing.dateTime = data.dateTime;
                        existing.culls = data.culls;
                        existing.eventCullsID = data.eventCullsID;
                        existing.eventMortalitiesID = data.eventMortalitiesID;
                    }
                    else
                    {
                        en.tbl_BriolerData.Add(data);
                    }
                }
                else
                {
                    en.tbl_BriolerData.Add(data);
                }

                en.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("removeBriolerData")]
        [HttpPost]
        // [Authorize]
        public bool removeBriolerData(tbl_BriolerData data)
        {
            try
            {
                Encryption ecn = new Encryption();

                omnioEntities en = new omnioEntities();
                tbl_BriolerData d = en.tbl_BriolerData.Find(data.ID);
                en.tbl_BriolerData.Remove(d);
                en.SaveChanges();


                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("removeMortalities")]
        [HttpPost]
        // [Authorize]
        public bool removeMortalities(tbl_BriolerData data)
        {
            try
            {
                Encryption ecn = new Encryption();

                omnioEntities en = new omnioEntities();
                tbl_BriolerData d = en.tbl_BriolerData.Find(data.ID);
                en.tbl_BriolerData.Remove(d);
                en.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }


        [Route("getFatalitiesDay")]
        [HttpPost]
        // [Authorize]
        public List<vw_briolerDataDay> getFatalitiesDay(info i)
        {
            try
            {
                DateTime start = DateTime.Parse(i.start);
                DateTime end = DateTime.Parse(i.end);

                Encryption ecn = new Encryption();

                omnioEntities en = new omnioEntities();

                List<vw_briolerDataDay> data = en.vw_briolerDataDay.AsNoTracking().Where(x => x.created >= start && x.created <= end && x.ID == i.id).ToList();


                return data;
            }
            catch
            {
                return null;
            }
        }


        // =================================================WEIGHTS--------------------------


        [Route("getBroilerWeights/{id}")]
        [HttpGet]
        // [Authorize]
        public List<tbl_Weights> getBroilerWeights(int id)
        {
            try
            {

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                List<tbl_Weights> weights = en.tbl_Weights.Where(x => x.BroilerID == id).ToList();


                return weights;
            }
            catch (Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }


        [Route("AddUpdateBroilerWeights")]
        [HttpPost]
        // [Authorize]
        public tbl_Weights AddUpdateBroilerWeights(tbl_Weights Weights)
        {
            try
            {

                Encryption ecn = new Encryption();

                //  Weights.dateTime = Weights.dateTime.Value.ToLocalTime();
                if (!Weights.WeightBack.HasValue)
                    Weights.WeightBack = 0;

                if (!Weights.WeightFront.HasValue)
                    Weights.WeightFront = 0;

                if (!Weights.WeightCenter.HasValue)
                    Weights.WeightCenter = 0;


                double ave = Weights.WeightBack.Value + Weights.WeightCenter.Value + Weights.WeightFront.Value;
                Weights.WeightAverage = Math.Round( ave/3,3);

                omnioEntities en = new omnioEntities();
                en.tbl_Weights.AddOrUpdate(Weights);
                en.SaveChanges();


                return Weights;
            }
            catch(Exception e)
            {
                string ex = e.ToString();
                return null;
            }
        }

        [Route("updateWeights")]
        [HttpPost]
        // [Authorize]
        public tbl_Weights updateWeights(tbl_Weights Weights)
        {
            try
            {

                Encryption ecn = new Encryption();

                if (!Weights.WeightBack.HasValue)
                    Weights.WeightBack = 0;

                if (!Weights.WeightFront.HasValue)
                    Weights.WeightFront = 0;

                if (!Weights.WeightCenter.HasValue)
                    Weights.WeightCenter = 0;

                if (!Weights.WeightBackR.HasValue)
                    Weights.WeightBackR = 0;

                if (!Weights.WeightCenterR.HasValue)
                    Weights.WeightCenterR = 0;

                if (!Weights.WeightFrontR.HasValue)
                    Weights.WeightFrontR = 0;

                if (!Weights.SampleSize.HasValue)
                    Weights.SampleSize = 0;

                double sum = Weights.WeightBack.Value + Weights.WeightCenter.Value + Weights.WeightFront.Value +
                    Weights.WeightBackR.Value + Weights.WeightCenterR.Value + Weights.WeightFrontR.Value;

                if (Weights.SampleSize.Value > 0)
                    Weights.WeightAverage = Math.Round(sum / Weights.SampleSize.Value, 3);
                else
                    Weights.WeightAverage = 0;

                omnioEntities en = new omnioEntities();
                if (Weights.ID > 0)
                    en.tbl_Weights.AddOrUpdate(Weights);
                else
                    en.tbl_Weights.Add(Weights);

                en.SaveChanges();


                return Weights;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                return null;
            }
        }

        [Route("removeBroilerWeight")]
        [HttpPost]
        // [Authorize]
        public bool removeBroilerWeight(tbl_Weights Weights)
        {
            try
            {

               

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                tbl_Weights w = en.tbl_Weights.Find(Weights.ID);

                en.tbl_Weights.Remove(w);
                en.SaveChanges();


                return true;
            }
            catch
            {
                return false;
            }
        }



        // =================================================eventData--------------------------


        [Route("getCullData")]
        [HttpGet]
        // [Authorize]
        public List<tbl_CullsEvent> getCullData()
        {
            try
            {

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                List<tbl_CullsEvent> CullsEvent = en.tbl_CullsEvent.Where(x => x.ID > 0).ToList();


                return CullsEvent;
            }
            catch (Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }



        [Route("getMortalityData")]
        [HttpGet]
        // [Authorize]
        public List<tbl_MortalitiesEvent> getMortalityData()
        {
            try
            {

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                List<tbl_MortalitiesEvent> MortalitiesEvent = en.tbl_MortalitiesEvent.Where(x => x.ID > 0).ToList();


                return MortalitiesEvent;
            }
            catch (Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }





        [Route("getHistoricalMortalitiesAndCulls/")]
        [HttpPost]
        // [Authorize]
        public List<vw_briolerDataDay> getHistoricalMortalitiesAndCulls(info i)
        {
            try
            {

                omnioEntities en = new omnioEntities();
               en.Configuration.LazyLoadingEnabled = false;

                DateTime start = DateTime.Parse(i.start);
                DateTime end = DateTime.Parse(i.end);


                List<vw_briolerDataDay> cullsmor = en.vw_briolerDataDay.AsNoTracking().Where(x => x.ID == i.id && x.created > start && x.created < end).ToList();


                return cullsmor;
            }
            catch (Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }


        [Route("getHistoricalWeight/")]
        [HttpPost]
        // [Authorize]
        public List<tbl_Weights> getHistoricalWeight(info i)
        {
            try
            {

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                DateTime start = DateTime.Parse(i.start);
                DateTime end = DateTime.Parse(i.end);


                List<tbl_Weights> weights = en.tbl_Weights.Where(x => x.BroilerID == i.id && x.dateTime > start && x.dateTime < end).ToList();
               


                return weights;
            }
            catch (Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }

        [Route("getCatchHistoricalWeight/")]
        [HttpPost]
        // [Authorize]
        public List<tbl_catching> getCatchHistoricalWeight(info i)
        {
            try
            {

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                DateTime start = DateTime.Parse(i.start);
                DateTime end = DateTime.Parse(i.end);


                List<tbl_catching> weights = en.tbl_catching.Where(x => x.broilerId == i.id && x.date > start && x.date < end).ToList();



                return weights;
            }
            catch (Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }


        [Route("getHistoricalPWeight/")]
        [HttpPost]
        // [Authorize]
        public List<tbl_PlacementWeight> getHistoricalPWeight(info i)
        {
            try
            {

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                DateTime start = DateTime.Parse(i.start);
                DateTime end = DateTime.Parse(i.end);


                List<tbl_PlacementWeight> weights = en.tbl_PlacementWeight.Where(x => x.broilerId == i.id && x.date > start && x.date < end).ToList();



                return weights;
            }
            catch (Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }


        // =================================================WaterAndFeed--------------------------

        [Route("getWaterFeedData/{id}")]
        [HttpGet]
        // [Authorize]
        public List<tbl_FeedWater> getWaterFeedData(int id)
        {
            try
            {

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;
                List<tbl_FeedWater> FeedWater = en.tbl_FeedWater.Where(x => x.broilerID == id).ToList();

                return FeedWater;
            }
            catch (Exception e)
            {
                string str = e.ToString();
                return null;
            }
        }


        [Route("AddUpdateFeedWater")]
        [HttpPost]
        // [Authorize]
        public tbl_FeedWater AddUpdateFeedWater(tbl_FeedWater feedWater)
        {
            try
            {
                Encryption ecn = new Encryption();

                //  Weights.dateTime = Weights.dateTime.Value.ToLocalTime();
                if (!feedWater.Feed.HasValue)
                    feedWater.Feed = 0;

                if (!feedWater.WaterConsumption.HasValue)
                    feedWater.WaterConsumption = 0;


                omnioEntities en = new omnioEntities();
                en.tbl_FeedWater.AddOrUpdate(feedWater);
                en.SaveChanges();


                return feedWater;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                return null;
            }
        }

        [Route("removeFeedWater")]
        [HttpPost]
        // [Authorize]
        public bool removeFeedWater(tbl_FeedWater feedWater)
        {
            try
            {



                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;

                tbl_FeedWater w = en.tbl_FeedWater.Find(feedWater.ID);

                en.tbl_FeedWater.Remove(w);
                en.SaveChanges();


                return true;
            }
            catch
            {
                return false;
            }
        }

    }



}


public class info
{
   public int id { get; set; }
   public string start { get; set; }
   public  string end { get; set; }

    public info()
    {

    }
   

}


