using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TuringNatural3
{
    public enum State
    {
        Q0, Q1, Q2, Q0m, Q1m, Q2m,
        Q3
    }

    public enum Direction
    {
        L, R, None
    }

    public class StateDef
    {
        public State State { get; set; }
        public Func<char?, (State, char?, Direction)> TransitionFunction { get; set; }
    }

    public class TuringMachine
    {
        public List<StateDef> StateDefinitions = new List<StateDef>
        {
            new StateDef { State = State.Q0, TransitionFunction = (symbol) =>
            {
                if(symbol == '-')
                {
                    return (State.Q0m, null, Direction.R);
                }
                else if(symbol != null)
                {
                    return (State.Q0, null, Direction.R );
                }

                else
                {
                    return(State.Q1, null, Direction.L);
                }
            } },
               new StateDef { State = State.Q0m, TransitionFunction = (symbol) =>
            {
                if(symbol != null)
                {
                    return (State.Q0m, null, Direction.R );
                }
                else
                {
                    return(State.Q1m, null, Direction.L);
                }
            } },

            new StateDef { State = State.Q1m, TransitionFunction =  (symbol) => {
                  var parsedSymbol =  parseChareToNullOrInt(symbol);

                if (parsedSymbol >= 3 && parsedSymbol <= 9)
                    return (State.Q3, (parsedSymbol-3).ToString()[0], Direction.None);
                if(parsedSymbol >= 0 && parsedSymbol <= 2 )
                    return (State.Q2m, int.Parse((parsedSymbol + 10 - 3).ToString()[0].ToString()).ToString()[0], Direction.L);
                if(symbol == null)
                {
                    return (State.Q3, 3.ToString()[0], Direction.None);
                }
                throw new InvalidCastException();
            }

        },

            new StateDef { State = State.Q1, TransitionFunction =  (symbol) => {
                  var parsedSymbol =  parseChareToNullOrInt(symbol);

                if (parsedSymbol >= 0 && parsedSymbol <= 6)
                    return (State.Q3, (parsedSymbol+3).ToString()[0], Direction.None);
                if(parsedSymbol >= 7 && parsedSymbol <= 9 )
                    return (State.Q2, int.Parse((parsedSymbol + 3).ToString()[1].ToString()).ToString()[0], Direction.L);
                if(symbol == null)
                {
                    return (State.Q3, 3.ToString()[0], Direction.None);
                }
                throw new InvalidCastException();
            }

        },
            new StateDef { State = State.Q2, TransitionFunction =  (symbol) => {
                  var parsedSymbol =  parseChareToNullOrInt(symbol);


                if(parsedSymbol >= 1 && parsedSymbol < 9)
                    return (State.Q3, (parsedSymbol+1).ToString()[0], Direction.None);
                if(parsedSymbol == 9 )
                    return (State.Q2, 0.ToString()[0], Direction.L);
                if(parsedSymbol == null)
                {
                    return (State.Q3, 1.ToString()[0], Direction.None);
                }
                throw new InvalidCastException();
            }

        },
              new StateDef { State = State.Q2m, TransitionFunction =  (symbol) => {
                  var parsedSymbol =  parseChareToNullOrInt(symbol);


                if(parsedSymbol >= 1 && parsedSymbol <= 9)
                    return (State.Q3, (parsedSymbol-1).ToString()[0], Direction.None);
                if(parsedSymbol == 0 )
                    return (State.Q2m, 9.ToString()[0], Direction.L);
                if(parsedSymbol == null)
                {
                    return (State.Q3, 1.ToString()[0], Direction.None);
                }
                throw new InvalidCastException();
            }

        },
             new StateDef { State = State.Q3, TransitionFunction = (symbol) =>
            {
                return (State.Q3, null, Direction.None);
            } },
        };

        private static int? parseChareToNullOrInt(char? symbol)
        {
            int? symParsed;
            if (symbol == null)
            {
                symParsed = null;
            }
            else
            {
                symParsed = int.Parse(symbol.ToString());
            }
            return symParsed;
        }

        private ObservableCollection<ValueCurrentPair> tape;

        public int headPosition { get; set; } = 0;

        private void setCurrent()
        {
            for (int i = 0; i < this.tape.Count; i++)
            {
                var val = this.tape[i].Value;
                this.tape.RemoveAt(i);
                this.tape.Insert(i, new ValueCurrentPair { Current = false, Value = val });
            }

            var value = GetTapeCell().Value;
            this.tape.RemoveAt(headPosition);
            this.tape.Insert(headPosition, new ValueCurrentPair { Current = true, Value = value });

        }

        private ValueCurrentPair GetTapeCell()
        {
            if (headPosition >= this.tape.Count)
            {
                this.tape.Insert(headPosition, new ValueCurrentPair { Current = true, Value = null });
            }
            if (headPosition < 0)
            {
                this.tape.Insert(0, new ValueCurrentPair { Current = true, Value = null });
                headPosition = 0;
            }
            return this.tape[headPosition];
        }

        public TuringMachine(ObservableCollection<ValueCurrentPair> tape)
        {
            this.tape = tape;
            setCurrent();
        }

        public State CurrentState { get; set; } = State.Q0;

        public void Step()
        {
            var tuple = this.StateDefinitions.First(x => x.State == this.CurrentState).TransitionFunction(GetTapeCell().Value);
            this.CurrentState = tuple.Item1;
            if (tuple.Item2 != null)
            {
                this.tape.RemoveAt(this.headPosition);
                this.tape.Insert(this.headPosition, new ValueCurrentPair() { Current = true, Value = tuple.Item2.Value });
            }
            switch (tuple.Item3)
            {
                case Direction.L:
                    this.headPosition--;
                    break;
                case Direction.R:
                    this.headPosition++;
                    break;
                case Direction.None:
                    break;
                default:
                    break;
            }
            setCurrent();

        }
    }
}
