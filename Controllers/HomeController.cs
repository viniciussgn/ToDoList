using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ToDoList_Projeto_Portifólio.Data;
using ToDoList_Projeto_Portifólio.Models;

namespace ToDoList_Projeto_Portifólio.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string id)
        {
            var filtros = new Filtros(id);
            ViewBag.Filtros = filtros;
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Status = _context.Statuses.ToList();
            ViewBag.VencimentosValores = Filtros.VencimentosValoresFiltro;

            IQueryable<Tarefa> consulta = _context.Tarefas
                .Include(c => c.Categoria)
                .Include(s => s.Status);

            if (filtros.TemCategoria)
            {
                consulta = consulta.Where(t => t.CategoriaId == filtros.CategoriaId);
            }

            if (filtros.TemStatus)
            {
                consulta = consulta.Where(t => t.StatusId == filtros.StatusId);
            }

            if (filtros.TemVencimento)
            {
                var hoje = DateTime.Today;

                if (filtros.EPassado)
                {
                    consulta = consulta.Where(t => t.DataDeVencimento < hoje);
                }
                else if (filtros.EFuturo)
                {
                    consulta = consulta.Where(t => t.DataDeVencimento > hoje);
                }
                else if (filtros.EHoje)
                {
                    consulta = consulta.Where(t => t.DataDeVencimento == hoje);
                }
            }

            var tarefas = consulta.OrderBy(t => t.DataDeVencimento).ToList();

            return View(tarefas);
        }

        public IActionResult Adicionar()
        {
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Status = _context.Statuses.ToList();

            Tarefa tarefa = new()
            {
                Descricao = string.Empty,
                CategoriaId = string.Empty,
                Categoria = new Categoria
                {
                    CategoriaId = string.Empty,
                    Nome = string.Empty
                },
                StatusId = "aberto"
            };

            return View(tarefa);
        }

        [HttpPost]
        public IActionResult Filtrar(string[] filtro)
        {
            string id = string.Join('-', filtro);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult MarcarCompleto([FromRoute] string id, Tarefa tarefaSelecionada)
        {
            tarefaSelecionada = _context.Tarefas.Find(tarefaSelecionada.Id);

            if (tarefaSelecionada != null)
            {
                tarefaSelecionada.StatusId = "completo";
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult Adicionar(Tarefa tarefa)
        {
            if (ModelState.IsValid)
            {
                _context.Tarefas.Add(tarefa);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categorias = _context.Categorias.ToList();
                ViewBag.Status = _context.Statuses.ToList();
                return View(tarefa);
            }
    
        }

        [HttpPost]

        public IActionResult DeletarCompletos(string id)
        {
            var paraDeletar = _context.Tarefas
                .Where(t => t.StatusId == "completo")
                .ToList();

            foreach (var tarefa in paraDeletar)
            {
                _context.Tarefas.Remove(tarefa);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", new { ID = id });
        }


    }

}
