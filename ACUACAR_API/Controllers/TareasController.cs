using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ACUACAR_API.Models;

using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.AspNetCore.Cors;

namespace ACUACAR_API.Controllers
{
    [EnableCors("reglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly string cadenaSQL;

        public TareasController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSql");
        }


        [HttpPost]
        [Route("crear-tarea")]
        public IActionResult CrearTarea([FromBody] Tarea objeto)
        {
            try
            {
                // creamos la conexion
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("[dbo].[CrearTarea]", conexion);
                    // Agregar los parámetros al comando
                    cmd.Parameters.AddWithValue("IdUsuario", objeto.IdUsuario);
                    cmd.Parameters.AddWithValue("NombreTarea", objeto.NombreTarea);
                    // Indicar que es un proceso almacenado
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();

                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message });
            }
        }

        [HttpGet]
        [Route("tareas-por-usuario/{idUsuario}")]
        public IActionResult TareasPorUsuario(int idUsuario)
        {
            List<Tarea> listaTareas = new List<Tarea>();

            try
            {
                // creamos la conexion
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("[dbo].[TareasPorUsuario]", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using(var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            listaTareas.Add(new Tarea()
                            {
                                IdTarea = Convert.ToInt32( rd["IdTarea"] ),
                                IdUsuario = idUsuario,
                                NombreTarea = rd["NombreTarea"].ToString(),
                            });
                        }
                    }

                    if (listaTareas.Count == 0)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { mensaje = "El usuario no tiene tareas asociadas" });
                    }
                    else
                    {
                        // Retornamos las tareas como resultado
                        return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = listaTareas }); ;
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
