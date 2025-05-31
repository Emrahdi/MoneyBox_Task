
# Hi

I try to summarize what I think and done with this task below.


    * The task took more than 1 hour to complete, because I wanted to add as much as unit tests to cover the following cases.
            
            * TransferMoney 
                * Exception cases
                * Successfull test
                * Balance and PaidIn check
                * Nofication Cases
                * Check if GetAccountById & Update are called correctly
            * WithdrawMoney
                * Successfull test
		        * Exception cases
                * Successfull test
                * Balance and Withdrawn check
                * Nofication Case
                * Check if GetAccountById & Update are called correctly
            * Account
                * DepositMoney
                    * Successfull case with Balance and PaidInCheck
                    * Exception case
                * WithdrawMoney
                    * Successfull case with Balance and Withdrawn
                    * Exception case
		
    * According to the explanation, the domain modals are asked to be richer. So I decided to move the MoneyTransfer method to the Account object. First I created a method in Account class called TransferMoney, but then I decided to divide it to WithdrawMoney & DepositMoney. I think dividing the method into to 2 different methods has more benefits. As I worked in banking industry many years, I know every money transcation that makes accounting, has its own general ledger accounts ( Ex. atm cash money cash draw general ledger account, swift general ledger account etc.) so diving into 2 made more sense for me for future operations.
    * The negative side of diving into 2 methods is, when you execute  
        from.WithdrawMoney(amount);
        to.DepositMoney(amount);
    consecutively in TransferMoney Execute function, whenever DepositMoney gets error, the from Balance and Withdrawn values would be changed. But, I didn't make any more changes to rollback these values, or add fromAccount object to DepositMoney function to make the validations in WithdrawMoney because the values don't impact the rest of the execute function and the update method for now. But the validations will be done also in DepositMoney function.

    * The next step might be creating an interface and inherit the Account class from this interface in case of better testing abilities. 

   Thank you for reading.

   Best Regards,
   ED.
