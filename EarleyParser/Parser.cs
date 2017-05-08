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
using System.IO;
using System.Linq;

namespace Earley
{
	public class Parser
	{
		protected readonly Production startProduction;

		public virtual Production StartProduction => this.startProduction;
		public Parser(Production startProduction)
		{
			this.startProduction
				= ((startProduction ?? throw new ArgumentNullException()).Symbols.LastOrDefault() == Terminal.EOF)
				? startProduction : throw new ArgumentException(nameof(startProduction));
		}

		public virtual List<object> Parse(string input)
			=> Parse(new StringReader(input ?? throw new ArgumentNullException(nameof(input))));

		public virtual List<object> Parse(TextReader reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			State initial = new State();

			initial.Add(new Item(startProduction, initial));

			State current = initial;

			State next = new State();

			do
			{
				var completedNullable =
					new Dictionary<Production, List<Item>>();

				for (int i = 0; i < current.ItemsCount; i++)
				{
					Item item = current[i];

					if (!item.AtEnd && item.Symbol is Nonterminal)
					{
						this.Predictor(current, item, completedNullable);
					}
					else if (!item.AtEnd && item.Symbol is Terminal)
					{
						this.Scanner(item, next, reader.Peek());
					}
					else
					{
						this.Completer(current, item, completedNullable);
					}
				}

				current = next;
				next = new State();
			}
			while (reader.Read() != -1 && current.ItemsCount > 0);

			if (current.ItemsCount == 1 &&
				current[0].AtEnd &&
				current[0].Production == startProduction &&
				current[0].State == initial)
			{
				return current[0].Reduce();
			}
			else
			{
				return new List<object>();
			}
		}

		protected virtual void Predictor(
			State state,
			Item item,
			IDictionary<Production, List<Item>> completedNullable)
		{
			if (item.AtEnd || !(item.Symbol is Nonterminal nt))
				throw new ArgumentException(nameof(item));

			foreach (Production p in nt.Productions)
			{
				Item newItem = new Item(p, state);

				if (!state.Contains(newItem))
				{
					state.Add(newItem);
					ShiftCompletedNullable(state, newItem, completedNullable);
				}
			}
		}

		protected virtual void Scanner(Item item, State next, int ch)
		{
			if (item.AtEnd || !(item.Symbol is Terminal t))
				throw new ArgumentException(nameof(item));

			if (t.Contains(ch))
			{
				Item newItem = item.NextItem;
				newItem.Add(ch);
				next.Add(newItem);
			}
		}

		protected virtual void Completer(
			State state,
			Item item,
			IDictionary<Production, List<Item>> completedNullable)
		{
			if (!item.AtEnd) throw new ArgumentException(nameof(item));

			if (item.State == state)
			{
				// completed a nullable item

				if (!completedNullable.TryGetValue(item.Production, out List<Item> items))
				{
					completedNullable[item.Production] = items = new List<Item>();
				}
				items.Add(item);
			}

			foreach (Item parentItem in item.State.GetItemsFor(item.Production))
			{
				Item newItem = state.Import(parentItem.NextItem);
				newItem.Add(item);
				this.ShiftCompletedNullable(state, newItem, completedNullable);
			}
		}

		// When an item is added to the current state, any nullable
		// productions that have already been completed need to be
		// added to the new item
		protected virtual void ShiftCompletedNullable(
			State state,
			Item item,
			IDictionary<Production, List<Item>> completedNullable)
		{
			if (!item.AtEnd && item.Symbol is Nonterminal nt)
			{
				Item nextItem = null;

				foreach (Production p in completedNullable.Keys)
				{
					if (nt.Contains(p))
					{
						if (nextItem == null)
						{
							nextItem = state.Import(item.NextItem);
						}

						foreach (Item nullableItem in completedNullable[p])
						{
							nextItem.Add(nullableItem);
						}
					}
				}

				if (nextItem != null)
				{
					this.ShiftCompletedNullable(state, nextItem, completedNullable);
				}
			}
		}
	}
}