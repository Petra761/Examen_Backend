using System.ComponentModel.DataAnnotations;

namespace ExamenFinal.Entidades
{
    public class ClienteCredito
    {
        [Key]
        public string ci { get; set; }
        public string nombre { get; set; }
        public int limiteCredito { get; set; }
        public int saldoUsado { get; set; }
    }
}
