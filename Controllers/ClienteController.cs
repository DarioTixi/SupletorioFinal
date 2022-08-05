using FacturaWeb.Models;
using FacturaWeb.Permisos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacturaWeb.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {

           Respuesta respuesta = new Respuesta();

    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);
        // GET: Cliente
        //[PermisoRol(Rol.administrador)]
        public ActionResult Cliente()
        {
            Usuario sessionUser = (Usuario)Session["Usuario"];
            ViewBag.UsuarioAdm = sessionUser.idRol == Rol.administrador ? "0" : "1";
            return View();
        }
        public string AgregarCliente(Cliente cliente)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);

                SqlCommand command = new SqlCommand("SP_AgregarClientes", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Cedula   ", cliente.Cedula                   );
                command.Parameters.AddWithValue("@Nombre   ", cliente.NomCliente.Split(' ')[0]);
                command.Parameters.AddWithValue("@Apellido ", cliente.NomCliente.Split(' ')[1]);
                command.Parameters.AddWithValue("@Telefono ", cliente.Telefono                  );
                command.Parameters.AddWithValue("@Direccion", cliente.Direccion                );
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

        public string EditarCliente(Cliente cliente)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);

                SqlCommand command = new SqlCommand("SP_EditarCliente", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ID"     ,   cliente.ClienteID               );
                command.Parameters.AddWithValue("@Cedula   ", cliente.Cedula                  );
                command.Parameters.AddWithValue("@Nombre   ", cliente.NomCliente.Split(' ')[0]);
                command.Parameters.AddWithValue("@Apellido ", cliente.NomCliente.Split(' ')[1]);
                command.Parameters.AddWithValue("@Telefono ", cliente.Telefono                );
                command.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                command.Parameters.AddWithValue("@Estado", cliente.Estado);
                connection.Open();
                int ID = Convert.ToInt32(command.ExecuteNonQuery());


                respuesta.Datos = false;

                if (ID <= 0)
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

        public string EliminarCliente(Cliente cliente)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);

                SqlCommand command = new SqlCommand("SP_EliminarCliente", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ID", cliente.ClienteID);
                connection.Open();
                int ID = Convert.ToInt32(command.ExecuteNonQuery());


                respuesta.Datos = false;

                if (ID <= 0)
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