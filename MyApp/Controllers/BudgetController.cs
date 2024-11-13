using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyApp.Controllers
{
    public class BudgetController : Controller
    {
        private static List<Transaction> transactions = new List<Transaction>();

        private List<Transaction> GetTransactionsFromSession()
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
        }

        public IActionResult Index()
        {
            var transactions = GetTransactionsFromSession();
            var (totalIncome, totalExpense, balance) = GetTotalsFromSession();

            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.Balance = balance;

            return View(transactions);
        }

        public IActionResult Shop()
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
        }

        public IActionResult AddTransaction()
        {
            var (totalIncome, totalExpense, balance) = GetTotalsFromSession();

            ViewBag.Balance = balance;

            return View();
        }

        [HttpPost]
        public IActionResult AddTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var transactions = GetTransactionsFromSession();
                transactions.Add(transaction);
                SaveTransactionsToSession(transactions);
                CalculateTotals();
                return RedirectToAction("Index");
            }

            var (totalIncome, totalExpense, balance) = GetTotalsFromSession();

            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.Balance = balance;

            return View(transaction);
        }
    }
}
