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
using IBMWIoTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace OmniAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LiveUpdateController : ApiController
    {


        private readonly HttpClient _client;

        public LiveUpdateController()
        {
            _client = new HttpClient();
        }



        public class EventData
        {
            public string typeId { get; set; }
            public string deviceId { get; set; }
            public string eventId { get; set; }
            public string format { get; set; }
            public DateTime timestamp { get; set; }
            public string payload { get; set; }
        }


        /*  [Route("getLatestTest/{deviceID}")]
          [HttpGet]
          public List<Models.tbl_DeviceDataDetail> getLatestTest(string deviceID)
          {

              Models.cb_LiveData livedata = new Models.cb_LiveData();

            //  Models.tbl_Devices devices = livedata.tbl_Devices.Find(deviceID);

           //   return devices.tbl_DeviceDataDetail.ToList(); 



          }*/




        [Route("getLatest/{deviceID}")]
        [HttpGet]
        public async Task<List<tbl_DeviceDataDetail>> GetLatest(string deviceID)
        {


            omnioEntities en = new omnioEntities();

            tbl_Devices devices = en.tbl_Devices.Find(deviceID);

            String type = devices.tbl_DeviceType.Name;
            string id = deviceID;
            //Console.WriteLine(type);
            //Console.WriteLine(id);


            //var payload = "";
            var payloadNH3 = "";
            DateTime timestamp = DateTime.Now;

            //string url = "https://ufeqt5.internetofthings.ibmcloud.com/api/v0002/device/types/" + type + "/devices/" + id+"/events";
            //string username = "a-ufeqt5-bjdkp6bnjo";
            //string password = "w+cALizg8gneI)*RBY";


            /*using (HttpClient client = new HttpClient())
            {
                // Set the basic authentication header
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                // Make the request
                HttpResponseMessage response = await client.GetAsync(url);

                 // Ensure a successful response
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string rData = await response.Content.ReadAsStringAsync();
                EventData[] data = JsonConvert.DeserializeObject<EventData[]>(rData);
                DateTime timestamp = Convert.ToDateTime(data[0].timestamp);
               
                if (type == "SC")
                {
                    payload = Convert.ToString(data[0].payload);

                    // payloadNH3 = Convert.ToString(data[1]["payload"]);

                    //                    DateTime timestamp_NH3 = Convert.ToDateTime(data[1]["timestamp"]);



                    ////  // payloadNH3;
                    ///

                    return ExtractDataSC(payload,devices.tbl_DeviceDataDetail.ToList(),timestamp);

                    //  payload = names;
                }
                else if(type =="SW") {
                    payload = Convert.ToString(data[0].payload);
                   return ExtractDataSW(payload, devices.tbl_DeviceDataDetail.ToList(), timestamp);

                }
                else if (type == "TH")
                {
                    payload = Convert.ToString(data[0].payload);
                   return  ExtractDataT(payload, devices.tbl_DeviceDataDetail.ToList(), timestamp);

                }
                //var ret = "No data";
                return null;
                
            }

*/

            if (type == "SC")
            {

                return ExtractDataSC(devices.tbl_DeviceDataDetail.ToList());

            }
            else if (type == "SW")
            {
                return ExtractDataSW(devices.tbl_DeviceDataDetail.ToList());

            }
            else if (type == "TH")
            {
                return ExtractDataT(devices.tbl_DeviceDataDetail.ToList());

            }
            //var ret = "No data";
            return null;


            /*ar payload = "";
             var payloadNH3 = "";
             DateTime timesstamp = Convert.ToDateTime((dynamic).data[0]["timestamp"]);
             DateTime timestamp = Convert.ToDateTime(data[0]["timestamp"]);*/
        }

        public class Person
        {
            public string TypeId { get; set; }
            public string DeviceId { get; set; }
            public string TimeStamp { get; set; }
        }


        private List<tbl_DeviceDataDetail> ExtractDataSC(List<tbl_DeviceDataDetail> details)
        {
            try
            {
                //byte[] dataT = Convert.FromBase64String(payload);
                //string d = Encoding.UTF8.GetString(dataT);
                //dSC dSW = JsonConvert.DeserializeObject<dSC>(d);

                //byte[] dataTNH3 = Convert.FromBase64String(payload);
                //string dNH3 = Encoding.UTF8.GetString(dataTNH3);
                //dSCNH3 dSWNH3 = JsonConvert.DeserializeObject<dSCNH3>(dNH3);

                var ambientTempDetail = details.Find(x => x.ValueType == "Temperature");
                //ambientTempDetail.TImestamp = ambientTempDetail.TImestamp;

                var humidityDetail = details.Find(x => x.ValueType == "Humidity");
                //humidityDetail.Value = 23.4;
                //humidityDetail.TImestamp = humidityDetail.TImestamp;

                var batteryDetail = details.Find(x => x.ValueType == "Battery");
                //batteryDetail.Value = Convert.ToDouble(payload);
                //batteryDetail.TImestamp = batteryDetail.TImestamp;

                var pressureDetail = details.Find(x => x.ValueType == "Pressure");
                //pressureDetail.Value = Convert.ToDouble(payload);
                //pressureDetail.TImestamp = pressureDetail.TImestamp;

                var co2Detail = details.Find(x => x.ValueType == "CO2");
                //co2Detail.Value = Convert.ToDouble(payload);
                //co2Detail.TImestamp = co2Detail.TImestamp;

                var nh3Detail = details.Find(x => x.ValueType == "NH3");
                //nh3Detail.Value = Convert.ToDouble(payload);
                //nh3Detail.TImestamp = timestamp;

                var luxDetail = details.Find(x => x.ValueType == "Lux");
                //luxDetail.Value = Convert.ToDouble(payload);
                //luxDetail.TImestamp = luxDetail.TImestamp;

                return new List<tbl_DeviceDataDetail>
                {
                    ambientTempDetail,
                    humidityDetail,
                    batteryDetail,
                    pressureDetail,
                    co2Detail,
                    nh3Detail,
                    luxDetail
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        private List<tbl_DeviceDataDetail> ExtractDataSW(List<tbl_DeviceDataDetail> details)
        {
            try
            {
                //byte[] dataT = Convert.FromBase64String(payload);
                //string d = Encoding.UTF8.GetString(dataT);
                //dSW dSW = JsonConvert.DeserializeObject<dSW>(d);

                var waterTempDetail = details.Find(x => x.ValueType == "Temperature");
                //waterTempDetail.Value = Convert.ToDouble(dSW.d.Temp.Replace('.', '.'));
                //waterTempDetail.TImestamp = waterTempDetail.TImestamp;

                var phDetail = details.Find(x => x.ValueType == "PH");
                //phDetail.Value = Convert.ToDouble(dSW.d.PH.Replace('.', '.'));
                //phDetail.TImestamp = phDetail.TImestamp;

                var orpDetail = details.Find(x => x.ValueType == "ORP");
                //orpDetail.Value = Convert.ToDouble(dSW.d.ORP.Replace('.', '.'));
                //orpDetail.TImestamp = orpDetail.TImestamp;


                var batteryDetail = details.Find(x => x.ValueType == "Battery");
                //batteryDetail.Value = Convert.ToDouble(dSW.d.BAT.Replace('.', '.'));
                //batteryDetail.TImestamp = batteryDetail.TImestamp;

                return new List<tbl_DeviceDataDetail>
                {
                    waterTempDetail,
                    phDetail,
                    orpDetail,
                    batteryDetail
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }


        private List<tbl_DeviceDataDetail> ExtractDataT(List<tbl_DeviceDataDetail> details)
        {
            try
            {
                /*byte[] dataT = Convert.FromBase64String(payload);
                string d = Encoding.UTF8.GetString(dataT);
                dT dSW = JsonConvert.DeserializeObject<dT>(d);*/

                var batteryDetail = details.Find(x => x.ValueType == "Battery");
                //batteryDetail.Value = Convert.ToDouble(dSW.d.BAT.Replace('.', '.'));
                //batteryDetail.TImestamp = batteryDetail.TImestamp;

                var ambientTempDetail = FindDetail(details, "Temperature", "Floor Temp", "Floor_Temp");
                //ambientTempDetail.Value = Convert.ToDouble(dSW.d.Temp.Replace('.', '.'));
                //ambientTempDetail.TImestamp = ambientTempDetail.TImestamp;


                return new List<tbl_DeviceDataDetail>
                {
                    batteryDetail,
                    ambientTempDetail
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
















        //Get Latest Raw Data
        [Route("getLatestRaw/{deviceID}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetLatestRaw(string deviceID)
        {


            omnioEntities en = new omnioEntities();

            tbl_Devices devices = en.tbl_Devices.Find(deviceID);

            String type = devices.tbl_DeviceType.Name;
            string id = deviceID;

            if (type == "SC")
            {

                return ExtractRawDataSC(devices.tbl_DeviceDataDetail.ToList(), id);

            }
            else if (type == "SW")
            {
                return ExtractRawDataSW(devices.tbl_DeviceDataDetail.ToList(), id);

            }
            else if (type == "TH")
            {
                return ExtractRawDataT(devices.tbl_DeviceDataDetail.ToList(), id);

            }
            //var ret = "No data";
            return null;

        }

        private tbl_DeviceDataDetail FindDetail(IEnumerable<tbl_DeviceDataDetail> details, params string[] valueTypes)
        {
            if (details == null || valueTypes == null || valueTypes.Length == 0)
            {
                return null;
            }

            return details.FirstOrDefault(detail =>
                detail != null &&
                valueTypes.Any(valueType => string.Equals(detail.ValueType, valueType, StringComparison.OrdinalIgnoreCase)));
        }

        private HttpResponseMessage ExtractRawDataSC(List<tbl_DeviceDataDetail> details, string id)
        {
            try
            {
                var ambientTempDetail = details.Find(x => x.ValueType == "Temperature");
                var humidityDetail = details.Find(x => x.ValueType == "Humidity");
                var batteryDetail = details.Find(x => x.ValueType == "Battery");
                var pressureDetail = details.Find(x => x.ValueType == "Pressure");
                var co2Detail = details.Find(x => x.ValueType == "CO2");
                var nh3Detail = details.Find(x => x.ValueType == "NH3");
                var luxDetail = details.Find(x => x.ValueType == "Lux");

                SC scData = new SC
                {
                    ID = id,
                    BAT = batteryDetail?.Value ,
                    Temp = ambientTempDetail?.Value,
                    Hum = humidityDetail?.Value,
                    Press = pressureDetail?.Value,
                    CO2 = co2Detail?.Value,
                    NH3 = nh3Detail?.Value,
                    Lux = luxDetail?.Value
                    //BAT = batteryDetail?.Value != null ? Convert.ToInt32(batteryDetail.Value) : 0,
                    //Temp = ambientTempDetail?.Value != null ? Convert.ToInt32(ambientTempDetail.Value) : 0,
                    //Hum = humidityDetail?.Value != null ? Convert.ToInt32(humidityDetail.Value) : 0,
                    //Press = pressureDetail?.Value != null ? Convert.ToInt32(pressureDetail.Value) : 0,
                    //CO2 = co2Detail?.Value != null ? Convert.ToInt32(co2Detail.Value) : 0,
                    //NH3 = nh3Detail?.Value != null ? Convert.ToInt32(nh3Detail.Value) : 0,
                    //Lux = luxDetail?.Value != null ? Convert.ToInt32(luxDetail.Value) : 0
                };

                dSC resData = new dSC
                {
                    d = scData
                };

                // Serialize the response object to a JSON string
                //string jsonResponse = JsonConvert.SerializeObject(resData);

                var jsonString = JsonConvert.SerializeObject(resData, Formatting.None);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        private HttpResponseMessage ExtractRawDataSW(List<tbl_DeviceDataDetail> details, string id)
        {
            try
            {
                var waterTempDetail = details.Find(x => x.ValueType == "Temperature");
                var phDetail = details.Find(x => x.ValueType == "PH");
                var orpDetail = details.Find(x => x.ValueType == "ORP");
                var batteryDetail = details.Find(x => x.ValueType == "Battery");

                SW swData = new SW
                {
                    ID = id,
                    BAT = batteryDetail?.Value,
                    Temp = waterTempDetail?.Value,
                    ORP = orpDetail?.Value,
                    PH = phDetail?.Value
                };

                dSW resData = new dSW
                {
                    d = swData
                };

                // Serialize the response object to a JSON string
                //string jsonResponse = JsonConvert.SerializeObject(resData);
                var jsonString = JsonConvert.SerializeObject(resData, Formatting.None);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }



        private HttpResponseMessage ExtractRawDataT(List<tbl_DeviceDataDetail> details, string id)
        {
            try
            {
                var batteryDetail = details.Find(x => x.ValueType == "Battery");
                var ambientTempDetail = FindDetail(details, "Temperature", "Floor Temp", "Floor_Temp");

                T tData = new T
                {
                    ID = id,
                    BAT = batteryDetail?.Value,
                    Floor_Temp = ambientTempDetail?.Value
                    //BAT = batteryDetail?.Value != null ? Convert.ToInt32(batteryDetail.Value) : 0,
                    //Floor_Temp = ambientTempDetail?.Value != null ? Convert.ToInt32(ambientTempDetail.Value) : 0
                };

                dT resData = new dT
                {
                    d = tData
                };

                // Serialize the response object to a JSON string
                //string jsonResponse = JsonConvert.SerializeObject(resData);
                //JsonConvert.SerializeObject(resData, Formatting.None);
                var jsonString = JsonConvert.SerializeObject(resData, Formatting.None);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }



    }
}

public class dSC
{
    public SC d  { get; set; }
 
}

public class dSCNH3
{
    public SCNH3 d { get; set; }

}

public class dSW
{
    public SW d { get; set; }

}

public class dT
{
    public T d { get; set; }

}


public class SC
{
    public string ID { get; set; }
    public double? BAT { get; set; }
    public double? Temp { get; set; }
    public double? Hum { get; set; }
    public double? Press { get; set; }
    public double? NH3 { get; set; }
    public double? CO2 { get; set; }
    public double? Lux { get; set; }


}

public class SCNH3
{
    public string ID { get; set; }
    public string NH3 { get; set; }
    public string CO2 { get; set; }
    public string Lux { get; set; }



}

public class SW
{
    public string ID { get; set; }
    public double? BAT { get; set; }
    public double? Temp { get; set; }
    public double? PH { get; set; }
    public double? ORP { get; set; }
    public double? EC { get; set; }


}

public class T
{
    public string ID { get; set; }
    public double? BAT { get; set; }
    public double? Floor_Temp { get; set; }
    //public string Hum { get; set; }
  

}
