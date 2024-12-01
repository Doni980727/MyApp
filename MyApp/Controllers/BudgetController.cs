using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyApp.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly AppDbContext _context;

        public BudgetController(AppDbContext context)
        {
            _context = context;
        }

        /*private List<Transaction> GetTransactionsFromSession()
        {
            var sessionData = HttpContext.Session.GetString("Transactions");
            if (sessionData != null)
            {
                return JsonConvert.DeserializeObject<List<Transaction>>(sessionData);
            }
            return new List<Transaction>();
        }

        private void SaveTransactionsToSession(List<Transaction> transactions)
        {
            var sessionData = JsonConvert.SerializeObject(transactions);
            HttpContext.Session.SetString("Transactions", sessionData);
        }

        // Private method to calculate totals
        private void CalculateTotals()
        {
            var transactions = GetTransactionsFromSession();

            decimal totalIncome = 0;
            decimal totalExpense = 0;

            foreach (var transaction in transactions)
            {
                if (transaction.Type == "Income")
                    totalIncome += transaction.Amount;
                else
                    totalExpense += transaction.Amount;
            }

            decimal balance = totalIncome - totalExpense;

            HttpContext.Session.SetString("TotalIncome", totalIncome.ToString());
            HttpContext.Session.SetString("TotalExpense", totalExpense.ToString());
            HttpContext.Session.SetString("Balance", balance.ToString());
        }

        private (decimal totalIncome, decimal totalExpense, decimal balance) GetTotalsFromSession()
        {
            decimal.TryParse(HttpContext.Session.GetString("TotalIncome"), out decimal totalIncome);
            decimal.TryParse(HttpContext.Session.GetString("TotalExpense"), out decimal totalExpense);
            decimal.TryParse(HttpContext.Session.GetString("Balance"), out decimal balance);

            return (totalIncome, totalExpense, balance);
        }*/

        public IActionResult Index()
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);

            var transactions = _context.Transaction.Where(t => t.UsersId == currentUser.UsersId).ToList();

            decimal totalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            decimal totalExpense = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            decimal balance = totalIncome - totalExpense;

            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.Balance = balance;

            return View(transactions);
        }

        public IActionResult AddTransaction()
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            var transaction = _context.Transaction.Where(t => t.UsersId == currentUser.UsersId).ToList();

            decimal totalIncome = transaction.Where(t => t.Type == "Income").Sum(t => t.Amount);
            decimal totalExpense = transaction.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            decimal balance = totalIncome - totalExpense;

            ViewBag.Balance = balance;

            return View();
        }

        [HttpPost]
        public IActionResult AddTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);

                if (currentUser != null)
                {
                    transaction.UsersId = currentUser.UsersId;

                    _context.Transaction.Add(transaction);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(transaction);
        }

        [HttpGet]
        public IActionResult EditTransaction(int id)
        { 
            var transaction = _context.Transaction.FirstOrDefault(t => t.TransactionId == id && 
                t.UsersId == _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name).UsersId);

            if (transaction == null)
            {
                return NotFound();
            }

            ViewBag.Type = transaction.Type;
            ViewBag.Description = transaction.Description;

            return View(transaction);
        }

        [HttpPost]
        public IActionResult EditTransaction(Transaction updatedTransaction)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
                var transaction = _context.Transaction.FirstOrDefault(
                    t => t.TransactionId == updatedTransaction.TransactionId && t.UsersId == currentUser.UsersId);

                if (transaction == null)
                {
                    return NotFound();
                }

                transaction.Amount = updatedTransaction.Amount;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(updatedTransaction);
        }

        [HttpPost]
        public IActionResult DeleteTransaction(int id)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            var transaction = _context.Transaction.FirstOrDefault(t => t.TransactionId == id && t.UsersId == currentUser.UsersId);

            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transaction.Remove(transaction);
            _context.SaveChanges();
            return RedirectToAction("Index"); 
        }

        /*public IActionResult Shop()
        {
            ViewData["Item1"] = new Item() { Id = 1, Name = "Sweater", Price = 399 };
            ViewData["Item2"] = new Item() { Id = 2, Name = "Chinos", Price = 799 };
            ViewData["Item3"] = new Item() { Id = 3, Name = "Shirt", Price = 699 };
            ViewData["Item4"] = new Item() { Id = 4, Name = "Jacket", Price = 2199 };
            ViewData["Item5"] = new Item() { Id = 5, Name = "Shoes", Price = 899 };

            var (totalIncome, totalExpense, balance) = GetTotalsFromSession();

            ViewBag.Balance = balance;

            return View(); 
        }

        [HttpPost]
        public IActionResult Shop(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var transactions = GetTransactionsFromSession();
                transactions.Add(transaction);
                SaveTransactionsToSession(transactions);
                CalculateTotals();
                return RedirectToAction("Shop");
            }
            return View(transactions);
        }*/
    }
}
