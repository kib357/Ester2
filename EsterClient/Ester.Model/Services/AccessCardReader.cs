using System;
using System.IO.Ports;
using Ester.Model.Interfaces;

namespace Ester.Model.Services
{
	public delegate void OnReceived(int sc, int cn);

	public class AccessCardReader : IAccessCardReader
	{
		private SerialPort _sPort = null;

		public String Port
		{
			get { return _sPort.PortName; }
			set { _sPort.Close(); _sPort.PortName = value; _sPort.Open(); }
		}

		public OnReceived Received { get; set; }

		public bool IsOpened { get { return _sPort.IsOpen; } }

		public AccessCardReader(string comport,
						  int baudrate = 9600,
						  Parity parity = Parity.None,
						  StopBits stopbits = StopBits.One,
						  int databits = 8,
						  Handshake handshake = Handshake.None)
		{
			_sPort = new SerialPort(comport);

			_sPort.BaudRate = baudrate;
			_sPort.Parity = parity;
			_sPort.StopBits = stopbits;
			_sPort.DataBits = databits;
			_sPort.Handshake = handshake;

			_sPort.DataReceived += new SerialDataReceivedEventHandler(DataReceviedHandler);

		}


		private void DataReceviedHandler(object sender, SerialDataReceivedEventArgs e)
		{
			SerialPort sp = (SerialPort)sender;
			string indata = sp.ReadLine();

			if (indata.Contains("Em-Marine"))
			{
				string[] words = indata.Split(new[] { ',', ' ' });
				if (Received != null)
					Received(int.Parse(words[1]), int.Parse(words[2]));
			}
		}

		public void Open()
		{
			if (!_sPort.IsOpen) _sPort.Open();
		}

		public void Close()
		{
			if (!_sPort.IsOpen) _sPort.Close();
		}
	}
}
