using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Prueba_Tecnica_Coem.Models;

public partial class Demandantes
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Apellidos { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Celular { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdNivelEducativo { get; set; }

    public string? Notas { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string ExperienciaAnterior { get; set; } = null!;

    public virtual ICollection<Aplicaciones> Aplicaciones { get; set; } = new List<Aplicaciones>();

    public virtual NivelEducativo? IdNivelEducativoNavigation { get; set; } = null!;

    public virtual Usuarios? IdUsuarioNavigation { get; set; } = null!;
}
