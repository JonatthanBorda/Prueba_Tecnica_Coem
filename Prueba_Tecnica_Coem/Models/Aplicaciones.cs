using System;
using System.Collections.Generic;

namespace Prueba_Tecnica_Coem.Models;

public partial class Aplicaciones
{
    public int Id { get; set; }

    public int IdVacante { get; set; }

    public int IdDemandante { get; set; }

    public DateTime FechaAplicacion { get; set; }

    public int IdEstado { get; set; }

    public virtual Demandantes IdDemandanteNavigation { get; set; } = null!;

    public virtual EstadoAplicacion IdEstadoNavigation { get; set; } = null!;

    public virtual Vacantes IdVacanteNavigation { get; set; } = null!;
}
