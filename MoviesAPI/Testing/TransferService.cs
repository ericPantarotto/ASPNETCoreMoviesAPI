using System;

namespace MoviesAPI.Testing
{
    public class TransferService
    {
        public void WireTransfer(Account origin, Account destination, decimal amount)
        {
            if (amount > origin.Funds) { throw new ApplicationException("The origin account doesn't have enough funds."); }

            origin.Funds -= amount;
            destination.Funds += amount;
        }
    }
}
