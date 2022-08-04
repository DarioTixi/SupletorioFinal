using FacturaWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FacturaWeb.Controllers
{
    public class LoginController : Controller
    {

        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);

       
        public ActionResult Index()
        {
              return View();
        }
        public string ConvertirSha256(string texto)
        {
            //using System.Text;
            //USAR LA REFERENCIA DE "System.Security.Cryptography"

            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public string validarCredenciales(Usuario user)
        {
            Respuesta respuesta = new Respuesta();
            try
            {

                string query = "Select * from Usuario Where email = @email and password = @password and estado = 1 ";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", user.email);
                command.Parameters.AddWithValue("@password", user.password);
                connection.Open();

                using (SqlDataReader dr = command.ExecuteReader())
                {

                    if (dr.Read())
                    {
                        Usuario userResponse = new Usuario();
                        userResponse.email = dr["email"].ToString();
                        userResponse.password = dr["password"].ToString();
                        userResponse.idRol = (Rol)dr["idRol"];
                        userResponse.id = (int)dr["id"];

                        FormsAuthentication.SetAuthCookie(userResponse.email, false);
                        Session["Usuario"] = userResponse;

                        respuesta.Datos = userResponse;
                        return JsonConvert.SerializeObject(respuesta);
                    }
                    respuesta.Datos = null;
                    return JsonConvert.SerializeObject(null);
                }

            
            }
            catch (Exception ex)
            {
                respuesta.Error = ex.Message;
                return JsonConvert.SerializeObject(respuesta);
            }


        }

        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            Session["Usuario"] = null;
            return RedirectToAction("Index", "Login");

        }

        }

   
}