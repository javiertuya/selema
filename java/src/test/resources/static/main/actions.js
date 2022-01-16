//Acciones a realizar en las alertas, confirms y prompts
function doAlert() {
    alert('alert msg'); 
    document.getElementById('spanAlert').textContent='clicked';
}
function doConfirm() {
    var ret=confirm('confirm msg'); 
    document.getElementById('spanConfirm').textContent=ret.toString();
}
function doPrompt() {
    var ret=prompt('confirm msg');
    if (ret==null) 
        ret='NULL'; 
    document.getElementById('spanPrompt').textContent=ret;
}