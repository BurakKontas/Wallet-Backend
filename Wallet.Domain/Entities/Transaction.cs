﻿using System;
using System.Collections.Generic;

namespace Wallet.Domain.Entities;

public partial class Transaction
{
    public int Id { get; set; }

    public int? Senderid { get; set; }

    public int? Receiverid { get; set; }

    public decimal Amount { get; set; }

    public virtual User? Receiver { get; set; }

    public virtual User? Sender { get; set; }
}
