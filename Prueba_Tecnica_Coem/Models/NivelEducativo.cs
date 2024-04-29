using System;
using System.Collections.Generic;

namespace Prueba_Tecnica_Coem.Models;

public partial class NivelEducativo
{
    public int Id { get; set; }

    public string Nivel { get; set; } = null!;

    public virtual ICollection<Demandantes> Demandantes { get; set; } = new List<Demandantes>();
}
