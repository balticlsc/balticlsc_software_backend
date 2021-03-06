///////////////////////////////////////////////////////////
//  Message.cs
//  Implementation of the Class Message
//  Generated by Enterprise Architect
//  Created on:      13-mar-2020 15:15:03
//  Original author: smialek
///////////////////////////////////////////////////////////


using System;

namespace Baltic.DataModel.CALExecutable {
	public abstract class Message {

		public string TaskUid;
		public string MsgUid;
		public string QueueUid;

		public Message(){
			MsgUid = Guid.NewGuid().ToString();
		}

		~Message(){

		}

	}//end Message

}//end namespace CAL_Executable