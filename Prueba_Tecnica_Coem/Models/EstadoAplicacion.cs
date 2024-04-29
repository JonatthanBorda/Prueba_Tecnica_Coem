using System;
using System.Collections.Generic;

namespace Prueba_Tecnica_Coem.Models;

public partial class EstadoAplicacion
{
    public int Id { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<Aplicaciones> Aplicaciones { get; set; } = new List<Aplicaciones>();
}
