using BanksMVC.Controllers;

namespace BanksMvcTestProject
{
    public class BanksDepositLogicUnitTest
    {
        [Fact]
        public void TestBankNum1()
        {
            var controller = new BanksMVC.Controllers.BanksController(new MySqlDbLib.MySqlDbLib(""));
            int bankNum = (int)BanksController.Banks.VTB;
            decimal amount = 200;
            decimal actualResult = Decimal.Multiply(amount, 3);
            decimal result = controller.CalculateAmount(bankNum, amount); // amount * 3
            Assert.True(result == actualResult, $"Result should be {actualResult} instead of {result}");

            amount = 50.75M;
            actualResult = Decimal.Multiply(amount, 3);
            result = controller.CalculateAmount(bankNum, amount); // amount * 3
            Assert.True(result == actualResult, $"Result should be {actualResult} instead of {result}");
        }

        [Fact]
        public void TestBankNum2()
        {
            var controller = new BanksMVC.Controllers.BanksController(new MySqlDbLib.MySqlDbLib(""));
            int bankNum = (int)BanksController.Banks.Sber;
            decimal amount = 500;
            decimal actualResult = amount - (Decimal.Multiply(amount, 0.5M));
            decimal result = controller.CalculateAmount(bankNum, amount); // amount - 50%
            Assert.True(result == actualResult, $"Result should be {actualResult} instead of {result}");

            amount = 175.19M;
            actualResult = amount - (Decimal.Multiply(amount, 0.5M));
            result = controller.CalculateAmount(bankNum, amount); // amount - 50%
            Assert.True(result == actualResult, $"Result should be {actualResult} instead of {result}");
        }

        [Fact]
        public void TestBankNum3()
        {
            var controller = new BanksMVC.Controllers.BanksController(new MySqlDbLib.MySqlDbLib(""));
            int bankNum = (int)BanksController.Banks.Tinkov;
            decimal amount = 300;
            decimal actualResult = amount + Decimal.Multiply(amount, 0.5M) - 100;
            decimal result = controller.CalculateAmount(bankNum, amount); // amount + 50% - 100
            Assert.True(result == actualResult, $"Result should be {actualResult} instead of {result}");

            amount = 99.34M;
            actualResult = amount + Decimal.Multiply(amount, 0.5M) - 100;
            result = controller.CalculateAmount(bankNum, amount); // amount + 50% - 100
            Assert.True(result == actualResult, $"Result should be {actualResult} instead of {result}");
            
            amount = 25.5M;
            actualResult = amount + Decimal.Multiply(amount, 0.5M) - 100;
            actualResult = actualResult < 0 ? 0 : actualResult;
            result = controller.CalculateAmount(bankNum, amount); // amount + 50% - 100
            Assert.True(result == actualResult, $"Result should be {actualResult} instead of {result}");
        }

        [Fact]
        public void TestOtherBankNums()
        {
            var controller = new BanksMVC.Controllers.BanksController(new MySqlDbLib.MySqlDbLib(""));
            int bankNum = (int)BanksController.Banks.Alpha;
            decimal amount = 800;
            decimal result = controller.CalculateAmount(bankNum, amount); // = amount
            Assert.True(result == amount, $"Result should be {amount} instead of {result}");

            bankNum = (int)BanksController.Banks.PSB;
            amount = 700;
            result = controller.CalculateAmount(bankNum, amount); // = amount
            Assert.True(result == amount, $"Result should be {amount} instead of {result}");
        }
    }
}