using System;
using System.Collections.Generic;

namespace Earley
{
	class Program
	{
		/// <summary>
		/// Digit	::= [0-9]
		/// Literal	::= Digit
		///			  | Literal Digit
		/// Exp		::= Literal
		///			  | (Exp)
		///			  | Exp + Exp
		///			  | Exp - Exp
		///			  | Exp * Exp
		///			  | Exp / Exp
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			var digit = "0123456789".AsTerminal();

			var literal = "Literal".AsNonterminal();

			literal.Add(
				digit,
				literal + digit);

			var exp = "Exp".AsNonterminal();

			exp.Add(
				literal,
				"(" + exp + ")",
				exp + "+" + exp,
				exp + "-" + exp,
				exp + "*" + exp,
				exp + "/" + exp
				);

			var start = exp + Terminal.EOF;

			Parser parser = new Parser(start);

			Console.WriteLine("Type an arithmetic expression. Type a blank line to quit.");

			string line = null;

			while((line = Console.ReadLine()) != null)
			{
				List<dynamic> results = parser.Parse(line);

				if(results.Count == 0)
				{
					Console.WriteLine("Parse Error");
				}
				else
				{
					for(int i = 0; i < results.Count; i++)
					{
						Console.WriteLine($"Result({i + 1}) = ", results[i]);
					}
				}
			}
		}
	}
}
