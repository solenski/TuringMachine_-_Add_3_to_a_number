using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TuringNatural3
{
    public struct ValueCurrentPair
    {
        public char? Value { get; set; }
        public bool Current { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<IEnumerable<ValueCurrentPair>> readStrings =
            File.ReadAllText("strings.txt")
            .Split('#')
            .Select(x => x.Select(y => new ValueCurrentPair { Current = false, Value = y }));

        public ObservableCollection<ValueCurrentPair> Tape;
        private TuringMachine turingMachine;

        private int stringsPointer = 0;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            initialize();
        }

        private void initialize()
        {
            this.Tape = new ObservableCollection<ValueCurrentPair>(this.readStrings.ToList()[stringsPointer]);
            this.VisualTape.ItemsSource = this.Tape;

            this.turingMachine = new TuringMachine(this.Tape);
            updateStateIndicator();
        }

        private void updateStateIndicator()
        {
            this.STATE_INDICATOR.Text = "STATE: " + this.turingMachine.CurrentState.ToString();
            this.CURRENT_STRING.Text = "STRING: " +  new string(this.readStrings.ToList()[stringsPointer].Where(x => x.Value != null).Select(x => x.Value.Value).ToArray());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.turingMachine.CurrentState == State.Q3)
            {
                this.stringsPointer++;
                if (this.readStrings.ToList().ElementAt(this.stringsPointer) != null)
                {
                    initialize();
                }
            }

            this.turingMachine.Step();
            updateStateIndicator();
         
        }
    }
}
