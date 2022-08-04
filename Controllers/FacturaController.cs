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
    
    public class FacturaController : Controller
    {
        // GET: Factura
        
        public ActionResult Facturar()
        {

            return View();
        }

        public string GuardarFactura(Cliente cliente, List<Producto> productos)
        {

            Respuesta respuesta = new Respuesta();

            decimal sum = 0;
            foreach (var item in productos)
            {
                item.Precio = item.Precio.Replace('.',',');
                decimal d = Convert.ToDecimal(item.Cantidad) * Convert.ToDecimal(item.Precio);
                sum += d;
                item.Total = d;
                
            }


            decimal iva = sum * Convert.ToDecimal(0.12);
            decimal total = sum + iva;

            SqlTransaction objTrans = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString))
            {
                connection.Open();
                objTrans = connection.BeginTransaction();
                Usuario sessionUser = (Usuario)Session["Usuario"];
                SqlCommand command = new SqlCommand("SP_GuardarFacturabyUser", connection);
                command.Connection = connection;
                command.Transaction = objTrans;
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    command.Parameters.AddWithValue("@Cliente", cliente.ClienteID);
                    command.Parameters.AddWithValue("@Sub", sum);
                    command.Parameters.AddWithValue("@IVA", iva);
                    command.Parameters.AddWithValue("@Total", total);
                    command.Parameters.AddWithValue("@idUsuario", sessionUser.id);

                    int ID = Convert.ToInt32(command.ExecuteScalar());

                    if (ID <= 0)
                    {
                        objTrans.Rollback();
                        respuesta.Error = " No se pudo Guardar";
                        return JsonConvert.SerializeObject(respuesta);
                    }

                    foreach (var item in productos)
                    {
                        command.CommandText = "SP_GuardarDetalleFac";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@Factura", ID);
                        command.Parameters.AddWithValue("@Producto", item.ProductoID);
                        command.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                        command.Parameters.AddWithValue("@Precio", item.Precio.Replace(',','.'));
                        command.Parameters.AddWithValue("@Subtotal", item.Total);

                        int n = Convert.ToInt32(command.ExecuteScalar());
                        if (n == 0)
                        {
                            objTrans.Rollback();
                            respuesta.Error = "Stock Insuficiente para el producto: " + item.NomProducto;
                            return JsonConvert.SerializeObject(respuesta);
                        }
                    }


                    objTrans.Commit();
                    
                }
                catch (Exception ex)
                {
                    objTrans.Rollback();
                    respuesta.Error = ex.Message.Replace("'", "").Replace("\"", "");
                    return JsonConvert.SerializeObject(respuesta);
                }

                connection.Close();
            }

            return JsonConvert.SerializeObject(respuesta);
        }

        public string ObtenerProductos()
        {
            Respuesta respuesta = new Respuesta();
            List<Producto> clientes = new List<Producto>();
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("SP_ObtenerProductos", connection);
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
       
        public string ObtenerClientes()
        {
            Respuesta respuesta = new Respuesta();
            List<Cliente> clientes = new List<Cliente>();
            try
            {
                Usuario sessionUser = (Usuario)Session["Usuario"];
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("SP_ObtenerClientes", connection);
                command.Connection = connection;
                //command.Parameters.AddWithValue("","")
                int nameProcedure = sessionUser.idRol == Rol.administrador
                   ? 0
                   : 1;
                command.Parameters.AddWithValue("@ID", nameProcedure);
                int i = 1;
                command.CommandType = CommandType.StoredProcedure;
                using (var Reader = command.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        Cliente c = new Cliente();
                        c.Num = i++;
                        c.ClienteID = Reader.GetInt32(0);
                        c.Cedula = Reader.GetString(1);
                        c.NomCliente = Reader.GetString(2);
                        c.Telefono = Reader.GetString(3);
                        c.Direccion = Reader.GetString(4);
                        c.Estado = Reader.GetInt32(5);
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

        public ActionResult DetalleFactura(string IDFactura)
        {
            if (string.IsNullOrWhiteSpace(IDFactura))
            {
                return RedirectToAction("Index", "Inicio");
            }


            Respuesta respuesta = new Respuesta();
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexion"].ConnectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("SP_ObtenerFacturaID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", IDFactura);
                command.Connection = connection;
                Factura factura = new Factura();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        factura.Id_Venta = reader.GetInt32(0);
                        factura.Fecha = reader.GetDateTime(1).ToString();
                        factura.Cedula = reader.GetString(2);
                        factura.Nombre = reader.GetString(3);
                        factura.Apellido = reader.GetString(4);
                        factura.Direccion = reader.GetString(5);
                        factura.Telefono = reader.GetString(6);
                        factura.Subtotal = reader.GetDecimal(7).ToString();
                        factura.Iva = reader.GetDecimal(8).ToString();
                        factura.Total = reader.GetDecimal(9).ToString();
                        
                    }
                }

                command.CommandText = "SP_ObtenerDetalleIDVenta";
                int i = 1;
                using (var Reader = command.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        Producto c = new Producto();
                        c.Num = i++;
                        c.NomProducto = Reader.GetString(2);
                        c.Cantidad = Reader.GetInt32(3);
                        c.Precio = Reader.GetDecimal(4).ToString();
                        c.Total = Reader.GetDecimal(5);
                        factura.Productos.Add(c);

                    }
                }



                respuesta.Datos = factura;


                
            }
            catch (Exception ex)
            {
                respuesta.Error = ex.Message;
                
            }


            return View(respuesta.Datos);
        }
    }
}