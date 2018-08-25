function test_function() {
    var test = get_text_input("So then, what's your name?", 14, "");
    show_text(text.Get('Main').Get('Debug2').GetString());
    show_text(text.Get('Main').Get('ExampleText').GetString());
    show_text(text.Get('Main').Get('Debug').GetString());
}

Console.WriteLine("hi");
wait(5);
Console.WriteLine("oo");

/* get_text_input @title @max_length @default_input
 * 
 * show_text @text
 * 
 * wait @seconds
 * 
 * set_player_control @state
 * 
 */