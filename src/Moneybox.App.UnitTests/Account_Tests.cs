using Moneybox.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests {
    public class Account_Tests {


        [Theory]
        [InlineData(500.56, "e476ed86-5b59-415f-b080-c556c0807756", 1000, 2000, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        [InlineData(100, "e476ed86-5b59-415f-b080-c556c0807756", -1000, 0, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        [InlineData(10, "e476ed86-5b59-415f-b080-c556c0807756", 0, 0, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        [InlineData(0, "e476ed86-5b59-415f-b080-c556c0807756", 0, 0, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        public void Account_DepositMoney_SuccessfullTest(decimal amount, string toAccountIdS, int toBalance, int toPaidIn, string toEmail, string toUserIdS, string toUserName) {
            var toAccountId = Guid.Parse(toAccountIdS);
            var toUserId = Guid.Parse(toUserIdS);
            Account toAccount = new Account() {
                Balance = toBalance,
                Id = toAccountId,
                PaidIn = toPaidIn,
                User = new User() {
                    Email = toEmail,
                    Id = toUserId,
                    Name = toUserName
                }
            };
            toAccount.DepositMoney(amount);

            Assert.Equal(toAccount.Balance, amount + toBalance);
            Assert.Equal(toAccount.PaidIn, amount + toPaidIn);
        }


        [Theory]
        [InlineData(4000, "e476ed86-5b59-415f-b080-c556c0807756", 1000, 2000, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        [InlineData(4001, "e476ed86-5b59-415f-b080-c556c0807756", 50000, 0, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        public void Account_DepositMoney_UnSuccessfullTest(decimal amount, string toAccountIdS, int toBalance, int toPaidIn, string toEmail, string toUserIdS, string toUserName) {
            var toAccountId = Guid.Parse(toAccountIdS);
            var toUserId = Guid.Parse(toUserIdS);
            Account toAccount = new Account() {
                Balance = toBalance,
                Id = toAccountId,
                PaidIn = toPaidIn,
                User = new User() {
                    Email = toEmail,
                    Id = toUserId,
                    Name = toUserName
                }
            };
            Assert.Throws<InvalidOperationException>(() => toAccount.DepositMoney(amount));
            Assert.Equal(toAccount.Balance, toBalance);
            Assert.Equal(toAccount.PaidIn, toPaidIn);
        }


        [Theory]
        [InlineData(500.56, "e476ed86-5b59-415f-b080-c556c0807756", 1000, 2000, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        [InlineData(25, "e476ed86-5b59-415f-b080-c556c0807756", 50, 0, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        public void Account_WithdrawMoney_SuccessfullTest(decimal amount, string fromAccountIdS, int fromBalance, int fromWithdrawn, string fromEmail, string fromUserIdS, string fromUserName) {
            var toAccountId = Guid.Parse(fromAccountIdS);
            var toUserId = Guid.Parse(fromUserIdS);
            Account fromAccount = new Account() {
                Balance = fromBalance,
                Id = toAccountId,
                Withdrawn = fromWithdrawn,
                User = new User() {
                    Email = fromEmail,
                    Id = toUserId,
                    Name = fromUserName
                }
            };
            fromAccount.WithdrawMoney(amount);

            Assert.Equal(fromAccount.Balance, fromBalance - amount);
            Assert.Equal(fromAccount.Withdrawn, fromWithdrawn - amount);
            Assert.Equal(fromAccount.Id, toAccountId);
        }


        [Theory]
        [InlineData(100.14, "e476ed86-5b59-415f-b080-c556c0807756", 50, 2000, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        [InlineData(100, "e476ed86-5b59-415f-b080-c556c0807756", 0, 0, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        [InlineData(15.6, "e476ed86-5b59-415f-b080-c556c0807756", 15, 0, "r2d2@gmail.com", "bfa3b7c8-3ade-474b-a2a4-d84b2b042c2f", "R2D2")]
        public void Account_WithdrawMoney_UnSuccessfullTest(decimal amount, string fromAccountIdS, int fromBalance, int fromWithdrawn, string fromEmail, string fromUserIdS, string fromUserName) {
            var toAccountId = Guid.Parse(fromAccountIdS);
            var toUserId = Guid.Parse(fromUserIdS);
            Account fromAccount = new Account() {
                Balance = fromBalance,
                Id = toAccountId,
                Withdrawn = fromWithdrawn,
                User = new User() {
                    Email = fromEmail,
                    Id = toUserId,
                    Name = fromUserName
                }
            };
            Assert.Throws<InvalidOperationException>(() => fromAccount.WithdrawMoney(amount));
            Assert.Equal(fromAccount.Balance, fromBalance);
            Assert.Equal(fromAccount.Withdrawn, fromWithdrawn);
        }
    }
}
