using System;
using System.Collections.Generic;

namespace Wallet.Domain.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Phone { get; set; } = null!;

    public string? Username { get; set; }

    public string Hashpassword { get; set; } = null!;

    public int Walletid { get; set; }

    public virtual ICollection<Transaction> TransactionReceivers { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionSenders { get; set; } = new List<Transaction>();

    public virtual Wallets? Wallet { get; set; }
}
