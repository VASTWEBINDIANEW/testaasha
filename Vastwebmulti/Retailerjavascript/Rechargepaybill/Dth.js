/* get dth dropdown text */

function getdthdropdowntext(x)
{
    if(x == "Airtel Digital TV")
    {
        document.getElementById("txtdthcardno").placeholder = "Enter Customer Id (start with 3)";
        document.getElementById("txtdthcardno").value = "";
        document.getElementById("dthmessage").innerHTML = "";
        document.getElementById("dthamtmessage").innerHTML = "";
    }
    if (x == "Dish TV")
    {
        document.getElementById("txtdthcardno").placeholder = "Viewing Card Number (start with 0)";
        document.getElementById("txtdthcardno").value = "";
        document.getElementById("dthmessage").innerHTML = "";
        document.getElementById("dthamtmessage").innerHTML = "";
    }
    if(x == "Big TV")
    {
        document.getElementById("txtdthcardno").placeholder = "Smart Card Number (start with 2)";
        document.getElementById("txtdthcardno").value = "";
        document.getElementById("dthmessage").innerHTML = "";
        document.getElementById("dthamtmessage").innerHTML = "";
    }
    if (x == "Sun Direct") {
        document.getElementById("txtdthcardno").placeholder = "Enter Smart Card Number";
        document.getElementById("txtdthcardno").value = "";
        document.getElementById("dthmessage").innerHTML = "";
        document.getElementById("dthamtmessage").innerHTML = "";
    }
    if(x == "Tata Sky")
    {
        document.getElementById("txtdthcardno").placeholder = "Mobile No.(Subscriber-ID start with 1)";
        document.getElementById("txtdthcardno").value = "";
        document.getElementById("dthmessage").innerHTML = "";
        document.getElementById("dthamtmessage").innerHTML = "";
    }
    if (x == "Videocon D2H") {
        document.getElementById("txtdthcardno").placeholder = "Enter Customer ID";
        document.getElementById("txtdthcardno").value = "";
        document.getElementById("dthmessage").innerHTML = "";
        document.getElementById("dthamtmessage").innerHTML = "";
    }
}
/* get Airtel Digital TV" continue button value */
function getdthcontiunebtn(ddldth, custromerid, dthamount)
{
    if (ddldth == "Select Operators") { 
        document.getElementById("dthoperator").innerHTML = "Select Any Operator";
        event.preventDefault();
        return false;
    }
    else {
        document.getElementById("dthoperator").innerHTML = "";
        if ((custromerid == "" || dthamount == "") && ddldth == "Airtel Digital TV") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter your customer Id";
              
            }
            else {
                document.getElementById("dthmessage").innerHTML = "";
               
            }
            if (dthamount == "") {
                document.getElementById("dthamtmessage").innerHTML = "Enter amount";
               
            }
            else {
                document.getElementById("dthamtmessage").innerHTML = "";
            }
            event.preventDefault();
        }
        else {
            if (ddldth == "Airtel Digital TV") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;


                if (dthcardno != 10 && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [10] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else if (custromerid.charAt(0) != "3") {
                    document.getElementById("dthmessage").innerHTML = "it should start with 3";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }

                else {

                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;
                }



            }
        }

        /* Dish TV */
        if ((custromerid == "" || dthamount == "") && ddldth == "Dish TV") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter your Viewing Card Number";
            }
            else {
                document.getElementById("dthmessage").innerHTML = "";
            }
            if (dthamount == "") {
                document.getElementById("dthamtmessage").innerHTML = "Enter amount";
            }
            else {
                document.getElementById("dthamtmessage").innerHTML = "";
            }
            event.preventDefault();
        }
        else {
            if (ddldth == "Dish TV") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;
                if (dthcardno != 11 && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [11] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else if (custromerid.charAt(0) != "0") {
                    document.getElementById("dthmessage").innerHTML = "it should start with 0";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;
                }
            }
        }

        /* Big TV */
        if ((custromerid == "" || dthamount == "") && ddldth == "Big TV") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter Smart Card Number";
            }
            else {
                document.getElementById("dthmessage").innerHTML = "";
            }
            if (dthamount == "") {
                document.getElementById("dthamtmessage").innerHTML = "Enter amount";
            }
            else {
                document.getElementById("dthamtmessage").innerHTML = "";
            }
            event.preventDefault();
        }
        else {
            if (ddldth == "Big TV") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;
                if (dthcardno != 12 && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [12] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else if (custromerid.charAt(0) != "2") {
                    document.getElementById("dthmessage").innerHTML = "it should start with 2";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();

                    return false;
                }
                else {
                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;
                }
            }
        }

        /* Sun Direct */
        if ((custromerid == "" || dthamount == "") && ddldth == "Sun Direct") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter Smart Card Number";
            }
            else {
                document.getElementById("dthmessage").innerHTML = "";
            }
            if (dthamount == "") {
                document.getElementById("dthamtmessage").innerHTML = "Enter amount";
            }
            else {
                document.getElementById("dthamtmessage").innerHTML = "";
            }
            event.preventDefault();
        }
        else {
            if (ddldth == "Sun Direct") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;
                if (dthcardno != 11 && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [11] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                //else if ((custromerid.charAt(0) != "1") && custromerid.charAt(0) != "4") {
                //    document.getElementById("dthmessage").innerHTML = "it should start with 1 or 4";
                //    document.getElementById("dthmessage").style.display = "block";
                //    event.preventDefault();
                //    return false;
                //}

                else {
                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;
                }
            }
        }
        /* Tata Sky */
        if ((custromerid == "" || dthamount == "") && ddldth == "Tata Sky") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter mobile no and subscriber-ID  ";
            }
            else {
                document.getElementById("dthmessage").innerHTML = "";
            }
            if (dthamount == "") {
                document.getElementById("dthamtmessage").innerHTML = "Enter amount";
            }
            else {
                document.getElementById("dthamtmessage").innerHTML = "";
            }
            event.preventDefault();
        }
        else {
            if (ddldth == "Tata Sky") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;
                if (dthcardno != 10 && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [10] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else if (custromerid.charAt(0) != "1") {
                    document.getElementById("dthmessage").innerHTML = "it should start with 1";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;
                }
            }
        }

        /* Videocon D2H*/
        if ((custromerid == "" || dthamount == "") && ddldth == "Videocon D2H") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter Customer ID";
            }
            else {
                document.getElementById("dthmessage").innerHTML = "";
            }
            if (dthamount == "") {
                document.getElementById("dthamtmessage").innerHTML = "Enter amount";
            }
            else {
                document.getElementById("dthamtmessage").innerHTML = "";
            }
            event.preventDefault();
        }
        else {
            if (ddldth == "Videocon D2H") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;
                if ((dthcardno < 8 || dthcardno > 10) && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [8-10] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;
                }
            }
        }
    }
}

/*hide and show checkplan and viewplan*/
function showdthplanandhideplan(opttext) {
    if (opttext == "Airtel Digital TV") {
        document.getElementById("checkplan").style.display = "none";
        document.getElementById("dthtvviewplan").style.display = "block";
        $('#dthplaan').text('Customer Info');
    }
    if (opttext == "Dish TV") {
        document.getElementById("checkplan").style.display = "none";
        document.getElementById("dthtvviewplan").style.display = "block";
        $('#dthplaan').text('Customer Info');
    }
    if (opttext == "Big TV") {
        document.getElementById("checkplan").style.display = "none";
        document.getElementById("dthtvviewplan").style.display = "none";
    }
    if (opttext == "Sun Direct") {
        document.getElementById("checkplan").style.display = "none";
        document.getElementById("dthtvviewplan").style.display = "block";
        $('#dthplaan').text('Customer Info');
    }
    if (opttext == "Tata Sky") {
        document.getElementById("checkplan").style.display = "none";
        document.getElementById("dthtvviewplan").style.display = "block";
        $('#dthplaan').text('Customer Info');
    }
    if (opttext == "Videocon D2H") {
        document.getElementById("checkplan").style.display = "none";
        document.getElementById("dthtvviewplan").style.display = "block";
        $('#dthplaan').text('Customer Info');
    }
    
 
}

//best dth offer plan on click view plan button
function validationbsetoffer(ddldth, custromerid) {
    if (ddldth == "Select Operators") {
        document.getElementById("dthoperator").innerHTML = "Select Any Operator";
        event.preventDefault();
        return false;
    }
    else {
        document.getElementById("dthoperator").innerHTML = "";
        if (custromerid == "" && ddldth == "Airtel Digital TV") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter your customer Id";

            }
            else {
                document.getElementById("dthmessage").innerHTML = "";

            }
        
            event.preventDefault();
            return false;
        }
        else {
            if (ddldth == "Airtel Digital TV") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;


                if (dthcardno != 10 && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [10] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else if (custromerid.charAt(0) != "3") {
                    document.getElementById("dthmessage").innerHTML = "it should start with 3";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }

                else {

                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;
                }



            }
        }

        /* Dish TV */
        if (custromerid == ""  && ddldth == "Dish TV") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter your Viewing Card Number";
            }
            else {
                document.getElementById("dthmessage").innerHTML = "";
            }

            event.preventDefault();
            return false;
        }
        else {
            if (ddldth == "Dish TV") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;
                if (dthcardno != 11 && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [11] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else if (custromerid.charAt(0) != "0") {
                    document.getElementById("dthmessage").innerHTML = "it should start with 0";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;
                }
            }
        }

        /* Tata Sky */
        if (custromerid == ""  && ddldth == "Tata Sky") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter mobile no and subscriber-ID  ";
            }
            else {
                document.getElementById("dthmessage").innerHTML = "";
            }

            event.preventDefault();
            return false;
        }
        else {
            if (ddldth == "Tata Sky") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;
                if (dthcardno != 10 && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [10] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else if (custromerid.charAt(0) != "1") {
                    document.getElementById("dthmessage").innerHTML = "it should start with 1";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;
                }
            }
        }
        if (custromerid == "" && ddldth == "Sun Direct") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter mobile no and subscriber-ID  ";
            }
            else {
                document.getElementById("dthmessage").innerHTML = "";
            }

            event.preventDefault();
            return false;
        }
        else {
            if (ddldth == "Sun Direct") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;
                if (dthcardno != 11 && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [11] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else /*if (custromerid.charAt(0) == "1" || custromerid.charAt(0) == "4" || custromerid.charAt(0) == "7") */{

                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;

                  
                }
                //else {
                //    document.getElementById("dthmessage").innerHTML = "it should start with 1 or 4 or 7";
                //    document.getElementById("dthmessage").style.display = "block";
                //    event.preventDefault();
                //    return false;
                //}
            }
        }

        /* Videocon D2H*/
        if (custromerid == ""  && ddldth == "Videocon D2H") {
            if (custromerid == "") {
                document.getElementById("dthmessage").innerHTML = "Enter Customer ID";
            }
            else {
                document.getElementById("dthmessage").innerHTML = "";
            }
        

            event.preventDefault();
            return false;
        }
        else {
            if (ddldth == "Videocon D2H") {
                document.getElementById("dthmessage").innerHTML = "";
                document.getElementById("dthamtmessage").innerHTML = "";
                dthcardno = document.getElementById("txtdthcardno").value.length;
                if ((dthcardno < 8 || dthcardno > 10) && txtdthcardno != "" && txtdthcardno != null) {
                    document.getElementById("dthmessage").innerHTML = "Number should be [8-10] digits";
                    document.getElementById("dthmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("dthmessage").innerHTML = "";
                    document.getElementById("dthamtmessage").innerHTML = "";
                    return true;
                }
            }
        }
    }
}