using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI_Exercise_03_10_2020.Models;



namespace WebAPI_Exercise_03_10_2020.Controllers
{
    [RoutePrefix("api/UserAccount")]
    public class UserAccountController : ApiController
    {

        WEB_API_DATAEntities webApiDB = new WEB_API_DATAEntities();

        [HttpPost]
        [Route("userRegistration")]
        public object UserRegistration(USER_REGISTRATION_DATA uSER_REGISTRATION_)
        {
            try
            {
                var data = webApiDB.USER_REGISTRATION_DATA.FirstOrDefault(x => x.ID == uSER_REGISTRATION_.ID || x.USER_EMAIL == uSER_REGISTRATION_.USER_EMAIL);
                if (data == null)
                {
                    uSER_REGISTRATION_.PASSWD = Encryption.Encrypt(uSER_REGISTRATION_.PASSWD);

                    USER_LOGIN_DATE UserLogin = new USER_LOGIN_DATE
                    {
                        USER_ID = uSER_REGISTRATION_.USER_EMAIL,
                        PASSWD = uSER_REGISTRATION_.PASSWD,
                        ISACTIVE = true,
                        CREATED_DATE = DateTime.Now,
                        UPDATED_DATE = null
                    };
                    webApiDB.USER_LOGIN_DATE.Add(UserLogin);

                    uSER_REGISTRATION_.CREATED_DATE = DateTime.Now;
                    uSER_REGISTRATION_.ISACTIVE = true;
                    webApiDB.USER_REGISTRATION_DATA.Add(uSER_REGISTRATION_);

                    webApiDB.SaveChanges();

                    return new { result = "New User added successfully..!!" };


                }
                else
                {
                    return new { error = "This user allready exist. Try register with another email ID." };
                }
            }
            catch (Exception ex)
            {
                return new { error = ex.Message.ToString() };
            }
        }


        [HttpPost]
        [Route("UpdateUser/{id}")]
        public object UpdateUserData(int id, USER_REGISTRATION_DATA udt)
        {
            try
            {
                var userData = webApiDB.USER_REGISTRATION_DATA.Find(id);
                if (userData != null)
                {
                    userData.FIRST_NAME = udt.FIRST_NAME;
                    userData.LAST_NAME = udt.LAST_NAME;
                    userData.USER_EMAIL = udt.USER_EMAIL;
                    userData.USER_ADDR = udt.USER_ADDR;
                    userData.CONTACT_NUMBER = udt.CONTACT_NUMBER;
                    userData.UPDATE_DATE = DateTime.Now;

                    webApiDB.SaveChanges();

                    return new { result = "User details updated..!!" };
                }
                else
                {
                    return new { error = "This user does not exist..!!" };
                }
            }
            catch (Exception ex)
            {
                return new { error = ex.Message.ToString() };
            }
        }

        [HttpPost]
        [Route("UserLogin")]
        public object UserLogin(JObject jsonData)
        {
            try
            {
                dynamic json = jsonData;
                string usrPwd = "";
                string jEmail = json.email;
                string jPassword = json.password;

                long q = (from login in webApiDB.USER_LOGIN_DATE
                          where login.USER_ID == jEmail && login.ISACTIVE == true
                          select login.ID).FirstOrDefault();

                string pwd = (from login in webApiDB.USER_LOGIN_DATE
                              where login.USER_ID == jEmail && login.ISACTIVE == true && login.ID == q
                              select login.PASSWD).FirstOrDefault();

                if (pwd.Any())
                {
                    usrPwd = Encryption.Decrypt(pwd);
                }
                else
                {
                    return new { error = "Invalid Details..!!" };
                }


                long data = (from login in webApiDB.USER_LOGIN_DATE
                             where login.USER_ID == jEmail && jPassword == usrPwd && login.ISACTIVE == true
                             select login.ID).FirstOrDefault();


                if (data > 0)
                {
                    return new { result = q.ToString() };
                }
                else
                {
                    return new { error = "INVALID DETAILS..!!" };
                }
            }
            catch (Exception ex)
            {
                return new { error = ex.Message.ToString() };
            }
        }

        [HttpGet]
        [Route("getUserData/{id}")]
        public object GetUserDetailByID(int id)
        {
            try
            {
                var data = webApiDB.USER_REGISTRATION_DATA.Find(id);
                if (data != null)
                {
                    return new { result = data };
                }
                else
                {
                    return new { error = "Data not found..!!" };
                }

            }
            catch (Exception ex)
            {
                return new { error = ex.Message.ToString() };
            }
        }

        [HttpPost]
        [Route("changePassword")]
        public object UserChangePassword(JObject jsonData)
        {
            try
            {
                dynamic json = jsonData;
                string jEmail = json.email;
                string jNewPassword = json.password;

                var data = (from login in webApiDB.USER_REGISTRATION_DATA
                            where login.USER_EMAIL == jEmail
                            select login).FirstOrDefault();

                var q = (from usrLogin in webApiDB.USER_LOGIN_DATE
                            where usrLogin.USER_ID == jEmail
                            select usrLogin).FirstOrDefault();

                if (data != null)
                {
                    data.PASSWD = Encryption.Encrypt(jNewPassword);

                }
                else
                {
                    return new { error = "Invalid Email ID..!!" };
                }

                if(q!=null)
                {
                    q.PASSWD= Encryption.Encrypt(jNewPassword);
                    webApiDB.SaveChanges();
                    return new { result = "Password updated success..!!" };
                }
                else
                {
                    return new { error = "Invalid Email ID..!!" };
                }

            }
            catch (Exception ex)
            {
                return new { error = ex.Message.ToString() };
            }
        }

        [HttpGet]
        [Route("GetAllUserInfo")]
        public object GetAllUserData()
        {
            try
            {
                var data = webApiDB.USER_REGISTRATION_DATA.Select(x => new { x.FIRST_NAME, x.LAST_NAME, x.USER_EMAIL, x.USER_ADDR, x.CONTACT_NUMBER }).ToArray();
                if(data.Any())
                {
                    return new { result = data };
                }
                else
                {
                    return new { error = "DATA NOT FOUND..!!" };
                }
            }
            catch(Exception ex)
            {
                return new { error = ex.Message.ToString() };
            }
        }

    }
}
