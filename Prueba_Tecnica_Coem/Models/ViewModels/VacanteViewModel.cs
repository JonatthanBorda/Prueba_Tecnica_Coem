using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Prueba_Tecnica_Coem.Models;

public class VacanteViewModel
{
    public int Id { get; set; }
    public string Descripcion { get; set; }
    public string Requisitos { get; set; }
    public string Industria { get; set; }
    public int NumeroDeAplicaciones { get; set; }
}


