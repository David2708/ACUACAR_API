using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ACUACAR_API.Models;

using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Cors;

namespace ACUACAR_API.Controllers
{
    [EnableCors("reglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class EjecucionesController : ControllerBase
    {
        private readonly string cadenaSQL;

        public EjecucionesController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSql");
        }

        [HttpPost]
        [Route("Registro")]

        public IActionResult Registro([FromBody] Ejecucion objeto)
        {
            try
            {
                // creamos la conexion
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("[dbo].[GuardarEjecucion]", conexion);
                    // Agregar los parámetros al comando
                    cmd.Parameters.AddWithValue("IdTarea", objeto.IdTarea);
                    cmd.Parameters.AddWithValue("IdUsuario", objeto.IdUsuario);
                    cmd.Parameters.AddWithValue("FechaEjecucion", objeto.FechaEjecucion);
                    cmd.Parameters.AddWithValue("HoraInicio", objeto.HoraInicio);
                    cmd.Parameters.AddWithValue("HoraFin", objeto.HoraFin);
                    cmd.Parameters.AddWithValue("Duracion", objeto.Duracion);
                    cmd.Parameters.AddWithValue("Observaciones", objeto.observaciones);
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
    }
}