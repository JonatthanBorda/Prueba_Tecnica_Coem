namespace Prueba_Tecnica_Coem.Models
{
    public class Enum
    {
        public enum TiposUsuario
        {
            Demandante = 1,
            Empleador
        }

        public enum EstadosAplicacion
        {
            Enviada = 1,
            Vista,
            EnProceso,
            Finalizada
        }

        public enum NivelEducativo
        {
            SinEstudiosFormales = 1,
            Primaria_Bachillerato,
            Profesional_Técnico_Tecnólogo,
            Pregrado
        }
    }
}
