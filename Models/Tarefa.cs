using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ToDoList_Projeto_Portifólio.Models
{
    public class Tarefa
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha a descrição")]
        public required string Descricao { get; set; }

        [Required(ErrorMessage = "Preencha a data de vencimento")]
        public DateTime? DataDeVencimento { get; set; }

        [Required(ErrorMessage = "Selecione uma categoria")]
        public required string CategoriaId { get; set; }

        [ValidateNever]
        public required Categoria Categoria { get; set; }

        [Required(ErrorMessage = "Selecione um Status")]
        public required string StatusId { get; set; }
        [ValidateNever]
        public Status Status { get; set; }  
        public bool Atrasado => StatusId == "aberto" && DataDeVencimento < DateTime.Today;
    }
}

            
