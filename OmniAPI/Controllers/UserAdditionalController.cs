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
    public class UserAdditionalController : ApiController
    {
        private byte[] Key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
        private byte[] Vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 252, 112, 79, 32, 114, 156 };

        private ICryptoTransform EncryptorTransform, DecryptorTransform;
        private System.Text.UTF8Encoding UTFEncoder;

        [Route("Farmer/addUserAdditional/")]
        [HttpPost]
        public int addUserAdditional(tbl_UsersAdditional farmer)
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
                tbl_Users actUser;
                if (farmer.ID == 0)
                     actUser = en.tbl_Users.Find(farmer.UserID);
                else
                    actUser = en.tbl_Users.Find(farmer.ID);




                tbl_UsersAdditional user = new tbl_UsersAdditional();

                Encryption enc = new Encryption();

                user.Name = enc.ByteArrToString(enc.Encrypt(farmer.Name));
                user.Surname = enc.ByteArrToString(enc.Encrypt(farmer.Surname));
                user.Email = enc.ByteArrToString(enc.Encrypt(farmer.Email));
                user.ContactNumber = enc.ByteArrToString(enc.Encrypt(farmer.ContactNumber));
                user.Username = enc.ByteArrToString(enc.Encrypt(farmer.Username));
                user.Password = "";
                user.UserTypeID = 2;
                user.Registered = false;
                user.UniqueID = Guid.NewGuid().ToString();
                user.UserID = farmer.UserID;

                if(actUser.UserTypeID.Value == 1)
                {
                    user.UserTypeID = 1;
                }
              

                if (en.tbl_UsersAdditional.Any(x => x.Email == user.Email) && en.tbl_Users.Any(x => x.Email == user.Email))
                {
                    return 1;
                }
                else
                {

                    en.tbl_UsersAdditional.Add(user);
                    en.SaveChanges();

                    sendEmail(farmer.Email, user.UniqueID, farmer.Name, farmer.Surname);
                }
               

              //  sendEmail(farmer.Email, user.UniqueID, farmer.Name, farmer.Surname);

                return 0;
            }
            catch
            {

                return 2;
            }

            // return ByteArrToString(Encrypt(name));
        }


        [Route("getUserAdd/{userID}")]
        [HttpGet]
        public List<tbl_UsersAdditional> getUserAdd(int userID)
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
                List<tbl_UsersAdditional> farmers = en.tbl_UsersAdditional.Where(x => x.UserID == userID).ToList();

                List<tbl_UsersAdditional> farmersDecode = new List<tbl_UsersAdditional>();

                Encryption enc = new Encryption();

                foreach (tbl_UsersAdditional farmer in farmers)
                {
                    if (farmer.Username != "Administrator")
                    {
                        tbl_UsersAdditional decode = new tbl_UsersAdditional();
                        decode.ID = farmer.ID;
                        decode.Name = enc.Decrypt(enc.StrToByteArray(farmer.Name));
                        decode.Surname = enc.Decrypt(enc.StrToByteArray(farmer.Surname));
                        decode.Email = enc.Decrypt(enc.StrToByteArray(farmer.Email));
                        decode.ContactNumber = enc.Decrypt(enc.StrToByteArray(farmer.ContactNumber));

                        decode.Registered = farmer.Registered;
                        decode.Username = farmer.Username;
                     //   decode.tbl_Farm = farmer.tbl_Farm;

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


        [Route("editUserAdd")]
        [HttpPost]
        public bool editUserAdd(tbl_UsersAdditional farmer)
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
                tbl_UsersAdditional user = en.tbl_UsersAdditional.Find(farmer.ID);
                Encryption enc = new Encryption();
                if (user.Email ==  enc.ByteArrToString(enc.Encrypt(farmer.Email)) )
                {
                                     
                    user.Name = enc.ByteArrToString(enc.Encrypt(farmer.Name));
                    user.Surname = enc.ByteArrToString(enc.Encrypt(farmer.Surname));
                    user.Email = enc.ByteArrToString(enc.Encrypt(farmer.Email));
                    user.ContactNumber = enc.ByteArrToString(enc.Encrypt(farmer.ContactNumber));

                   // user.Registered = farmer.Registered;

                    if (farmer.Password != "")
                    {
                        user.Password = enc.ByteArrToString(enc.Encrypt(farmer.Password));
                    }


                    en.SaveChanges();
                }
                else
                {
                    string email = enc.ByteArrToString(enc.Encrypt(farmer.Email));

                    if (en.tbl_UsersAdditional.Any(x => x.Email == email) && en.tbl_Users.Any(x => x.Email == email)  )
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



                return true;
            }
            catch(Exception ex)
            {

                string e = ex.ToString();
                return false;
            }

            // return ByteArrToString(Encrypt(name));
        }

        [Route("removeUserAdd")]
        [HttpPost]
        public bool removeUserAdd(tbl_UsersAdditional farmer)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_UsersAdditional user = en.tbl_UsersAdditional.Find(farmer.ID);
                en.tbl_UsersAdditional.Remove(user);

                en.SaveChanges();

                return true;
            }
            catch (Exception e)
            {

                return false;
            }

            // return ByteArrToString(Encrypt(name));
        }



        public string sendEmail(string address, string uniqueID, string name, string surname)
        {
            try
            {
                if (1 == 1)
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




                    return "1";
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }


        [Route("sendMailAdd/{id}")]
        [HttpGet]
        public bool sendMailAdd(int id)
        {
            try
            {
                omnioEntities en = new omnioEntities();
                tbl_UsersAdditional user = en.tbl_UsersAdditional.Find(id);

                RijndaelManaged rm = new RijndaelManaged();

                //Create an encryptor and a decryptor using our encryption method, key, and vector.
                EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
                DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

                //Used to translate bytes to text and vice versa
                UTFEncoder = new System.Text.UTF8Encoding();


                tbl_Users farmersDecode = new tbl_Users();
                Encryption enc = new Encryption();

                tbl_UsersAdditional decode = new tbl_UsersAdditional();
                decode.ID = user.ID;
                decode.Name = enc.Decrypt(enc.StrToByteArray(user.Name));
                decode.Surname = enc.Decrypt(enc.StrToByteArray(user.Surname));
                decode.Email = enc.Decrypt(enc.StrToByteArray(user.Email));
                decode.ContactNumber = enc.Decrypt(enc.StrToByteArray(user.ContactNumber));

                decode.Registered = user.Registered;
                decode.Username = user.Username;


                sendEmail(decode.Email, user.UniqueID, decode.Name, decode.Surname);

                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }
    }
}
