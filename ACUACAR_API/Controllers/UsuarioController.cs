using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ACUACAR_API.Models;
using Microsoft.AspNetCore.Cors;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace ACUACAR_API.Controllers
{
    [EnableCors("reglasCors")]

    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly string cadenaSQL;

        public UsuarioController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSql");
        }

        [HttpPost]
        [Route("Registro")]

        public IActionResult Registro([FromBody] Usuario objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("[dbo].[RegistrarUsuario]", conexion);
                    cmd.Parameters.AddWithValue("Nombre", objeto.Nombre);
                    cmd.Parameters.AddWithValue("Email", objeto.Email);
                    cmd.Parameters.AddWithValue("Contrasenia", objeto.Contrasenia);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Ejecutar el comando y obtener el idUsuario creado
                    int idUsuario = Convert.ToInt32(cmd.ExecuteScalar());

                    // Consultar el usuario creado con el idUsuario obtenido
                    cmd = new SqlCommand("SELECT * FROM [dbo].[Usuarios] WHERE idUsuario = @idUsuario", conexion);
                    cmd.Parameters.AddWithValue("idUsuario", idUsuario);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Leer el resultado de la consulta y devolver el objeto usuario
                    if (reader.Read())
                    {
                        Usuario usuarioCreado = new Usuario();
                        usuarioCreado.IdUsuario = (int)reader["idUsuario"];
                        usuarioCreado.Nombre = (string)reader["nombre"];
                        usuarioCreado.Email = (string)reader["email"];
                        return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", usuarioCreado });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status200OK, new { mensaje = "Error al obtener el usuario creado" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message });
            }
        }


        [HttpPost]
        [Route("Ingresar")]
        public IActionResult Ingresar([FromBody] Usuario objeto)
        {
            try
            {
                // creamos la conexion
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("[dbo].[ValidarCredenciales]", conexion);
                    // Agregar los parámetros al comando
                    cmd.Parameters.AddWithValue("Email", objeto.Email);
                    cmd.Parameters.AddWithValue("Contrasenia", objeto.Contrasenia);
                    // Indicar que es un proceso almacenado
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Usuario usuario = new Usuario();
                            usuario.IdUsuario = (int)reader["idUsuario"];
                            usuario.Nombre = (string)reader["nombre"];
                            usuario.Email = (string)reader["email"];
                            return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", usuario });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status200OK, new { mensaje = "Error al leer el resultado" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message });
            }
        }


    }
}
