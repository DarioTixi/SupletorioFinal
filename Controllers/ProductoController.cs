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
    public class ProductoController : Controller

    { 
      Respuesta respuesta = new Respuesta();
    List<Producto> facturas = new List<Producto>();
    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);
        // GET: Productos
        public ActionResult Productos()
        {
            Usuario sessionUser = (Usuario)Session["Usuario"];
            ViewBag.UsuarioAdm = sessionUser.idRol == Rol.administrador ? "0" : "1";
            return View();
        }

        public string AgregarProducto(Producto producto)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);

                SqlCommand command = new SqlCommand("SP_AgregarProducto", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Producto", producto.NomProducto);
                command.Parameters.AddWithValue("@Stock", producto.Stock);
                command.Parameters.AddWithValue("@Precio", producto.Precio);
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

        public string EditarProducto(Producto producto)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);

                SqlCommand command = new SqlCommand("SP_EditarProducto", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ID", producto.ProductoID);
                command.Parameters.AddWithValue("@Producto", producto.NomProducto);
                command.Parameters.AddWithValue("@Stock", producto.Stock);
                command.Parameters.AddWithValue("@Precio", producto.Precio);
                command.Parameters.AddWithValue("@Estado", producto.Estado);
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

        public string EliminarProducto(Producto producto)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);

                SqlCommand command = new SqlCommand("SP_EliminarProducto", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ID", producto.ProductoID);
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

        public string ObtenerProductos()
        {
            Respuesta respuesta = new Respuesta();
            List<Producto> clientes = new List<Producto>();
            try
            {
                string nameProcedure = "";
                Usuario sessionUser = (Usuario)Session["Usuario"];

                nameProcedure = sessionUser.idRol == Rol.administrador
                    ? "SP_ObtenerProductosActivosEinactivos"
                    : "SP_ObtenerProductos";


                                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);
                connection.Open();
                SqlCommand command = new SqlCommand(nameProcedure, connection);
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                int i = 1;
                using (var Reader = command.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        Producto c = new Producto();
                        c.Num = i++;
                        c.ProductoID = Reader.GetInt32(0);
                        c.NomProducto = Reader.GetString(1);
                        c.Precio = Reader.GetDecimal(2).ToString();
                        c.Stock = Reader.GetInt32(3).ToString();
                        c.Estado = Reader.GetInt32(4);
                        clientes.Add(c);
                    }
                }
                connection.Close();
                respuesta.Datos = clientes;
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