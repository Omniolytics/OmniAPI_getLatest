using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using OmniAPI.Models;

namespace OmniAPI.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BroilerController : ApiController
    {


        [Route("getHatcheryData/{id}")]
        [HttpGet]
        public List<tbl_DOC>  getHatcheryData(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_DOC> hatch =   en.tbl_DOC.Where(x => x.broilerId == id).ToList();
              //  int max = hatch.Max(x => x.id);


                return hatch;
            }
            catch
            {
                return null;
            }
        }


        [Route("updateHatcheryData")]
        [HttpPost]
        public int updateHatcheryData(tbl_DOC doc)
        {
            try
            {
               

                omnioEntities en = new omnioEntities();

                en.tbl_DOC.AddOrUpdate(doc);
                en.SaveChanges();
                return doc.id;
            }
            catch
            {
                return 0;
            }
        }


        [Route("getCylces")]
        [HttpGet]
        public List<tbl_Cycles> getCylces()
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_Cycles> cylces = en.tbl_Cycles.ToList();



                return cylces;
            }
            catch
            {
                return null;
            }
        }

        [Route("getCylceId/{id}")]
        [HttpGet]
        public List<tbl_Cycles> getCylceId(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_Cycles> cylces = en.tbl_Cycles.Where( x=> x.broilerid == id).ToList();



                return cylces;
            }
            catch
            {
                return null;
            }
        }

        [Route("getFeed/{id}")]
        [HttpGet]
        public List<tbl_Feed> getFeed(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_Feed> feed = en.tbl_Feed.Where(x => x.broilerId == id).ToList();

                return feed;
            }
            catch
            {
                return null;
            }
        }

        [Route("getFeed/{id}/{cycleId}")]
        [HttpGet]
        public List<tbl_Feed> getFeed(int id, int cycleId)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_Feed> feed = en.tbl_Feed.Where(x => x.broilerId == id && x.cycleId == cycleId).ToList();

                return feed;
            }
            catch
            {
                return null;
            }
        }

        [Route("getKpiNotifications/{id}")]
        [HttpGet]
        public List<KpiNotificationDto> getKpiNotifications(int id)
        {
            try
            {
                using (omnioEntities en = new omnioEntities())
                {
                    const string query = "SELECT KPI, DeviationP, Enabled, PrimaryContact, SecondaryContact, Delay FROM dbo.tbl_KpiNotifications WHERE BroilerID = @broilerId";
                    SqlParameter broilerIdParameter = new SqlParameter("@broilerId", id);
                    List<KpiNotificationDto> notifications = en.Database.SqlQuery<KpiNotificationDto>(query, broilerIdParameter).ToList();

                    if (!notifications.Any())
                    {
                        const string fallbackQuery = "SELECT KPI, DeviationP, Enabled, PrimaryContact, SecondaryContact, Delay FROM dbo.tbl_KpiNotifications WHERE CAST(BroilerID AS NVARCHAR(50)) = @broilerId";
                        SqlParameter fallbackParameter = new SqlParameter("@broilerId", id.ToString());
                        notifications = en.Database.SqlQuery<KpiNotificationDto>(fallbackQuery, fallbackParameter).ToList();
                    }

                    return notifications;
                }
            }
            catch
            {
                return null;
            }
        }

        [Route("updateKpiNotifications")]
        [HttpPost]
        public int updateKpiNotifications(KpiNotificationUpdateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Kpi))
            {
                return 0;
            }

            try
            {
                using (omnioEntities en = new omnioEntities())
                {
                    if (request.Id.HasValue)
                    {
                        SqlParameter[] updateParameters = new[]
                        {
                            new SqlParameter("@Id", request.Id.Value),
                            new SqlParameter("@BroilerId", (object)request.BroilerId ?? DBNull.Value),
                            new SqlParameter("@KPI", (object)request.Kpi?.Trim() ?? DBNull.Value),
                            new SqlParameter("@DeviationP", (object)request.DeviationP ?? DBNull.Value),
                            new SqlParameter("@Enabled", (object)request.Enabled ?? DBNull.Value),
                            new SqlParameter("@PrimaryContact", (object)request.PrimaryContact ?? DBNull.Value),
                            new SqlParameter("@SecondaryContact", (object)request.SecondaryContact ?? DBNull.Value),
                            new SqlParameter("@Delay", (object)request.Delay ?? DBNull.Value)
                        };

                        const string updateQuery = @"UPDATE dbo.tbl_KpiNotifications
                                                     SET BroilerID = @BroilerId,
                                                         KPI = @KPI,
                                                         DeviationP = @DeviationP,
                                                         Enabled = @Enabled,
                                                         PrimaryContact = @PrimaryContact,
                                                         SecondaryContact = @SecondaryContact,
                                                         Delay = @Delay
                                                     WHERE id = @Id";

                        return en.Database.ExecuteSqlCommand(updateQuery, updateParameters);
                    }
                    else
                    {
                        if (!request.BroilerId.HasValue)
                        {
                            return 0;
                        }

                        SqlParameter[] insertParameters = new[]
                        {
                            new SqlParameter("@BroilerId", request.BroilerId.Value),
                            new SqlParameter("@KPI", (object)request.Kpi?.Trim() ?? DBNull.Value),
                            new SqlParameter("@DeviationP", (object)request.DeviationP ?? DBNull.Value),
                            new SqlParameter("@Enabled", (object)request.Enabled ?? DBNull.Value),
                            new SqlParameter("@PrimaryContact", (object)request.PrimaryContact ?? DBNull.Value),
                            new SqlParameter("@SecondaryContact", (object)request.SecondaryContact ?? DBNull.Value),
                            new SqlParameter("@Delay", (object)request.Delay ?? DBNull.Value)
                        };

                        const string insertQuery = @"INSERT INTO dbo.tbl_KpiNotifications (BroilerID, KPI, DeviationP, Enabled, PrimaryContact, SecondaryContact, Delay)
                                                     VALUES (@BroilerId, @KPI, @DeviationP, @Enabled, @PrimaryContact, @SecondaryContact, @Delay);
                                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        return en.Database.SqlQuery<int>(insertQuery, insertParameters).FirstOrDefault();
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        [Route("getActData/{id}/{cycleId}")]
        [HttpGet]
        public List<tbl_activities> getActData(int id,int cycleId)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_activities> activities = en.tbl_activities.Where(x => x.broilerId == id && x.cycleId == cycleId).ToList();

                for(int i = 0; i < activities.Count; i++)
                {

                    if (activities[i].startDate.HasValue)
                        activities[i].startDate = activities[i].startDate.Value.AddHours(2);
                }

                return activities;
            }
            catch
            {
                return null;
            }
        }

        [Route("getBroilerSettings/{id}")]
        [HttpGet]
        public List<tbl_BroilerSettings> getBroilerSettings(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_BroilerSettings> bs = en.tbl_BroilerSettings.Where(x => x.broilerId == id).ToList();



                return bs;
            }

            catch
            {
                return null;
            }
        }

        [Route("getCatchData/{id}")]
        [HttpGet]
        public List<tbl_catching> getCatchData(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_catching> bs = en.tbl_catching.Where(x => x.broilerId == id).ToList();



                return bs;
            }

            catch
            {
                return null;
            }
        }

        [Route("getCatchData/{id}/{cycleId}")]
        [HttpGet]
        public List<tbl_catching> getCatchData(int id, int cycleId)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_catching> bs = en.tbl_catching.Where(x => x.broilerId == id && x.cycleId == cycleId).ToList();

                return bs;
            }
            catch
            {
                return null;
            }
        }

        [Route("getDOCData/{id}")]
        [HttpGet]
        public List<tbl_DOCTable> getDOCData(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List < tbl_DOCTable> bs = en.tbl_DOCTable.Where(x => x.broilerId == id).ToList();
                               
                return bs;
            }

            catch
            {
                return null;
            }
        }

        [Route("getSystem/{id}")]
        [HttpGet]
        public List<tbl_Systems> getSystem(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_Systems> bs = en.tbl_Systems.Where(x => x.broilerId == id).ToList();

                return bs;
            }

            catch
            {
                return null;
            }
        }

        [Route("getHealth/{id}")]
        [HttpGet]
        public List<tbl_Health> getHealth(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_Health> bs = en.tbl_Health.Where(x => x.broilerId == id).ToList();

                return bs;
            }

            catch
            {
                return null;
            }
        }


        [Route("getPlacementWeights/{id}")]
        [HttpGet]
        public List<tbl_PlacementWeight> getPlacementWeights(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_PlacementWeight> bs = en.tbl_PlacementWeight.Where(x => x.broilerId == id).ToList();



                return bs;
            }

            catch
            {
                return null;
            }
        }

        [Route("getPlacementWeights/{id}/{cycleId}")]
        [HttpGet]
        public List<tbl_PlacementWeight> getPlacementWeights(int id, int cycleId)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                List<tbl_PlacementWeight> bs = en.tbl_PlacementWeight.Where(x => x.broilerId == id && x.cycleId == cycleId).ToList();

                return bs;
            }
            catch
            {
                return null;
            }
        }



        [Route("updateBroilerSettings")]
        [HttpPost]
        public int updateBroilerSettings(tbl_BroilerSettings bs)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_BroilerSettings.AddOrUpdate(bs);
                en.SaveChanges();
                return bs.id;
            }
            catch
            {
                return 0;
            }
        }

        [Route("updateDOCTable")]
        [HttpPost]
        public int updateDOCTableSettings(tbl_DOCTable bs)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_DOCTable.AddOrUpdate(bs);
                en.SaveChanges();
                return bs.id;
            }
            catch
            {
                return 0;
            }
        }


        [Route("updateSystems")]
        [HttpPost]
        public int updateSystems(tbl_Systems bs)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_Systems.AddOrUpdate(bs);
                en.SaveChanges();
                return bs.id;
            }
            catch
            {
                return 0;
            }
        }

        [Route("updateHealth")]
        [HttpPost]
        public int updateHealth(tbl_Health bs)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_Health.AddOrUpdate(bs);
                en.SaveChanges();
                return bs.id;
            }
            catch
            {
                return 0;
            }
        }

        [Route("updateActvities")]
        [HttpPost]
        public int updateActvities(tbl_activities bs)
        {
            try
            {
                bs.sid_1 = bs.sid_1 ??  "0";
                bs.sid_2 = bs.sid_2 ?? "0";
                bs.sid_3 = bs.sid_3 ?? "0";
                bs.sid_4 = bs.sid_4 ?? "0";

                omnioEntities en = new omnioEntities();
                en.tbl_activities.AddOrUpdate(bs);
                en.SaveChanges();
                return bs.id;
            }
            catch(Exception e)
            {
                string s = e.ToString();
                return 0;
            }
        }

        [Route("removeDOCTable")]
        [HttpPost]
        public bool removeDOCTable(tbl_DOCTable act)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_DOCTable f = en.tbl_DOCTable.Find(act.id);


                en.tbl_DOCTable.Remove(f);
                en.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("removeSystem")]
        [HttpPost]
        public bool removeSystem(tbl_Systems act)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_Systems f = en.tbl_Systems.Find(act.id);


                en.tbl_Systems.Remove(f);
                en.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("removeHealth")]
        [HttpPost]
        public bool removeHealth(tbl_Health act)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_Health f = en.tbl_Health.Find(act.id);


                en.tbl_Health.Remove(f);
                en.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("removeActivities")]
        [HttpPost]
        public bool removeCylces(tbl_activities act)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_activities f = en.tbl_activities.Find(act.id);


                en.tbl_activities.Remove(f);
                en.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("updateCatchData")]
        [HttpPost]
        public bool updateCatchData(tbl_catching bs)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_catching.AddOrUpdate(bs);
                en.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        [Route("updateCylces")]
        [HttpPost]
        public bool updateCylces(tbl_Cycles cycles)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_Cycles.AddOrUpdate(cycles);
                en.SaveChanges();


                return true;
            }
            catch
            {
                return false;
            }
        }


        [Route("removeCylces")]
        [HttpPost]
        public bool removeCylces(tbl_Cycles cycles)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_Cycles f = en.tbl_Cycles.Find(cycles.id);


                en.tbl_Cycles.Remove(f);
                en.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("updateFeed")]
        [HttpPost]
        public bool updateFeed(tbl_Feed cycles)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                en.tbl_Feed.AddOrUpdate(cycles);
                en.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("updateWeight")]
        [HttpPost]
        public bool updateWeight(tbl_PlacementWeight weight)
        {
            try
            {
                double bird = (weight.totalBirdWeight.Value) / weight.noOfBirdsPerBox.Value;
                int boxes = Convert.ToInt32(bird);
                omnioEntities en = new omnioEntities();

                var existing = en.tbl_PlacementWeight.FirstOrDefault(x => x.id == weight.id);
                if (existing != null)
                {
                    existing.totalBirdWeight = weight.totalBirdWeight;
                    existing.averageBox = weight.averageBox;
                    existing.noOfBoxes = weight.noOfBoxes;
                    existing.noOfBirdsPerBox = weight.noOfBirdsPerBox;
                    existing.date = weight.date;
                    existing.broilerId = weight.broilerId;
                    existing.cycleId = weight.cycleId;
                    existing.weightId = weight.weightId;
                }
                else
                {
                    en.tbl_PlacementWeight.Add(weight);
                }

                en.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("removeFeed")]
        [HttpPost]
        public bool removeFeed(tbl_Feed cycles)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_Feed f = en.tbl_Feed.Find(cycles.id);
                en.tbl_Feed.Remove(f);
                en.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("removePlacementWeight")]
        [HttpPost]
        public bool removePlacementWeight(tbl_PlacementWeight cycles)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_PlacementWeight f = en.tbl_PlacementWeight.Find(cycles.id);
             
            
                en.tbl_PlacementWeight.Remove(f);
                en.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("removeCatch")]
        [HttpPost]
        public bool removeFeed(tbl_catching cycles)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_catching f = en.tbl_catching.Find(cycles.id);
                en.tbl_catching.Remove(f);
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
