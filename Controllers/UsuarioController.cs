using FacturaWeb.Models;
using FacturaWeb.Permisos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FacturaWeb.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        // GET: Usuario
        [PermisoRol(Rol.administrador)]
        public ActionResult Usuario()
        {

            Usuario sessionUser = (Usuario)Session["Usuario"];
            ViewBag.UsuarioAdm = sessionUser.idRol == Rol.administrador ? "0" : "1";

            return View();
        }

        public string ObtenerUsuarios()
        {
            Respuesta respuesta = new Respuesta();
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                Usuario sessionUser = (Usuario)Session["Usuario"];
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("SP_ObtenerUsuarios", connection);
                command.Connection = connection;
                //command.Parameters.AddWithValue("","")
                int nameProcedure = sessionUser.idRol == Rol.administrador
                   ? 0
                   : 1;
                command.Parameters.AddWithValue("@ID", nameProcedure);
                command.CommandType = CommandType.StoredProcedure;
                using (var Reader = command.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        Usuario c = new Usuario();
                        c.id = Reader.GetInt32(0);
                        c.email= Reader.GetString(1);
                        c.idRol = (Rol)Reader.GetInt32(2);
                        c.estado = Reader.GetInt32(3);
                        usuarios.Add(c);
                    }
                }
                connection.Close();
                respuesta.Datos = usuarios;
                return JsonConvert.SerializeObject(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Error = ex.Message;
                return JsonConvert.SerializeObject(respuesta);
            }
        }
        public string AgregarUsuario(Usuario usuario)
        {
            var lowerCaseRegex = "(?=.*[a-z])";
            var upperCaseRegex = "(?=.*[A-Z])";
            var symbolsRegex = @"(?=.*[!@#$%^&+/',\""*])";
            var numericRegex = "(?=.*[0-9])";

            Respuesta respuesta = new Respuesta();

            if (!new Regex(@"^" + lowerCaseRegex + "").IsMatch(usuario.password))
            {
                respuesta.Datos = false;
                respuesta.Error = "Debe tener una letra minuscula";
                return JsonConvert.SerializeObject(respuesta);
            }
            if (!new Regex(@"^" + upperCaseRegex + "").IsMatch(usuario.password))
            {
                respuesta.Datos = false;
                respuesta.Error = "Debe tener una letra mayuscula";

                return JsonConvert.SerializeObject(respuesta);
            }
            if (!new Regex(@"^" + symbolsRegex + "").IsMatch(usuario.password))
            {
                respuesta.Datos = false;
                respuesta.Error = "Debe tener una caracter especial";

                return JsonConvert.SerializeObject(respuesta);
            }
            if (!new Regex(@"^" + numericRegex + "").IsMatch(usuario.password))
            {
                respuesta.Datos = false;
                respuesta.Error = "Debe tener un numero";

                return JsonConvert.SerializeObject(respuesta);
            }

            if (usuario.password.Length <= 8)
            {
                respuesta.Datos = false;
                return JsonConvert.SerializeObject(respuesta);
            }



            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);

                SqlCommand command = new SqlCommand("SP_AgregarUsuario", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@email   ", usuario.email);
                command.Parameters.AddWithValue("@password   ", usuario.password);
                command.Parameters.AddWithValue("@idRol ", usuario.idRol);
                connection.Open();
                int ID = Convert.ToInt32(command.ExecuteNonQuery());

                respuesta.Datos = false;

                if (ID > 0)
                {
                    respuesta.Datos = true;
                }
                connection.Close();
                return JsonConvert.SerializeObject(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Error = ex.Message;
                return JsonConvert.SerializeObject(respuesta);
            }
        }

        public string EditarUsuario(Usuario usuario)
        {
            Respuesta respuesta = new Respuesta();
            usuario.password = usuario.password == null ? 0.ToString() : usuario.password;
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);


                SqlCommand command = new SqlCommand("SP_EditarUsuario", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@id   ", usuario.id);
                command.Parameters.AddWithValue("@email   ", usuario.email);
                command.Parameters.AddWithValue("@password   ",  usuario.password);
                command.Parameters.AddWithValue("@idRol ", usuario.idRol);
                command.Parameters.AddWithValue("@estado ", usuario.estado);

                connection.Open();
                int ID = Convert.ToInt32(command.ExecuteNonQuery());

                respuesta.Datos = false;

                if (ID > 0)
                {
                    respuesta.Datos = true;
                }
                connection.Close();
                return JsonConvert.SerializeObject(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Error = ex.Message;
                return JsonConvert.SerializeObject(respuesta);
            }
        }

        public string EliminarUsuario(Usuario usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);

                SqlCommand command = new SqlCommand("SP_EliminarUsuario", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@id  ", usuario.id);

                connection.Open();
                int ID = Convert.ToInt32(command.ExecuteNonQuery());

                respuesta.Datos = false;

                if (ID > 0)
                {
                    respuesta.Datos = true;
                }
                connection.Close();
                return JsonConvert.SerializeObject(respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Error = ex.Message;
                return JsonConvert.SerializeObject(respuesta);
            }
        }
    }
}