using System;
using System.Collections.Generic;

namespace Wallet.Domain.Entities;

public partial class Wallets
{
    public int Id { get; set; }

    public decimal Balance { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
