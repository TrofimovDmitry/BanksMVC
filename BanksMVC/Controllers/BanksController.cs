using BanksMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BanksMVC.Controllers
{

    public class BanksController : Controller
    {
        public MySqlDbLib.MySqlDbLib DbLib { get; }

        public BanksController(MySqlDbLib.MySqlDbLib db) => DbLib = db;
        
        public enum Banks
        {
            [Display(Name = "ВТБ")]
            VTB,
            [Display(Name = "Сбербанк")]
            Sber,
            [Display(Name = "Тиньков")]
            Tinkov,
            [Display(Name = "Альфа-Банк")]
            Alpha,
            [Display(Name = "Промсвязьбанк")]
            PSB
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetBankNames()
        {
            var banksDict = new Dictionary<string, string>();
            foreach (var bankName in Enum.GetNames<Banks>())
            {
                var bank = Enum.Parse(typeof(Banks), bankName);
                var dispName = bank.GetType().GetMember(bank.ToString()).First().GetCustomAttribute<DisplayAttribute>();
                System.Diagnostics.Debug.WriteLine(dispName);
                banksDict.Add(bankName, dispName.Name);
            }
            return Json(banksDict);
        }

        // POST: Banks/Create?bankName=N&amount=A
        [HttpPost]
        public async Task<IActionResult> Create(string bankName, string amount)
        {
            amount = amount.Replace(".", ",");
            if (Enum.IsDefined(typeof(Banks), bankName))
            {
                decimal amountNum;
                if (Decimal.TryParse(amount, out amountNum))
                {
                    amountNum = CalculateAmount((int)Enum.Parse(typeof(Banks), bankName), amountNum);
                    Bank bank = await GetBankByName(bankName);
                    if (await GetBankByName(bankName) is null)
                        await DbLib.CreateBank(Guid.NewGuid(), bankName, 0);
                    decimal total = bank is null ? 0 : bank.Total;
                    total = Decimal.Add(amountNum, total);
                    await DbLib.SetBankTotal(bankName, total);
                } else
                {
                    Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return new JsonResult(new Dictionary<string, string>() {
                        {"message", "Bad input data"} });
                }
               
            } else
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Dictionary<string, string>() {
                        {"message", "Unknown bank name"} });
            }
            return Json(new Dictionary<string, string>() { { "message", "Create success" } });
        }

        private async Task<Bank> GetBankByName(string bankName)
        {
            List<object> bankParams = await DbLib.GetBankParamsByName(bankName);
            if (bankParams is null)
                return null;
            else
                return new Bank(Guid.Parse(bankParams[0].ToString()), bankParams[1].ToString(), (Decimal)(bankParams[2]));
        }

        public decimal CalculateAmount(int bankNum, decimal amount)
        {
            switch (bankNum)
            {
                case 0:
                    amount = Decimal.Multiply(amount, 3);
                    break;
                case 1:
                    amount -= Decimal.Multiply(amount, 0.5M);
                    break;
                case 2:
                    amount = amount + Decimal.Multiply(amount, 0.5M) - 100;
                    amount = amount < 0 ? 0 : amount;
                    break;
            }
            return amount;
        }

        // GET: Banks/Read
        [HttpGet]
        public async Task<IActionResult> Read()
        {
            var banksParams = await DbLib.GetBanksParams();
            List<Bank> banksList = new List<Bank>();
            foreach (List<object> bankParams in banksParams)
            {
                banksList.Add(new Bank(Guid.Parse(bankParams[0].ToString()), bankParams[1].ToString(), (Decimal)bankParams[2]));
            }
            return Json(banksList);
        }

        // POST: Banks/Update?bankName=N&amount=A
        [HttpPost]
        public async Task<IActionResult> Update(string bankName, decimal amount)
        {
            if (Enum.IsDefined(typeof(Banks), bankName))
            {
                //Bank bank = await GetBankByName(bankName);
                if (await GetBankByName(bankName) is null)
                    await DbLib.CreateBank(Guid.NewGuid(), bankName, 0);
                await DbLib.SetBankTotal(bankName, amount);
            }
            else
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Dictionary<string, string>() {
                    {"message", $"Bank {bankName} not found"} });
            }
            return new JsonResult(new Dictionary<string, string>() {
                {"message", $"Balance of {bankName} was updated. Amount: {amount}"} });
        }

        //POST: Banks/Delete?bankName=N
        [HttpPost]
        public async Task<IActionResult> Delete(string bankName)
        {
            if (Enum.IsDefined(typeof(Banks), bankName))
            {
                //Bank bank = await GetBankByName(bankName);
                if (await GetBankByName(bankName) is null)
                    await DbLib.CreateBank(Guid.NewGuid(), bankName, 0);
                await DbLib.SetBankTotal(bankName, 0);
                return new JsonResult(new Dictionary<string, string>() {
                {"message", $"Balance of {bankName} was reset"} });
            }
            else
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Dictionary<string, string>() { 
                    {"message", $"Bank {bankName} not found"} });
            }
        }
    }
}
