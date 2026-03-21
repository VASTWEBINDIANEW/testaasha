/* call function continue button value */
function getcontinuebtnvalue(operatorddl,opvalueddl, circleddl, mobileno, radiovalue, amout)
{
    if (mobileno == "" || amout == "" )
    {
        if(mobileno == "")
        {
            document.getElementById("txtmobilemessage").innerHTML = "Please Enter Valid Mobile Number";
   
        }
        else
        {
            document.getElementById("txtmobilemessage").innerHTML = "";
        }
        
        if ((opvalueddl == "" || opvalueddl == null) && mobileno != "") {
            document.getElementById("mobileoptddl").innerHTML = "Please Select Operator";
         
        }
        else {
            document.getElementById("mobileoptddl").innerHTML = "";
        }
        if (circleddl == "Select Circle" && mobileno != "" && opvalueddl != "") {
            document.getElementById("mobileCircleddl").innerHTML = "Please Select Circle";

        }
        else {
            document.getElementById("mobileCircleddl").innerHTML = "";

        }
        if (amout == "" && mobileno != "" && opvalueddl != "" && circleddl != "Select Circle") {
            document.getElementById("txtmobileamtemessage").innerHTML = "Enter amount";

        }
        else {
            document.getElementById("txtmobileamtemessage").innerHTML = "";

        }
        event.preventDefault();
        return false;
    }
    else {
        document.getElementById("txtmobilemessage").innerHTML = "";
        document.getElementById("txtmobileamtemessage").innerHTML = "";
        var mobileno = document.getElementById("txtmobile").value;
          var mobilenolenght = document.getElementById("txtmobile").value.length;
           var pattern = "^[6-9]{1}[0-9]{9}$";
     if (mobilenolenght != 10 && mobilenolenght != null && mobilenolenght != "")
        {
          
            document.getElementById("txtmobilemessage").innerHTML = "Number should be [10] digits";
            document.getElementById("txtmobilemessage").style.display = "block";
            event.preventDefault();
            return false;
        }
     else if (!mobileno.match(pattern)) {
           
            document.getElementById("txtmobilemessage").innerHTML = "Your Mobile Number is not Valid.";
            document.getElementById("txtmobilemessage").style.display = "block";
            event.preventDefault();
            return false;
           
     }
        else if (opvalueddl == "" || opvalueddl == null) {
         document.getElementById("mobileoptddl").innerHTML = "Please Select Operator";
         event.preventDefault();
         return false;
     }
      
        else {
        
            document.getElementById("txtmobilemessage").innerHTML = "";
            document.getElementById("txtmobileamtemessage").innerHTML = "";
            return true;

        }
       
        
    }
}

/*hide and show viewplan and bestplan*/
function showplanandhideplan(optval)
{
    if(optval =="A")
    {
     document.getElementById("viewplan").style.display = "block";
        document.getElementById("bestplan").style.display = "block";
    }
    if (optval == "I") {
        document.getElementById("viewplan").style.display = "block";
        document.getElementById("bestplan").style.display = "block";
    }
        if(optval =="V")
        {
            document.getElementById("viewplan").style.display = "block";
            document.getElementById("bestplan").style.display = "block";
        }
        if (optval == "T") {
            document.getElementById("viewplan").style.display = "block";
            document.getElementById("bestplan").style.display = "none";
        }
        if (optval == "TS") {
            document.getElementById("viewplan").style.display = "block";
            document.getElementById("bestplan").style.display = "none";
        }
        if (optval == "B") {
            document.getElementById("viewplan").style.display = "block";
            document.getElementById("bestplan").style.display = "none";

        }
        if (optval == "BR") {
            document.getElementById("viewplan").style.display = "block";
            document.getElementById("bestplan").style.display = "none";
        }
        if (optval == "U") {
            document.getElementById("viewplan").style.display = "block";
            document.getElementById("bestplan").style.display = "none";
        }
        if (optval == "US") {
            document.getElementById("viewplan").style.display = "block";
            document.getElementById("bestplan").style.display = "none";
        }
        if (optval == "MT") {
            document.getElementById("viewplan").style.display = "block";
            document.getElementById("bestplan").style.display = "none";
        }
        if (optval == "MR") {
            document.getElementById("viewplan").style.display = "block";
            document.getElementById("bestplan").style.display = "none";
        }
        if (optval == "JIO") {
            document.getElementById("viewplan").style.display = "block";
            document.getElementById("bestplan").style.display = "block";
        }
}