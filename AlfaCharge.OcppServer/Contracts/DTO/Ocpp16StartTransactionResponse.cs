
using AlfaCharge.Domain.Models;

namespace AlfaCharge.OcppServer.Contracts.DTO
{
    public class Ocpp16StartTransactionResponse
    {
        public IdTagInfo IdTagInfo { get; set; } = new() { Status = IdTagStatus.Accepted };
        public int TransactionId { get; set; }
    }
}