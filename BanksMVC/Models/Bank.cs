using System.ComponentModel.DataAnnotations;

namespace BanksMVC.Models
{
    public class Bank
    {
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        public Bank() { }
        public Bank(Guid id, string name, decimal total) 
        {
            Id = id;
            Name = name;
            Total = total;
        }
    }
}