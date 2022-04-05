namespace Baltic.Types.QueueAccess
{
	public enum QueueRemoveMethod : int
	{
		OnEachAcknowledge = 0, // after a message Ack, the queue will start pretending to be removed, besides this - as for PredecessorBased
		PredecessorBased = 1, // if all the predecessor queues are removed - the queue is to be removed automatically (on the last message Ack)
		OnDemand = 2 // queue must be removed manually
	}
}