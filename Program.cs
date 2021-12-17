

using System.Collections.Generic;
using Terminal.Gui;

namespace VimlikeBindings
{
    public class Program
    {
        public static void Main()
        {
            Application.Init();

            /*******************************
             *           |
             *    lv1    |
             *           | 
             *           |
             *           |
             * ----------|     textView
             *           |
             *           | 
             *    lv2    |
             *           |
             *           |
             *           |
             * ****************************/


            var window = new Window();

            var lv1 = new ListView(new List<string>{"File 1","File 2"})
            {
                Width = Dim.Percent(30),
                Height= Dim.Percent(50),
            };

            var lv2 = new ListView(new List<string> { "Operation 1", "Operation 2" })
            {
                Y = Pos.Center(),
                Width = Dim.Percent(30),
                Height = Dim.Percent(50),
            };

            var textView = new TextView()
            {
                X = Pos.Percent(30) + 1,
                Width = Dim.Percent(70),
                Height = Dim.Fill(),
                Text = "Some text",

                // start the text view in readonly mode so that navigation keybindings 
                // will not enter text.  In Vim you have to hit Enter to modify the text
                ReadOnly = true,
            };

            // navigation keybindings - from lv1 to other controls
            lv1.KeyDown += (e) =>
            {
                if (e.KeyEvent.KeyValue == 'l') // right
                {
                    textView.FocusFirst();
                    e.Handled = true;
                }
                if (e.KeyEvent.KeyValue == 'j') // down
                {
                    lv2.FocusFirst();
                    e.Handled = true;
                }
            };

            // navigation keybindings - from lv2 to other controls
            lv2.KeyDown += (e) =>
            {
                if (e.KeyEvent.KeyValue == 'l') // right
                {
                    textView.FocusFirst();
                    e.Handled = true;
                }
                if (e.KeyEvent.KeyValue == 'k') // up
                {
                    lv1.FocusFirst();
                    e.Handled = true;
                }
            };

            // navigation keybindings - from textview to other controls
            textView.KeyDown += (e) =>
            {
                if (e.KeyEvent.KeyValue == 'h') // left
                {
                    if(textView.ReadOnly)
                    {
                        lv1.FocusFirst();
                        e.Handled = true;
                    }
                }
                if(e.KeyEvent.Key == Key.Enter) // enter edit mode
                {
                    textView.ReadOnly = false;
                    e.Handled = true;  // TODO: this does not seem to be respected (Enter still appears in control)
                }

                if (e.KeyEvent.Key == Key.Esc) // leave edit mode
                {
                    textView.ReadOnly = true;
                    e.Handled = true;
                }
            };
            window.Add(lv1);
            window.Add(lv2);
            window.Add(textView);

            Application.Top.Add(window);

            Application.Run();
            Application.Shutdown();

        }
    }
}
