using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DonatingFundsClient.Models
{
    public class Donar
    {
        public int DonorId { get; set; }
        public string DonorName { get; set; }

        public DateTime DateOfDonation { get; set; }
        public double Amount { get; set; }

        public int organization_Id { get; set; }
    }
}
