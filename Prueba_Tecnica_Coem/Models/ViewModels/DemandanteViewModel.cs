using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Prueba_Tecnica_Coem.Models;

public class DemandanteViewModel
{
    public int Id { get; set; }

    [Display(Name = "Nombre empresa")]
    public string RazonSocial { get; set; }
    public string Descripcion { get; set; }
    public string Requisitos { get; set; }
    public string Industria { get; set; }
    public bool HaAplicado { get; set; }

    [Display(Name = "Estado de aplicación")]
    public string EstadoAplicacion { get; set; }
}


