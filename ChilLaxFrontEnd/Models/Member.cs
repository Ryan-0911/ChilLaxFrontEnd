using System;
using System.Collections.Generic;

namespace ChilLaxFrontEnd.Models
{
    public partial class Member
    {
        public Member()
        {
            Carts = new HashSet<Cart>();
            CustomerServices = new HashSet<CustomerService>();
            PointHistories = new HashSet<PointHistory>();
            ProductOrders = new HashSet<ProductOrder>();
            TarotOrders = new HashSet<TarotOrder>();
        }

        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public string MemberTel { get; set; } = null!;
        public string? MemberAddress { get; set; }
        public string MemberEmail { get; set; } = null!;
        public DateTime MemberBirthday { get; set; }
        public bool? MemberSex { get; set; }
        public int? MemberPoint { get; set; }
        public DateTime MemberJoinTime { get; set; }
        public bool Available { get; set; }
        public string? Provider { get; set; }
        public string? ProviderUserId { get; set; }
        public bool? IsValid { get; set; }

        public virtual MemberCredential? MemberCredential { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<CustomerService> CustomerServices { get; set; }
        public virtual ICollection<PointHistory> PointHistories { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
        public virtual ICollection<TarotOrder> TarotOrders { get; set; }
    }
}
