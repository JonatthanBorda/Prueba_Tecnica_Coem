using System;
using System.Collections.Generic;

namespace Prueba_Tecnica_Coem.Models;

public partial class TipoUsuario
{
    public int Id { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
}
