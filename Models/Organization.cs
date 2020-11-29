using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DonatingFundsClient.Models
{
    public class Organization
    {
        public int Id { get; set; }

        public string OrganizationName { get; set; }

        public string TotalDonations { get; set; }
    }
}
