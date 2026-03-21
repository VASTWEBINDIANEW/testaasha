
urlbackname = $('#backtodashboardcall').data('request-url');

// .load() method deprecated from jQuery 1.8 onward
$(window).on("load", function () {


     
    var laaa = $('#hdlatitudevaluealltrans').val();//id  has been defined in _viewstart
    var looo = $('#hdlongitudevaluealltrans').val();//id has defined in _viewstart

    $('#latss').val(laaa);
    $('#longloc').val(looo);
    if ($('#latss').val() == null || $('#latss').val() == '' || $('#longloc').val() == null || $('#longloc').val() == '') {
        

        location.href = urlbackname;
    }
});
$(document).ready(function () {   //same as: $(function() { 
  
    sleep(500).then(() => {
        //do stuff
    })
});

$(window).load(function () {
   
    sleep(500).then(() => {
        //do stuff
    })
});