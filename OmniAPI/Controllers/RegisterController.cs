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
using IBMWIoTP;

namespace OmniAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class RegisterController : ApiController
    {


        [Route("getUser")]
        [HttpPost]
        [Authorize]
        public tbl_Users getUser(userReg Username)
        {
            try
            {
                Encryption ecn = new Encryption();

                omnioEntities en = new omnioEntities();
                en.Configuration.LazyLoadingEnabled = false;
                string pass = ecn.Decrypt(ecn.StrToByteArray("213105252240038159078029024226205218231107005041"));

             //   if (Username.Username != "michael.samson@omniolytics.co.uk" )
                Username.Username = ecn.ByteArrToString(ecn.Encrypt(Username.Username));

                if (en.tbl_Users.Any(x => x.Email == Username.Username))
                {

                    tbl_Users farmers = en.tbl_Users.First(x => x.Email == Username.Username);

                  //  if (farmers.Username != "michael.samson@omniolytics.co.uk")
                   // {
                        farmers.Name = ecn.Decrypt(ecn.StrToByteArray(farmers.Name));
                        farmers.Surname = ecn.Decrypt(ecn.StrToByteArray(farmers.Surname));
                        farmers.Email = ecn.Decrypt(ecn.StrToByteArray(farmers.Email));
                        farmers.ContactNumber = ecn.Decrypt(ecn.StrToByteArray(farmers.ContactNumber));

                        farmers.Username = farmers.Username;
                       
                  //  }

                    return farmers;
                }
                else if(en.tbl_UsersAdditional.Any(x => x.Email == Username.Username))
                {

                    tbl_UsersAdditional farmersAdd = en.tbl_UsersAdditional.First(x => x.Email == Username.Username);
                    tbl_Users farmers = en.tbl_Users.First(x => x.ID == farmersAdd.UserID);

                    farmers.Name = ecn.Decrypt(ecn.StrToByteArray(farmersAdd.Name));
                    farmers.Surname = ecn.Decrypt(ecn.StrToByteArray(farmersAdd.Surname));
                    farmers.Email = ecn.Decrypt(ecn.StrToByteArray(farmersAdd.Email));
                    farmers.ContactNumber = ecn.Decrypt(ecn.StrToByteArray(farmersAdd.ContactNumber));

                    farmers.Username = "*";
                    return farmers;
                }
                else
                {
                    return null;
                }
                  
                
            }
            catch
            {
                return null;
            }
        }


        [Route("checkUser")]
        [HttpPost]
       
        public bool checkUser(userReg Username)
        {
            try
            {
                Encryption ecn = new Encryption();

                omnioEntities en = new omnioEntities();

                if (Username.Username != "Administrator")
                    Username.Username = ecn.ByteArrToString(ecn.Encrypt(Username.Username));
                else
                    return false;

                int count = en.tbl_Users.Count(x => x.Username == Username.Username); 

                if (count == 0)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        [Route("registerUser")]
        [HttpPost]

        public string registerUser(userReg UserReg)
        {
            try
            {
                Encryption ecn = new Encryption();

                omnioEntities en = new omnioEntities();

                if (en.tbl_Users.Any(x => x.UniqueID == UserReg.UniqueID))
                {
                    tbl_Users users = en.tbl_Users.Single(x => x.UniqueID == UserReg.UniqueID);

                    if (users.Registered.Value)
                        return "User has already registered";

                   // users.Username = ecn.ByteArrToString(ecn.Encrypt(UserReg.Username));
                    users.Password = ecn.ByteArrToString(ecn.Encrypt(UserReg.Password));
                    users.Registered = true;

                    en.SaveChanges();


                    return "User registered";
                }
                else if (en.tbl_UsersAdditional.Any(x => x.UniqueID == UserReg.UniqueID))
                {
                    tbl_UsersAdditional users = en.tbl_UsersAdditional.Single(x => x.UniqueID == UserReg.UniqueID);

                    if (users.Registered.Value)
                        return "User has already registered";

                 //   users.Username = ecn.ByteArrToString(ecn.Encrypt(UserReg.Username));
                    users.Password = ecn.ByteArrToString(ecn.Encrypt(UserReg.Password));
                    users.Registered = true;

                    en.SaveChanges();


                    return "User registered";

                }
                else
                {
                    return "User does not exist";
                }

             

            }
            catch
            {
                return "Error";
            }
        }


       


    }
}

public class userReg
{

    public string UniqueID { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public userReg()
    {

    }
}
