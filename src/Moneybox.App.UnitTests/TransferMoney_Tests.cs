using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App;
using System;
using Xunit;
using Moq;
using Moneybox.App.Features;

namespace UnitTests {
    public class TransferMoney_Tests {
        [Theory]
        [InlineData(100.50, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 500, 100, 0, "dartvader@gmail.com", "e476ed86-5b59-415f-b080-c556c0807756", 1000, 2000, "r2d2@gmail.com")]
        [InlineData(400, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 500, 0, 0, "dartvader@gmail.com", "e476ed86-5b59-415f-b080-c556c0807756", 1000, 2000, "r2d2@gmail.com")]
        public void TransferMoney_Account_Balance_Update_Tests(decimal amount, string fromAccountIdS, int fromBalance, int fromWithdrawn, int fromPaidIn, string fromEmail, string toAccountIdS, int toBalance, int toPaidIn, string toEmail) {
            var fromAccountId = Guid.Parse(fromAccountIdS);
            var toAccountId = Guid.Parse(toAccountIdS);


            Account fromAccount = new Account() {
                Balance = fromBalance,
                Id = fromAccountId,
                PaidIn = fromPaidIn,
                Withdrawn = fromWithdrawn,
                User = new User() {
                    Email = fromEmail
                }
            };
            Account toAccount = new Account() {
                Balance = toBalance,
                Id = toAccountId,
                PaidIn = toPaidIn,
                User = new User() {
                    Email = toEmail
                }
            };

            var accountMock = GetMockIAccountRepository(fromAccount, toAccount);
            var notificationMock = GetMockINotificationService(fromEmail, toEmail);
            TransferMoney transferMoney = new TransferMoney(accountMock.Object, notificationMock.Object);
            transferMoney.Execute(fromAccountId, toAccountId, amount);
            accountMock.Verify(n => n.GetAccountById(fromAccountId), Times.Once);
            accountMock.Verify(n => n.GetAccountById(toAccountId), Times.Once);
            accountMock.Verify(n => n.Update(fromAccount), Times.Once);
            accountMock.Verify(n => n.Update(toAccount), Times.Once);

            Assert.Equal(fromAccount.Balance, fromBalance - amount);
            Assert.Equal(fromAccount.Withdrawn, fromWithdrawn - amount);

            Assert.Equal(toAccount.Balance, toBalance + amount);
            Assert.Equal(toAccount.PaidIn, toPaidIn + amount);
        }


        [Theory]
        [InlineData(600, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 500, 100, 0, "dartvader@gmail.com", "e476ed86-5b59-415f-b080-c556c0807756", 1000, 2000, "r2d2@gmail.com")]
        [InlineData(7000, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 50000, 0, 0, "dartvader@gmail.com", "e476ed86-5b59-415f-b080-c556c0807756", 1000, 2000, "r2d2@gmail.com")]
        public void TransferMoney_Account_Balance_Unsuccessfull_Tests(decimal amount, string fromAccountIdS, int fromBalance, int fromWithdrawn, int fromPaidIn, string fromEmail, string toAccountIdS, int toBalance, int toPaidIn, string toEmail) {
            var fromAccountId = Guid.Parse(fromAccountIdS);
            var toAccountId = Guid.Parse(toAccountIdS);

            Account fromAccount = new Account() {
                Balance = fromBalance,
                Id = fromAccountId,
                PaidIn = fromPaidIn,
                Withdrawn = fromWithdrawn,
                User = new User() {
                    Email = fromEmail
                }
            };
            Account toAccount = new Account() {
                Balance = toBalance,
                Id = toAccountId,
                PaidIn = toPaidIn,
                User = new User() {
                    Email = toEmail
                }
            };

            var accountMock = GetMockIAccountRepository(fromAccount, toAccount);
            var notificationMock = GetMockINotificationService(fromEmail, toEmail);
            TransferMoney transferMoney = new TransferMoney(accountMock.Object, notificationMock.Object);
            Assert.Throws<InvalidOperationException>(() => transferMoney.Execute(fromAccountId, toAccountId, amount));
            accountMock.Verify(n => n.GetAccountById(fromAccountId), Times.Once);
            accountMock.Verify(n => n.GetAccountById(toAccountId), Times.Once);
            accountMock.Verify(n => n.Update(fromAccount), Times.Never);
            accountMock.Verify(n => n.Update(toAccount), Times.Never);
        }


        [Theory]
        [InlineData(100.50, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 500, 0, "dartvader@gmail.com", "e476ed86-5b59-415f-b080-c556c0807756", 1000, 3500, "r2d2@gmail.com")]
        [InlineData(400, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 500, 0, "dartvader@gmail.com", "e476ed86-5b59-415f-b080-c556c0807756", 1000, 3500, "r2d2@gmail.com")]
        public void TransferMoney_Notification_Called_Tests(decimal amount, string fromAccountIdS, int fromBalance, int fromPaidIn, string fromEmail, string toAccountIdS, int toBalance, int toPaidIn, string toEmail) {
            var fromAccountId = Guid.Parse(fromAccountIdS);
            var toAccountId = Guid.Parse(toAccountIdS);

            Account fromAccount = new Account() {
                Balance = fromBalance,
                Id = fromAccountId,
                PaidIn = fromPaidIn,
                User = new User() {
                    Email = fromEmail
                }
            };
            Account toAccount = new Account() {
                Balance = toBalance,
                Id = toAccountId,
                PaidIn = toPaidIn,
                User = new User() {
                    Email = toEmail
                }
            };
            var accountMock = GetMockIAccountRepository(fromAccount, toAccount);
            var notificationMock = GetMockINotificationService(fromEmail, toEmail);
            TransferMoney transferMoney = new TransferMoney(accountMock.Object, notificationMock.Object);
            transferMoney.Execute(fromAccountId, toAccountId, amount);
            notificationMock.Verify(n => n.NotifyFundsLow(fromEmail), Times.Once);
            notificationMock.Verify(n => n.NotifyApproachingPayInLimit(toEmail), Times.Once);
        }

        [Theory]
        [InlineData(100.50, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 2000, 0, "dartvader@gmail.com", "e476ed86-5b59-415f-b080-c556c0807756", 1000, 400, "r2d2@gmail.com")]
        [InlineData(300, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 5000, 0, "dartvader@gmail.com", "e476ed86-5b59-415f-b080-c556c0807756", 1000, 1000, "r2d2@gmail.com")]
        public void TransferMoney_Notification_NonCalled_Tests(decimal amount, string fromAccountIdS, int fromBalance, int fromPaidIn, string fromEmail, string toAccountIdS, int toBalance, int toPaidIn, string toEmail) {
            var fromAccountId = Guid.Parse(fromAccountIdS);
            var toAccountId = Guid.Parse(toAccountIdS);

            Account fromAccount = new Account() {
                Balance = fromBalance,
                Id = fromAccountId,
                PaidIn = fromPaidIn,
                User = new User() {
                    Email = fromEmail
                }
            };
            Account toAccount = new Account() {
                Balance = toBalance,
                Id = toAccountId,
                PaidIn = toPaidIn,
                User = new User() {
                    Email = toEmail
                }
            };
            var accountMock = GetMockIAccountRepository(fromAccount, toAccount);
            var notificationMock = GetMockINotificationService(fromEmail, toEmail);
            TransferMoney transferMoney = new TransferMoney(accountMock.Object, notificationMock.Object);
            transferMoney.Execute(fromAccountId, toAccountId, amount);
            notificationMock.Verify(n => n.NotifyFundsLow(fromEmail), Times.Never);
            notificationMock.Verify(n => n.NotifyApproachingPayInLimit(toEmail), Times.Never);
        }

        Mock<IAccountRepository> GetMockIAccountRepository(Account fromAccount, Account toAccount) {
            var accountMock = new Mock<IAccountRepository>();
            accountMock.Setup(m => m.GetAccountById(fromAccount.Id)).Returns(fromAccount);
            accountMock.Setup(m => m.GetAccountById(toAccount.Id)).Returns(toAccount);
            accountMock.Setup(m => m.Update(fromAccount)).Verifiable();
            accountMock.Setup(m => m.Update(toAccount)).Verifiable();
            return accountMock;
        }

        Mock<INotificationService> GetMockINotificationService(string fromEmail, string toEmail) {
            var notficationMock = new Mock<INotificationService>();
            notficationMock.Setup(m => m.NotifyApproachingPayInLimit(toEmail)).Verifiable();
            notficationMock.Setup(m => m.NotifyFundsLow(fromEmail)).Verifiable();
            return notficationMock;
        }
    }
}
