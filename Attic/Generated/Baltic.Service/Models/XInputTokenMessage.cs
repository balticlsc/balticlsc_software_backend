///////////////////////////////////////////////////////////
//  XInputTokenMessage.cs
//  Implementation of the Class XInputTokenMessage
//  Generated by Enterprise Architect
//  Created on:      23-kwi-2020 13:08:32
//  Original author: smialek
///////////////////////////////////////////////////////////



using System.Collections.Generic;

namespace Baltic.Service.Models {
	public class XInputTokenMessage {

		public XInputTokenMessage(){

		}

		~XInputTokenMessage(){

		}

		public string MsgUid{
			get;
			set;
		}

		public string PinName{
			get;
			set;
		}

		public string JSONPars{
			get;
			set;
		}

		public IEnumerable<XSeqToken> SeqStack{
			get;
			set;
		}

	}//end XInputTokenMessage

}//end namespace Baltic.Service.Models