using ACUACAR_API.Models;

namespace ACUACAR_API.Models
{
    public class Ejecucion
    {
        public int IdTarea { get; set; }
        public int IdUsuario { get; set; }
        public string FechaEjecucion { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public int Duracion { get; set; }
        public string observaciones { get; set; }
    }
}
