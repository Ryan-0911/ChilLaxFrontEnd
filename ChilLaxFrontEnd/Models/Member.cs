using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string MemberName { get; set; } = null!;

    public string MemberTel { get; set; } = null!;

    public string? MemberAddress { get; set; }

    public string MemberEmail { get; set; } = null!;

    public DateTime MemberBirthday { get; set; }

    public bool? MemberSex { get; set; }

    public int? MemberPoint { get; set; }

    public DateTime MemberJoinTime { get; set; }

    public bool? Available { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<CustomerService> CustomerServices { get; set; } = new List<CustomerService>();

    public virtual MemberCredential? MemberCredential { get; set; }

    public virtual ICollection<PointHistory> PointHistories { get; set; } = new List<PointHistory>();

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    public virtual ICollection<TarotOrder> TarotOrders { get; set; } = new List<TarotOrder>();
}
