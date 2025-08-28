using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using OmniAPI.Classes;

namespace OmniAPI.Provider
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SimpleAuthorizationServer : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); //   
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            using (var db = new omnioEntities())
            {
                if (db != null)
                {
                    //  var empl = db.Employees.ToList();
                    Encryption encryption = new Encryption();

                        var user = db.tbl_Users.ToList();
                    var userAdd = db.tbl_UsersAdditional.ToList();

                    tbl_Users credUser = null;
                    tbl_UsersAdditional credUserAdd = null;

                    string username = "";
                    string password = "";
                  //  username = encryption.ByteArrToString(encryption.Encrypt(context.UserName));
                  //  if (context.UserName != "michael.samson@omniolytics.co.uk")
                  //  {
                         username = encryption.ByteArrToString(encryption.Encrypt(context.UserName));
                        //  string password = encryption.ByteArrToString(encryption.Encrypt(context.Password));
                         password = encryption.ByteArrToString(encryption.Encrypt(context.Password));
                  //  }
                 //   else
                //    {
                     //   username = context.UserName;
                     //   password = context.Password;
                 //  }

                    if (user.Any(u => u.Email == username && u.Password == password))
                    {
                        credUser = user.Find(u => u.Email == username && u.Password == password);
                    }
                    else if (userAdd.Any(u => u.Email == username && u.Password == password) )
                    {
                        credUserAdd = userAdd.Find(u => u.Email == username && u.Password == password);
                        credUser = new tbl_Users();
                        credUser.Name = credUserAdd.Name;
                        credUser.Surname = credUserAdd.Surname;
                        credUser.Password = credUserAdd.Password;
                        credUser.Registered = credUserAdd.Registered;
                        credUser.UniqueID = credUserAdd.UniqueID;
                        credUser.Username = credUserAdd.Username;
                        credUser.UserTypeID = credUserAdd.UserTypeID;
                    }



                    if (credUser != null)
                    {
                        if (!string.IsNullOrEmpty(credUser.Name))
                        {

                            if (credUser.Registered != null)
                            {

                                identity.AddClaim(new Claim("Age", "16"));

                                var props = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                {
                                    "userdisplayname", context.UserName
                                },
                                {
                                     "role", "admin"
                                }
                             });

                                var ticket = new AuthenticationTicket(identity, props);
                                context.Validated(ticket);
                            }
                            else
                            {
                                context.SetError("invalid_veri", "Verification still required");
                                context.Rejected();
                            }
                        }
                        else
                        {
                            context.SetError("invalid_grant", "Provided username and password is incorrect");
                            context.Rejected();
                        }
                    }
                    else
                    {
                        context.SetError("invalid_grant", "Provided username and password is incorrect");
                        context.Rejected();
                    }
                }
                else
                {
                    context.SetError("invalid_grant", "Provided username and password is incorrect");
                    context.Rejected();
                }
                return;
            }
        }
    }
}