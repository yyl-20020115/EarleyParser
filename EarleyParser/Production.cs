// Copyright 2004 Dominic Cooney. All Rights Reserved.

/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earley
{
	public class Production
	{
		public static Production operator +(Production production, string text)
		{
			return production?.Add(text.AsTerminal());
		}

		public static Nonterminal operator |(Production left, Production right)
		{
			return new Nonterminal(left, right);
		}

		public static Production Of(params Symbol[] symbols)
		{
			return new Production(symbols);
		}

		protected readonly List<Symbol> symbols = null;

		public Production(params Symbol[] symbols)
			=> this.symbols = new List<Symbol>(
				(symbols == null || symbols.Any(s => s == null))
				? throw new ArgumentNullException(nameof(symbols))
				: symbols
				);

		public virtual List<Symbol> Symbols => this.symbols;

		public virtual Production Add(params Symbol[] symbols)
		{
			this.symbols.AddRange(
				symbols == null || symbols.Any(s => s == null)
				? throw new ArgumentNullException(nameof(symbols))
				: symbols);

			return this;
		}

		public virtual dynamic Apply(dynamic[] args) => args;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < symbols.Count; i++)
			{
				Symbol symbol = symbols[i];

				if (symbol is Terminal t)
				{
					builder.Append(t.ToString());
				}
				else if (symbol is Nonterminal n)
				{
					builder.Append(n.Name ?? string.Empty);
				}
				if (i < symbols.Count - 1)
				{
					builder.Append(' ');
				}
			}
			return builder.ToString();
		}
	}
}
