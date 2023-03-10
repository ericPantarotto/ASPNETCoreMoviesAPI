using System;

namespace MoviesAPI.Testing
{
    public class TransferServiceValidation
    {
        private readonly IValidateWireTransfer validateWireTransfer;

        public TransferServiceValidation(IValidateWireTransfer validateWireTransfer)
        {
            this.validateWireTransfer = validateWireTransfer;
        }

        public void WireTransfer(Account origin, Account destination, decimal amount)
        {
            var state = validateWireTransfer.Validate(origin, destination, amount);

            if (!state.IsSuccessful)
            {
                throw new ApplicationException(state.ErrorMessage);
            }

            origin.Funds -= amount;
            destination.Funds += amount;
        }
    }
}
