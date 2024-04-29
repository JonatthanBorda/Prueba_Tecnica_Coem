using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Prueba_Tecnica_Coem.Models;

public partial class Vacantes
{
    public int Id { get; set; }

    public int IdEmpleador { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Requisitos { get; set; } = null!;

    public virtual ICollection<Aplicaciones> Aplicaciones { get; set; } = new List<Aplicaciones>();

    [Display(Name = "Industria")]
    public virtual Empleadores? IdEmpleadorNavigation { get; set; } = null!;
}
