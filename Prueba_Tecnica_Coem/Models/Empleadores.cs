using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Prueba_Tecnica_Coem.Models;

public partial class Empleadores
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string RazonSocial { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Localizacion { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Industria { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int NumeroEmpleados { get; set; }

    public virtual Usuarios? IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Vacantes> Vacantes { get; set; } = new List<Vacantes>();
}
