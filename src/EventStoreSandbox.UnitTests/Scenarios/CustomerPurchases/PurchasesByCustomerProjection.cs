namespace EventStore.SandBox.UnitTests
{
	public class PurchasesByCustomerProjection : Projection
	{
		public PurchasesByCustomerProjection ()
			: base("purchasesByCustomer", ProjectionType.Continuous)
		{
//			this.FromStream ("purchases")
//				.When<PurchaseAccepted> (
//				s => s.LinkTo (e => e.CustomerId)
//						.IgnoreIfEmpty ());
		}
	}
}

