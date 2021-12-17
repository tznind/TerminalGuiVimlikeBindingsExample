

using NStack;
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
                Text = GetSomeLongText(),
                WordWrap = true,

                // start the text view in readonly mode so that navigation keybindings 
                // will not enter text.  In Vim you have to hit Enter to modify the text
                ReadOnly = true,
            };

            // navigation keybindings
            RegisterListViewHJKLBindings(lv1);
            RegisterListViewHJKLBindings(lv2);

            // Delete and Enter to activate
            lv1.KeyDown += (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter) // enter
                {
                    // 'open' the item - in a real world case we presumably would change the text in textView too or something
                    textView.FocusFirst();
                    e.Handled = true;
                }

                if (e.KeyEvent.KeyValue == 'x') // delete
                {
                    var idx = lv1.SelectedItem;

                    // cannot delete
                    if (idx < 0 || idx >= lv1.Source.Length)
                        return;

                    // what are they trying to delete?
                    var list = lv1.Source.ToList();
                    var selected = list[idx];

                    // confirm it
                    if(YesNo("Delete",$"Delete {selected}?"))
                    {
                        // remove it
                        list.Remove(selected);
                        lv1.SetSource(list);
                    }
                }
            };

            // navigation keybindings - from textview to other controls
            RegisterListViewHJKLBindings(textView);

            textView.KeyDown += (e) =>
            {
                if(e.KeyEvent.Key == Key.Enter) // enter edit mode
                {
                    textView.ReadOnly = false;
                    e.Handled = true;  // TODO: this does not seem to be respected (Enter still appears in control)
                }

                if (e.KeyEvent.Key == Key.Esc) // leave edit mode
                {
                    if(textView.ReadOnly)
                    {
                        // we are already in readonly mode so navigate back to lv1
                        lv1.FocusFirst();
                    }
                    else
                    {
                        textView.ReadOnly = true;
                        e.Handled = true;
                    }
                }
            };
            window.Add(lv1);
            window.Add(lv2);
            window.Add(textView);

            Application.Top.Add(window);

            Application.Run();
            Application.Shutdown();

        }

        private static void RegisterListViewHJKLBindings(TextView textView)
        {
            textView.KeyDown += (e) =>
            {
                if (textView.ReadOnly)
                {
                    if (e.KeyEvent.KeyValue == 'l') // right
                    {
                        textView.SuperView.FocusNext();
                        textView.SuperView.EnsureFocus();
                        e.Handled = true;
                    }
                    if (e.KeyEvent.KeyValue == 'h') // left
                    {
                        // TODO: this seems to break when on the first control
                        textView.SuperView.FocusPrev();
                        textView.SuperView.EnsureFocus();
                        e.Handled = true;
                    }
                    if (e.KeyEvent.KeyValue == 'k') // up
                    {
                        var desiredTop = textView.TopRow - 1;

                        // move the cursor to ensure it is on the screen after we scroll (prevents textview refusing to scroll down)
                        textView.CursorPosition = new Point(textView.CursorPosition.X, Math.Min(textView.CursorPosition.Y, desiredTop));
                        textView.ScrollTo(desiredTop);
                        e.Handled = true;
                    }
                    if (e.KeyEvent.KeyValue == 'j') // down
                    {
                        var desiredTop = textView.TopRow + 1;

                        // move the cursor to ensure it is on the screen after we scroll (prevents textview refusing to scroll down)
                        textView.CursorPosition = new Point(textView.CursorPosition.X, Math.Max(textView.CursorPosition.Y, desiredTop));
                        textView.ScrollTo(desiredTop);
                        e.Handled = true;
                    }
                }
            };
        }
        private static void RegisterListViewHJKLBindings(ListView listview)
        {
            listview.KeyDown += (e) =>
            {
                if (e.KeyEvent.KeyValue == 'l') // right
                {
                    listview.SuperView.FocusNext();
                    listview.SuperView.EnsureFocus();
                    e.Handled = true;
                }
                if (e.KeyEvent.KeyValue == 'h') // left
                {
                    listview.SuperView.FocusPrev();
                    listview.SuperView.EnsureFocus();
                    e.Handled = true;
                }
                if (e.KeyEvent.KeyValue == 'j') // down
                {
                    listview.SelectedItem = Math.Max(0, Math.Min(listview.Source.Count - 1, listview.SelectedItem + 1));
                    listview.SetNeedsDisplay();
                    e.Handled = true;
                }
                if (e.KeyEvent.KeyValue == 'k') // up
                {
                    listview.SelectedItem = Math.Max(0, listview.SelectedItem - 1);
                    listview.SetNeedsDisplay();
                    e.Handled = true;
                }
            };
        }

        private static ustring GetSomeLongText()
        {
            return @"
Terminal.Gui is a library intended to create console-based applications using C#. The framework has been designed to make it easy to write applications that will work on monochrome terminals, as well as modern color terminals with mouse support.

This library works across Windows, Linux and MacOS.

This library provides a text-based toolkit as works in a way similar to graphic toolkits. There are many controls that can be used to create your applications and it is event based, meaning that you create the user interface, hook up various events and then let the a processing loop run your application, and your code is invoked via one or more callbacks.

The simplest application looks like this:

using Terminal.Gui;

[...]

This example shows a prompt and returns an integer value depending on which value was selected by the user (Yes, No, or if they use chose not to make a decision and instead pressed the ESC key).

More interesting user interfaces can be created by composing some of the various views that are included. In the following sections, you will see how applications are put together.

In the example above, you can see that we have initialized the runtime by calling the Init method in the Application class - this sets up the environment, initializes the color schemes available for your application and clears the screen to start your application.

The Application class, additionally creates an instance of the Toplevel class that is ready to be consumed, this instance is available in the Application.Top property, and can be used like this:
";
        }

        /// <summary>
        /// Poses a yes no question to the user using a modal dialog
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static bool YesNo(string title, string message)
        {
            bool answer = false;

            var yes = new Button("Yes", true) { HotKey = Key.Y /* TODO: Requires user to hit Alt+Y */};
            yes.Clicked += () =>
            {
                answer = true;
                Application.RequestStop();
            };

            var no = new Button("No") { HotKey = Key.N /* TODO: Requires user to hit Alt+N */};
            no.Clicked += () =>
            {
                answer = false;
                Application.RequestStop();
            };

            var dlg = new Dialog(title, yes, no)
            {
                Height = 10,
                Width = 20,
            };

            dlg.KeyDown += (e) =>
            {
                if (e.KeyEvent.KeyValue == 'y') // yes
                {
                    answer = true;
                    e.Handled = true;
                    Application.RequestStop();
                }
                if (e.KeyEvent.KeyValue == 'n') // no
                {
                    answer = false;
                    e.Handled = true;
                    Application.RequestStop();
                }
            };
            dlg.Add(new Label(message));

            Application.Run(dlg);

            return answer;
        }
    }
}
