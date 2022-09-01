using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	class Program
	{
		static bool _run = true;
		static Profiles.DigitalIO _output;
		private static Profiles.Turck.Fen20 _turck;

		static void Main(string[] args)
		{
			// Ryan - Sept 1, 2022
			// The original testing was performed on a Turck FEN20.
			// But currently I only have access to a TBEN-S1-8DXP,
			// which didn't require any modifications to the Profile
			var connection = Init();

			Loop();

			Console.ReadLine();

			Cleanup(connection);

			Console.ReadLine();
		}

		static Wrapper.ImplicitConnection Init()
		{
			// Start the listener on our host IP
			var connection = new Wrapper.ImplicitConnection();
			connection.StartListener("192.168.1.8");

			// Add a Turck device
			var profile = new Profiles.Turck.Fen20.Profile() { IpAddress = "192.168.1.10" };
			_turck = new Profiles.Turck.Fen20(10, profile, connection);

			// Add some IO points
			var register = Profiles.Turck.Fen20.Profile.DEFAULT_REGISTER;
			_turck.Inputs.IO.Add(new Profiles.DigitalIO((register, 0)));
			_turck.Inputs.IO.Add(new Profiles.DigitalIO((register, 1)));
			_output = new Profiles.DigitalIO((register, 2));
			_turck.Outputs.IO.Add(_output);

			// Start the connection
			_turck.StartConnection();
			return connection;
		}

		private static void Cleanup(Wrapper.ImplicitConnection connection)
		{
			// Give the loop time to finish up
			_run = false;
			System.Threading.Thread.Sleep(1000);

			_turck.StopConnection();
			connection.CloseListener();
		}

		static async void Loop()
		{
			await Task.Delay(1000);
			ToggleOutput();
			PrintState();
			if (_run) Loop();
		}

		static void PrintState()
		{
			Console.WriteLine(_turck.Inputs.ByteToString);
		}

		static void ToggleOutput()
		{
			_output.Value = !_output.Value;
		}
	}
}
