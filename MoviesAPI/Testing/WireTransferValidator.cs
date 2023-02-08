namespace MoviesAPI.Testing
{
    public class WireTransferValidator : IValidateWireTransfer
    {
        public OperationResult Validate(Account origin, Account destination, decimal amount)
        {
            if (amount > origin.Funds){ return new OperationResult(false, "The origin account doesn't have enough funds."); }

            // other validations

            return new OperationResult(true);
        }
    }
}
