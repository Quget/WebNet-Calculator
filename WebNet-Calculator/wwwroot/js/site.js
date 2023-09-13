
var inString = "";//calculator input string

//input from a calculator button(except for clr and = )
function calcInput(event) {
    inString += event.target.value;
    updateInput();
}

//= button has been pressed. Go to //Calculator?calcString= and calculator inputstring to calculate!
function submitInput()
{
    //encodeURIComponent so + can actually be used!
    window.location.replace('?calcString=' + encodeURIComponent(inString));
}

//clear input
function clearInput()
{
    inString = "";
    updateInput();
    window.location.replace('/');
}

//update user input.
function updateInput()
{
    document.getElementById('inputResult').innerHTML = inString;
}