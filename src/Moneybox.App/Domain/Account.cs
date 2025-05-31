using System;

namespace Moneybox.App {
    public class Account {

        //todo: get from configuration?
        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public void DepositMoney(decimal amount) {
            var paidIn = this.PaidIn + amount;
            if (paidIn > Account.PayInLimit) {
                throw new InvalidOperationException("Account pay in limit reached");
            }
            this.Balance = this.Balance + amount;
            this.PaidIn = this.PaidIn + amount;
        }

        public void WithdrawMoney(decimal amount) {
            var fromBalance = this.Balance - amount;
            if (fromBalance < 0m) {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }
            this.Balance = this.Balance - amount;
            this.Withdrawn = this.Withdrawn - amount;
        }
    }
}
