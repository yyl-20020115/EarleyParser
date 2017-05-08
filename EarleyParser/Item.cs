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

namespace Earley
{
	public class Item
	{
		protected readonly Production production;
		protected readonly int index;
		protected readonly State state;
		protected readonly List<dynamic> derivations;
		protected readonly Item prevItem;

		public virtual bool AtStart => index == 0;

		public virtual bool AtEnd => index == production.Symbols.Count;

		public virtual Item NextItem => AtEnd
			? throw new InvalidOperationException()
			: new Item(production, index + 1, state, this);

		public virtual State State => state;

		public virtual Production Production => production;

		public virtual Symbol Symbol => AtEnd
			? throw new InvalidOperationException()
			: production.Symbols[index];

		public Item(Production production, State state)
			: this(production, 0, state, null) { }

		public Item(Production production, int index, State state, Item prevItem)
		{
			this.production = production ?? throw new ArgumentNullException(nameof(production));
			this.state = state ?? throw new ArgumentNullException(nameof(state));
			this.index = index >= 0 && index < this.production.Symbols.Count ? index : throw new ArgumentOutOfRangeException(nameof(index));

			if (index == 0 && prevItem != null)
			{
				throw new ArgumentException(nameof(index));
			}

			if (index > 0 && prevItem == null)
			{
				throw new ArgumentNullException(nameof(prevItem));
			}
			this.prevItem = prevItem;

			this.derivations = index == 0 ? null : new List<dynamic>();
		}

		public override bool Equals(object obj)
			=> obj is Item other &&
				other != null &&
				other.production == production &&
				other.index == index &&
				other.state == state &&
				other.prevItem == prevItem;

		public override int GetHashCode()
			=> unchecked((((state.GetHashCode() * 31)
				+ production.GetHashCode()) * 31)
				+ (prevItem == null ? 0 : prevItem.GetHashCode()) * 32
				+ index);

		public virtual void Add(int ch)
		{
			if (AtStart || !(prevItem.Symbol is Terminal))
			{
				throw new InvalidOperationException();
			}

			if (!(prevItem.Symbol as Terminal).Contains(ch))
			{
				throw new ArgumentException(nameof(ch));
			}

			this.derivations.Add(ch);
		}

		public virtual void Add(Item it)
		{
			if (AtStart || !(prevItem.Symbol is Nonterminal))
			{
				throw new InvalidOperationException();
			}

			if (!(prevItem.Symbol as Nonterminal).Contains(it.production))
			{
				throw new ArgumentException(nameof(it));
			}

			this.derivations.Add(it);
		}

		public virtual List<dynamic> Reduce()
		{
			if (!AtEnd)
			{
				throw new InvalidOperationException();
			}

			List<dynamic> result = new List<dynamic>();

			foreach (dynamic[] args in ReduceWorker())
			{
				result.Add(this.production.Apply(args));
			}

			return result;
		}

		// reduces all the derivations for *this* symbol
		protected virtual List<dynamic> ReduceSymbol()
		{
			if (AtStart) throw new InvalidOperationException();

			List<dynamic> result = new List<dynamic>();

			if (prevItem.Symbol is Terminal)
			{
				result.AddRange(derivations);
			}
			else if (prevItem.Symbol is Nonterminal)
			{
				foreach (Item item in derivations)
				{
					result.AddRange(item.Reduce());
				}
			}

			return result;
		}

		protected virtual List<dynamic[]> ReduceWorker()
		{
			List<dynamic[]> result = new List<dynamic[]>();

			if (production.Symbols.Count == 0)
			{
				result.Add(new dynamic[0]);
			}
			else if (prevItem.AtStart)
			{
				foreach (dynamic value in ReduceSymbol())
				{
					dynamic[] args = new dynamic[production.Symbols.Count];
					args[0] = value;
					result.Add(args);
				}
			}
			else
			{
				List<dynamic> symbolReductions = ReduceSymbol();

				foreach (dynamic[] prefix in prevItem.ReduceWorker())
				{
					foreach (dynamic value in symbolReductions)
					{
						dynamic[] args = prefix.Clone() as dynamic[];
						args[prevItem.index] = value;
						result.Add(args);
					}
				}
			}

			return result;
		}

		// whether the other item is empty and just being used as a key,
		// or whether it has a different previous item, already has
		// derivations, etc. See State::Import.
		public virtual bool IsImportCompatible(Item other)
			=> Equals(other ?? throw new ArgumentNullException(nameof(other)))
				&& ((other.derivations == null && derivations == null)
				|| other.derivations.Count == 0);
	}
}
