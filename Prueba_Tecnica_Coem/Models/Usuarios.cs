using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Prueba_Tecnica_Coem.Models;

public partial class Usuarios
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string? Clave { get; set; } = null!;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdTipoUsuario { get; set; }

    public virtual ICollection<Demandantes> Demandantes { get; set; } = new List<Demandantes>();

    public virtual ICollection<Empleadores> Empleadores { get; set; } = new List<Empleadores>();

    public virtual TipoUsuario? IdTipoUsuarioNavigation { get; set; } = null!;
}
